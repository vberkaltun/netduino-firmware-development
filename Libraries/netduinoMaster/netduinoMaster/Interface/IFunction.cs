namespace netduinoMaster
{
    public interface IFunction
    {
        bool Listen { get; }
        string Name { get; }
        bool Request { get; }
    }
}