namespace intelliPWR.MasterScanner
{
    /// <summary>
    /// Data's field.
    /// </summary>
    interface ISlave
    {
        byte StartAddress { get; set; }
        byte StopAddress { get; set; }

        bool[] ConnectedSlavesArray { get; set; }
        byte ConnectedSlavesCount { get; set; }
    }
}
