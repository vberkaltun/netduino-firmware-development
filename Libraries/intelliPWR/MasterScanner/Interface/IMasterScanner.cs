namespace intelliPWR.MasterScanner
{
    public interface IMasterScanner
    {
        void ScanSlaves();
        bool SetRange(byte startAddress, byte stopAddress);
        void ResetRange();
        bool IsConnected(byte address);
    }
}