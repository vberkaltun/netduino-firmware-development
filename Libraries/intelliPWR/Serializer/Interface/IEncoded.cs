namespace intelliPWR.Serializer
{
    public interface IEncoded
    {
        string[] Data { get; set; }
        char[] Delimiter { get; set; }
        string Result { get; set; }
        int SizeofData { get; set; }
        int SizeofDelimiter { get; set; }
        bool StartWithDelimiter { get; set; }

        void Clear();
        bool Encode();
        void Fill();
    }
}