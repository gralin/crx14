using System;

namespace Gralin.NETMF.ST.CRX14
{
    public class ISO14443B
    {
        public enum Commands : byte
        {
            Initiate = 0x06,
            ReadBlock = 0x08,
            WriteBlock = 0x09,
            GetUid = 0x0B,
            ResetToInventory = 0x0C,
            Select = 0x0E,
            Completion = 0x0F
        }

        public static byte[] CompletionFrame = new byte[] {0x01, (byte)Commands.Completion};
        public static byte[] InitiationFrame = new byte[] {0x02, (byte)Commands.Initiate, 0x00 };
        public static byte[] GetUidFrame = new byte[] { 0x01, (byte)Commands.GetUid };

        public static byte[] SelectionFrame(byte chipId)
        {
            return new byte[] {0x02, (byte)Commands.Select, chipId};
        }

        public static byte[] ReadBlockFrame(byte blockNum)
        {
            return new byte[] {0x02, (byte)Commands.ReadBlock, blockNum};
        }

        public static byte[] WriteBlockFrame(byte blockNum, byte[] block, byte blockOffset = 0)
        {
            var result = new byte[7];
            result[0] = 6;
            result[1] = (byte) Commands.WriteBlock;
            result[2] = blockNum;
            Array.Copy(block, blockOffset, result, 3, 4);
            return result;
        }
    }
}