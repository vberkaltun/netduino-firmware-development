namespace intelliPWR.MasterScanner.Interface
{
    interface ISlave
    {
        bool CheckRange(byte startAddress, byte stopAddress);
        void CleanRange(byte startAddress, byte stopAddress);
        bool IsConnected(byte address);
        void ResetRange();
        bool SetRange(byte startAddress, byte stopAddress);
    }
}
