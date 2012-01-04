using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

namespace Gralin.NETMF.ST.CRX14
{
    public class Program
    {
        // Adjusted for FEZ Domino
        public const Cpu.Pin SdaPin = (Cpu.Pin)33; //FEZ_Pin.Digital.Di2
        public const Cpu.Pin SclPin = (Cpu.Pin)31; //FEZ_Pin.Digital.Di3
        public const Cpu.Pin LedPin = (Cpu.Pin)4; //FEZ_Pin.Digital.LED

        // This is my tag uid, ours will be different ;)
        public const string MyTagUid = "1910D552031A02D0";

        public static void Main()
        {
            var crx14 = CRX14.UseSoftwareI2C(0, SdaPin, SclPin);
            var led = new OutputPort(LedPin, false);

            Debug.Print("Waiting for TAG...");

            while (true)
            {
                var tags = crx14.FindTags();

                if (tags != null && tags.Length > 0)
                {
                    Debug.Print("Found: " + tags.Length);

                    foreach (var tag in tags)
                    {
                        Debug.Print("TAG: " + tag);
                        var eeprom = crx14.ReadEeprom(tag);
                        Debug.Print("Eeprom: " + eeprom.Length + " bytes");
                    }

                    break;
                }

                Thread.Sleep(1000);
            }

            led.Write(true);
            Debug.Print("Finished");
            Thread.Sleep(Timeout.Infinite);
        }
    }
}
