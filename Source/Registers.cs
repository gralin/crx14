namespace Gralin.NETMF.ST.CRX14
{
    public enum Registers : byte
    {
        /// <summary>
        /// The Parameter Register is an 8-bit volatile register used to configure the CRX14, and thus,
        /// to customize the circuit behavior. The Parameter Register is located at the I²C address 00h
        /// and it is accessible in I²C Read and Write modes. Its default value, 00h, puts the CRX14 in
        /// standard ISO14443 type-B configuration.
        /// </summary>
        Parameter = 0x00,
        /// <summary>
        /// The Input/Output Frame Register is a 36-Byte buffer that is accessed serially from Byte 0
        /// through to Byte 35. The Input/Output Frame Register is the buffer in which the CRX14 stores 
        /// the data Bytes of the request frame to be sent to the PICC. It automatically stores 
        /// the data Bytes of the answer frame received from the PICC. The first Byte (Byte 0) 
        /// of the Input/Output Frame Register is used to store the frame length for both
        /// transmission and reception.
        /// </summary>
        IOFrame = 0x01,
        /// <summary>
        /// The Authenticate Register is used to trigger the complete authentication exchange between
        /// the CRX14 and the secured ST short range memory. The Authentication system is based 
        /// on a proprietary challenge/response mechanism that allows the application software to 
        /// authenticate a secured ST short range memory of the SRXxxx family. A reader designed with 
        /// the CRX14 can check the authenticity of a memory device and protect the application system 
        /// against silicon copies or emulators.
        /// </summary>
        Authenticate = 0x02,
        /// <summary>
        /// The slot Marker Register is used to trigger an automated anti-collision sequence between 
        /// the CRX14 and any ST short range memory present in the electromagnetic field. 
        /// With one I²C access, the CRX14 launches a complete stream of commands starting from PCALL16(),
        /// SLOT_MARKER(1), SLOT_MARKER(2) up to SLOT_MARKER(15), and stores all the identified Chip_IDs
        /// into the Input/Output Frame Register (I²C address 01h).
        /// </summary>
        SlotMarker = 0x03
    }
}