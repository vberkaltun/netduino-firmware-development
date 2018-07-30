namespace intelliPWR.Serializer
{
    /// <summary>
    /// Decoded data's field.
    /// </summary>
    interface IDecoded
    {
        int SizeofDecodedDelimiter { get; set; }
        int SizeofDecodedData { get; set; }

        char[] DecodedDelimiter { get; set; }
        string DecodedData { get; set; }
        string[] DecodedResult { get; set; }
    }
}
