namespace netduinoMaster
{
    public interface IDevice
    {
        byte Address { get; }
        SFunctionArray Function { get; }
        EHandshake Handshake { get; }
        SVendor Vendor { get; }
    }
}