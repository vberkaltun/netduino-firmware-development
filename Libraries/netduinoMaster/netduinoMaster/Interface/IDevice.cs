namespace netduinoMaster
{
    public interface IDevice
    {
        byte Address { get; set; }
        SFunctionArray Function { get; set; }
        EHandshake Handshake { get; set; }
        SVendor Vendor { get; set; }
    }
}