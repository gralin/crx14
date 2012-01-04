using Gralin.NETMF.ST.CRX14.Helpers;

namespace Gralin.NETMF.ST.CRX14
{
    public struct Tag
    {
        public byte ChipId { get; set; }
        public byte[] Uid { get; set; }

        public override string ToString()
        {
            return "ChipId: " + ChipId + ", Uid: " + UidToStr(this);
        }

        private static string UidToStr(Tag tag)
        {
            var result = string.Empty;
            foreach (var b in tag.Uid)
                result += HexHelper.ToHex(b);
            return result;
        }
    }
}