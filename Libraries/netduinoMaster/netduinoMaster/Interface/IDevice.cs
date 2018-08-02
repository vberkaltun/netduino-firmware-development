namespace netduinoMaster
{
    interface IDevice
    {
        SVendor Vendor { get; set; }
        SFunction[] Function { get; set; }
        EHandshake Handshake { get; set; }
        char Address { get; set; }
    }
}