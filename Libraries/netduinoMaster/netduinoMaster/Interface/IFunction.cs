namespace netduinoMaster
{
    public interface IFunction
    {
        bool Listen { get; set; }
        string Name { get; set; }
        bool Request { get; set; }
    }
}