using System;
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

            Debug.EnableGCMessages(false);
            Debug.Print("Waiting for TAG...");

            while (true)
            {
                var tags = crx14.FindTags();

                if (tags != null && tags.Length > 0)
                {
                    Debug.Print("Found: " + tags.Length);

                    foreach (var tag in tags)
                    {
                        try
                        {
                            Debug.Print("TAG: " + tag);

                            const byte blockNum = 0;

                            Debug.Print("Reading eeprom block " + blockNum);
                            var bytes = crx14.ReadEeprom(tag, blockNum);
                            Debug.Print("bytes.Length = " + bytes.Length);
                            Debug.Print("bytes[0] = " + bytes[0]);

                            Debug.Print("Incrementing and saving");
                            bytes[0]++;
                            crx14.WriteEeprom(tag, 0, bytes);

                            Debug.Print("Reading eeprom block " + blockNum);
                            // to prove the value is read from TAG
                            bytes[0] = 0;
                            bytes = crx14.ReadEeprom(tag, blockNum);
                            Debug.Print("bytes[0] = " + bytes[0]);

                            led.Write(!led.Read());
                            crx14.UnselectTags();
                        }
                        catch (ArgumentException)
                        {
                            Debug.Print("Failed to select TAG, plase it closer to reader!");
                        }
                        finally
                        {
                            Debug.Print("");
                            Debug.Print("===============================");
                            Debug.Print("");
                        }
                    }
                }

                Thread.Sleep(1000);
            }
        }
    }
}
