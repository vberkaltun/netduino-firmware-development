namespace Serializer.Interface
{
    /// <summary>
    /// Encoded data's field.
    /// </summary>
    interface IEncodedData
    {
        int SizeofDelimiter { get; set; }
        int SizeofGivenData { get; set; }

        char[] Delimiter { get; set; }
        string[] GivenData { get; set; }
        string ResultData { get; set; }
    }
}
