namespace intelliPWR.Serializer
{
    public interface ISerializer
    {
        string[] Decode(string data, char[] delimiter);
        string Encode(string[] data, char[] Delimiter);
    }
}