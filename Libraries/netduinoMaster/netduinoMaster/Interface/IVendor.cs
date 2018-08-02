namespace netduinoMaster
{
    public interface IVendor
    {
        string Brand { get; set; }
        string Model { get; set; }
        string Version { get; set; }
    }
}