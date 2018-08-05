namespace intelliPWR.MasterScanner
{
    public interface IMasterScanner
    {
        void ScanSlaves(Microsoft.SPOT.Hardware.I2CDevice Device);
        bool SetRange(byte startAddress, byte stopAddress);
        void ResetRange();
        bool IsConnected(byte address);
    }
}