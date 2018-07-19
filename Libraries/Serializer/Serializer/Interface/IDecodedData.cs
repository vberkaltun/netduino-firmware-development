namespace Serializer.Interface
{
    /// <summary>
    /// Decoded data's field.
    /// </summary>
    interface IDecodedData
    {
        int SizeofDelimiter { get; set; }
        int SizeofGivenData { get; set; }

        char[] Delimiter { get; set; }
        string GivenData { get; set; }
        string[] ResultData { get; set; }
    }
}
