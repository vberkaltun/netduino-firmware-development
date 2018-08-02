namespace intelliPWR.Serializer
{
    /// <summary>
    /// Decoded data's field.
    /// </summary>
    interface IDecoded
    {
        int SizeofDelimiter { get; set; }
        int SizeofData { get; set; }

        char[] Delimiter { get; set; }
        string Data { get; set; }
        string[] Result { get; set; }
    }
}
