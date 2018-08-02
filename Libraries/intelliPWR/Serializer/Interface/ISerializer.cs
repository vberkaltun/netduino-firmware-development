namespace intelliPWR.Serializer.Interface
{
    public interface ISerializer
    {
        string[] Decode(string data, char[] delimiter);
        string Encode(bool startWithDelimiter, string[] data, char[] delimiter);
    }
}