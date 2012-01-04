namespace Gralin.NETMF.ST.CRX14
{
    public interface IDevice
    {
        bool Write(byte regAddress, byte[] data);
        byte[] Read(byte regAddress, byte dataLength);
        byte[] WriteRead(byte regAddress, byte[] data, byte readDataLength);
    }
}