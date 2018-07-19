namespace Serializer.Interface
{
    interface IOperations
    {
        /// <summary>
        /// Encode a given data with delimiter list. The most important thing in
        /// this step is the equation of data size and delimiter size.
        /// If the size of both data's is not equal to ±1 of each other, encoding
        /// can not work very well. Output will be null in this situation.
        /// </summary>
        /// <returns>A combined data.</returns>
        string Encode(string[] data, char[] delimiter);

        /// <summary>
        /// Decode a given data with delimiter list. The most important thing in
        /// this step is the equation of data size and delimiter size.
        /// If the size of both data's is not equal to ±1 of each other, encoding
        /// can not work very well. Output will be null in this situation.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="delimiter"></param>
        /// <returns>A separated data.</returns>
        string[] Decode(string data, char[] delimiter);
    }
}