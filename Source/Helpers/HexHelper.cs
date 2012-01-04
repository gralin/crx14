namespace Gralin.NETMF.ST.CRX14.Helpers
{
    public static class HexHelper
    {
        const string Hex = "0123456789ABCDEF";

        public static string ToHex(byte b)
        {
            return new string(new[] { Hex[b >> 4], Hex[b & 0x0F] });
        }
    }
}