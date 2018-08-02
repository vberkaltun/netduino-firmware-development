namespace netduinoMaster
{
    interface IMaster
    {
        SFunction[] Function { get; set; }
        string Receive { get; set; }
        string Transmit { get; set; }
    }
}
