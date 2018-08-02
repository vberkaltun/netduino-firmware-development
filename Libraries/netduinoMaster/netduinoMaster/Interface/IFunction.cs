namespace netduinoMaster
{
    interface IFunction
    {
        string Name { get; set; }
        bool Request { get; set; }
        ushort Listen { get; set; }
    }
}
