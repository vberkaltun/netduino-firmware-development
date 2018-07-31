namespace intelliPWR.Serializer
{
    /// <summary>
    /// Encoded data's field.
    /// </summary>
    interface IEncoded
    {
        int SizeofDelimiter { get; set; }
        int SizeofData { get; set; }

        char[] Delimiter { get; set; }
        string[] Data { get; set; }
        string Result { get; set; }
    }
}
