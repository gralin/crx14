using System;
using Microsoft.SPOT.Hardware;

namespace Gralin.NETMF.ST.CRX14.NativeI2C
{
    public class NativeI2CDevice : IDevice
    {
        private readonly I2CDevice _device;

        public NativeI2CDevice(byte address)
        {
            _device = new I2CDevice(new I2CDevice.Configuration(address, 400));
        }

        public bool Write(byte regAddress, byte[] data)
        {
            var transactions = new I2CDevice.I2CTransaction[] { GetWriteTransaciton(regAddress, data) };
            return _device.Execute(transactions, 1000) == 0;
        }

        public byte[] Read(byte regAddress, byte dataLength)
        {
            var write = new[] { regAddress };
            var read = new byte[dataLength];
            var transactions = new I2CDevice.I2CTransaction[]
            {
                I2CDevice.CreateWriteTransaction(write),
                I2CDevice.CreateReadTransaction(read)
            };

            return _device.Execute(transactions, 1000) > 0 ? read : null;
        }

        public byte[] WriteRead(byte regAddress, byte[] data, byte readDataLength)
        {
            var read = new byte[readDataLength];
            var transactions = new I2CDevice.I2CTransaction[]
            {
                GetWriteTransaciton(regAddress, data),
                I2CDevice.CreateReadTransaction(read)
            };

            return _device.Execute(transactions, 1000) > 0 ? read : null;
        }

        private static I2CDevice.I2CWriteTransaction GetWriteTransaciton(byte address, byte[] data)
        {
            var write = new byte[data.Length + 1];
            write[0] = address;
            Array.Copy(data, 0, write, 1, data.Length);
            return I2CDevice.CreateWriteTransaction(write);
        }
    }
}