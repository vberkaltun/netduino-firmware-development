namespace intelliPWR.MasterScanner
{
    /// <summary>
    /// Config's field.
    /// </summary>
    interface IConfig
    {
        ushort ClockSpeed { get; set; }
        ushort Timeout { get; set; }
        ushort RetryCount { get; set; }
    }
}
