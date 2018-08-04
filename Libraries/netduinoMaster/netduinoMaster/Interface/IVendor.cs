namespace netduinoMaster
{
    public interface IVendor
    {
        string Brand { get; }
        string Model { get; }
        string Version { get; }
    }
}