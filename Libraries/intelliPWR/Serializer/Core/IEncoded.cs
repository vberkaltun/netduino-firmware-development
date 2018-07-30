namespace intelliPWR.Serializer
{
    /// <summary>
    /// Encoded data's field.
    /// </summary>
    interface IEncoded
    {
        int SizeofEncodedDelimiter { get; set; }
        int SizeofEncodedData { get; set; }

        char[] EncodedDelimiter { get; set; }
        string[] EncodedData { get; set; }
        string EncodedResult { get; set; }
    }
}
