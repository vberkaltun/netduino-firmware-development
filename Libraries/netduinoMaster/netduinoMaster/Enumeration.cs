namespace netduinoMaster
{
    public enum EHandshake { Unknown, Ready };
    public enum ECommunication { Idle, Continue, End };
    public enum ENotify { Offline, Online, Unconfirmed, Confirmed };
    public enum ETimer { Ping, Scan, RGB };
}
