namespace netduinoMaster
{
    public interface IFunction
    {
        ushort Listen { get; set; }
        string Name { get; set; }
        bool Request { get; set; }
    }
}