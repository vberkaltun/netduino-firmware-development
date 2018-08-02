namespace intelliPWR.Serializer
{
    public interface IDecoded
    {
        string Data { get; set; }
        char[] Delimiter { get; set; }
        string[] Result { get; set; }
        int SizeofData { get; set; }
        int SizeofDelimiter { get; set; }

        void Clear();
        bool Decode();
        void Fill();
    }
}