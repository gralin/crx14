using System;
using System.Collections;
using System.Threading;
using Gralin.NETMF.ST.CRX14.NativeI2C;
using Gralin.NETMF.ST.CRX14.SoftwareI2C;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace Gralin.NETMF.ST.CRX14
{
    public class CRX14
    {
        const byte DeviceCode = 0xA;

        const byte BlockOffset = 7;
        const byte BlockCount = 9;
        const byte BytesPerBlock = 4;

        protected IDevice Device;
        
        protected CRX14(IDevice device)
        {
            Device = device;
        }

        public static CRX14 UseNativeI2C(byte address)
        {
            return new CRX14(new NativeI2CDevice(FormatAddress(address)));
        }

        public static CRX14 UseSoftwareI2C(byte address, Cpu.Pin sdaPin, Cpu.Pin sclPin)
        {
            return new CRX14(new SoftwareI2CDevice(FormatAddress(address), sdaPin, sclPin));
        }

        protected static byte FormatAddress(byte address)
        {
            return (byte) ((DeviceCode << 3) + (address & 7));
        }

        /// <summary>
        ///  Turn OFF electromagnetic field 
        /// </summary>
        public bool TurnOff()
        {
            return UnselectTags() && WriteParameter(0x00);
        }

        /// <summary>
        /// Turn ON electromagnetic field and initialize CRX14 
        /// </summary>
        public bool TurnOn()
        {
            return TurnOff() && WriteParameter(0x10);
        }

        /// <summary>
        /// Checks for available TAGs
        /// </summary>
        public Tag[] FindTags()
        {
            if (!TurnOn())
                return null;

            if (Initiate() == 0)
                return TurnOff() ? new Tag[0] : null;

            var response = SlotMarker();
            var detectFlags = (ushort)((response[2] << 8) + response[1]);
            var tags = new ArrayList();
            var index = 0;

            while (detectFlags > 0)
            {
                if ((detectFlags & 0x0001) > 0)
                {
                    var tag = new Tag { ChipId = response[index + 3] };

                    if (SelectTag(tag))
                    {
                        tag.Uid = GetTagUid();
                        tags.Add(tag);
                    }
                    else
                    {
                        Debug.Print("Failed to select TAG");
                    }
                }

                detectFlags >>= 1;
                index++;
            }

            var result = new Tag[tags.Count];

            for (var i = 0; i < tags.Count; i++)
                result[i] = (Tag)tags[i];

            return result;
        }

        public byte[] ReadEeprom(Tag tag)
        {
            if (!SelectTag(tag))
                throw new ArgumentException("Unable to select tag");

            var result = new byte[BlockCount * BytesPerBlock];

            for (byte i = 0; i < result.Length; i += BytesPerBlock)
            {
                var blockNum = (byte) (BlockOffset + i/BytesPerBlock);
                Array.Copy(WriteReadFrame(ISO14443B.ReadBlockFrame(blockNum), 1 + BytesPerBlock), 1,
                           result, i,
                           BytesPerBlock);
            }

            return result;
        }

        public byte[] ReadEeprom(Tag tag, byte blockNum)
        {
            if (blockNum < 0 || blockNum > BlockCount - 1)
                throw new ArgumentException("Invalid block number");

            if (!SelectTag(tag))
                throw new ArgumentException("Unable to select tag");

            blockNum += BlockOffset;

            var result = new byte[4];

            Array.Copy(WriteReadFrame(ISO14443B.ReadBlockFrame(blockNum), 1 + BytesPerBlock), 1, 
                       result, 0, 
                       4);
            
            return result;
        }

        public void WriteEeprom(Tag tag, byte[] data)
        {
            if (data.Length % BytesPerBlock > 0)
                throw new ArgumentException("Multiple of 4 bytes expected");

            if (data.Length / BytesPerBlock > BlockCount)
                throw new ArgumentException("To many bytes");

            if (!SelectTag(tag))
                throw new ArgumentException("Unable to select tag");

            var blockNum = BlockOffset;
            var dataOffset = (byte)0;

            for (byte i = 0; i < data.Length; i+=BytesPerBlock)
            {
                WriteEeprom(blockNum, data, dataOffset);
                blockNum++;
                dataOffset += BytesPerBlock;
            }
        }

        public void WriteEeprom(Tag tag, byte blockNum, byte[] data)
        {
            if (data == null || data.Length != BytesPerBlock)
                throw new ArgumentException("Invalid data length");

            if (blockNum < 0 || blockNum > BlockCount - 1)
                throw new ArgumentException("Invalid block number");

            if (!SelectTag(tag))
                throw new ArgumentException("Unable to select tag");

            WriteEeprom((byte) (BlockOffset + blockNum), data, 0);
        }

        /// <summary>
        /// TurnOff TAGs
        /// </summary>
        public bool UnselectTags()
        {
            return WriteFrame(ISO14443B.CompletionFrame);
        }

        /// <summary>
        /// Initiate all TAGs present in the electromagnetic field
        /// </summary>
        public byte Initiate()
        {
            return WriteReadFrame(ISO14443B.InitiationFrame, 2)[0];
        }

        /// <summary>
        /// Check all TAGs present in the electromagnetic field 
        /// </summary>
        public byte[] SlotMarker()
        {
            Device.Write((byte)Registers.SlotMarker, null);
            Thread.Sleep(20);
            return ReadFrame(19);
        }

        /// <summary>
        /// Select a TAG
        /// </summary>
        public bool SelectTag(Tag tag)
        {
            var response = WriteReadFrame(ISO14443B.SelectionFrame(tag.ChipId), 2);
            return response != null && response.Length == 2 && response[1] == tag.ChipId;
        }

        /// <summary>
        /// Write frame to CRX output buffer and read frame from input buffer
        /// </summary>
        public byte[] WriteReadFrame(byte[] writeFrame, byte readFrameLength)
        {
            WriteFrame(writeFrame);
            Thread.Sleep(20);
            return ReadFrame(readFrameLength);
        }

        /// <summary>
        /// Read frame from CRX14 input buffer
        /// </summary>
        public byte[] ReadFrame(byte frameLength)
        {
            return Device.Read((byte)Registers.IOFrame, frameLength);
        }

        /// <summary>
        /// Write frame to CRX14 output buffer
        /// </summary>
        public bool WriteFrame(byte[] frame)
        {
            return Device.Write((byte)Registers.IOFrame, frame);
        }

        /// <summary>
        /// Get unique Uid of selected TAG 
        /// </summary>
        public byte[] GetTagUid()
        {
            WriteFrame(ISO14443B.GetUidFrame);
            Thread.Sleep(20);
            var result = ReadFrame(9);

            if (result == null || result.Length != 9)
                return null;

            var uid = new byte[8];
            Array.Copy(result, 1, uid, 0, uid.Length);
            return uid;
        }

        protected void WriteEeprom(byte blockNum, byte[] data, byte dataOffset)
        {
            WriteFrame(ISO14443B.WriteBlockFrame(blockNum, data, dataOffset));
            // delay added so data can be saved
            Thread.Sleep(5);
        }

        protected bool WriteParameter(byte value)
        {
            var result = Device.WriteRead((byte)Registers.Parameter, new[] { value }, 1);
            return result != null && result.Length > 0 && result[0] == value;
        }
    }
}