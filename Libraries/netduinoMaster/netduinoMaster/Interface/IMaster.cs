namespace netduinoMaster
{
    public interface IMaster
    {
        SFunctionArray Function { get; set; }
        string Receive { get; set; }
        string Transmit { get; set; }
    }
}