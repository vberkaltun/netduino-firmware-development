namespace intelliPWR.Serializer
{
    public interface ISerializer
    {
        string[] Decode(char[] delimiter, string data);
        string Encode(bool startWithDelimiter, char[] delimiter, string[] data);
    }
}