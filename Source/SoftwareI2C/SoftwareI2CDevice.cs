using Microsoft.SPOT.Hardware;

namespace Gralin.NETMF.ST.CRX14.SoftwareI2C
{
    public class SoftwareI2CDevice : IDevice
    {
        private readonly SoftwareI2C _device;
        private readonly byte _readCmd;
        private readonly byte _writeCmd;

        public SoftwareI2CDevice(byte address, Cpu.Pin sdaPin, Cpu.Pin sclPin)
        {
            _device = new SoftwareI2C(sclPin, sdaPin, 400);
            _writeCmd = (byte) ((address << 1) + 0);
            _readCmd = (byte) ((address << 1) + 1);
        }

        #region IDevice Members

        public bool Write(byte regAddress, byte[] data)
        {
            var noData = data == null || data.Length == 0;

            _device.Transmit(true, false, _writeCmd);
            _device.Transmit(false, noData, regAddress);

            if (!noData)
                for (var i = 0; i < data.Length; i++)
                    _device.Transmit(false, i + 1 == data.Length, data[i]);

            return true;
        }

        public byte[] Read(byte regAddress, byte dataLength)
        {
            return WriteRead(regAddress, null, dataLength);
        }

        public byte[] WriteRead(byte regAddress, byte[] data, byte readDataLength)
        {
            Write(regAddress, data);

            var noData = readDataLength == 0;
            var result = new byte[readDataLength];

            _device.Transmit(true, noData, _readCmd);

            if (!noData)
                for (var i = 0; i < readDataLength; i++)
                    result[i] = _device.Receive(i + 1 < readDataLength, i + 1 == readDataLength);

            return result;
        }

        #endregion
    }
}