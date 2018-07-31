namespace intelliPWR.Serializer
{
    public class Function : Core
    {
        #region Protected

        /// <summary>
        /// Default constructor initializer.
        /// </summary>
        protected void Initialize()
        {
            ClearDecodedList(true);
            ClearEncodedList(true);
        }

        /// <summary>
        /// Fill result data of decoded array.
        /// </summary>
        protected void FillDecodedList()
        {
            // Declare and store separated words based on delimiter
            string[] tempofResult = Decoded.Data.Trim(Decoded.Delimiter).Split(Decoded.Delimiter);

            // Declare result data size based on newly calculated size
            Decoded.Result = new string[tempofResult.Length];

            // Clone all of them
            for (int index = 0; index < tempofResult.Length; index++)
                Decoded.Result[index] = tempofResult[index];
        }

        /// <summary>
        /// Fill result data of encoded array.
        /// </summary>
        protected void FillEncodedList()
        {
            // Store the size of received data at the here
            int sizeofAbsolute = Encoded.SizeofDelimiter - Encoded.SizeofData;

            // Declare an variable for storing done separator
            int checkedDelimiter = 0;

            // If absolute value is bigger than 0, add a delimiter to the first index
            if (sizeofAbsolute >= 0 && StartWithDelimiter)
                Encoded.Result += Encoded.Delimiter[checkedDelimiter++];

            for (int array = 0; array < Encoded.SizeofData; array++)
            {
                // Get line by line characters and fill result data
                Encoded.Result += Encoded.Data[array];

                if (checkedDelimiter != Encoded.SizeofDelimiter)
                    Encoded.Result += Encoded.Delimiter[checkedDelimiter++];
            }
        }

        /// <summary>
        /// Main decoder function.
        /// </summary>
        /// <returns></returns>
        protected bool DecodeData()
        {
            // Declare an variable for storing done separator
            int checkedDelimiter = 0;

            for (int index = 0; index < Decoded.SizeofData; index++)
            {
                // Found status flag, using for to find operate
                bool foundFlag = false;

                for (int subIndex = 0; subIndex < Decoded.SizeofDelimiter; subIndex++)
                {
                    if (Decoded.Delimiter[subIndex] == Decoded.Data[index])
                    {
                        foundFlag = true;
                        break;
                    }
                }

                // Is a Delimiter not found in Delimiter list, jump to next
                if (!foundFlag)
                    continue;

                // Check the overflow of counted Delimiter with the real size
                if (checkedDelimiter >= Decoded.SizeofDelimiter)
                    return false;

                // IMPORTANT NOTICE: When we arrive there, we are on the right 
                // Way but we need to check again that the counted Delimiter position 
                // With real Delimiter position. This is important because the real 
                // Order can be different from the calculation order that processed 
                // at the here
                if (Decoded.Delimiter[checkedDelimiter++] != Decoded.Data[index])
                    return false;
            }

            // Cross-check it again, must be equal with together
            if (checkedDelimiter < Decoded.SizeofDelimiter)
                return false;

            // Arrived final function
            FillDecodedList();

            return true;
        }

        /// <summary>
        /// Main encoder function.
        /// </summary>
        /// <returns></returns>
        protected bool EncodeData()
        {
            // Store difference of given data and delimiter data
            int AbsoluteofDifference = Encoded.SizeofData - Encoded.SizeofDelimiter;

            // If calculated data is smaller than -1, calculate absolute value
            if (AbsoluteofDifference < -1)
                AbsoluteofDifference *= -1;

            // IMPORTANT NOTICE: The absolute value always must be 0 or zero
            // For example, If size of given data is bigger or smaller than 
            // the size of delimiters, we can not have enough delimiters for encoding
            // For this reason, When ABS(s) of delimiters and data is 0 or 1,
            // encoding can be performed very well
            if (AbsoluteofDifference > 1)
                return false;

            // Check that whether given data includes a delimiters or not
            for (int array = 0; array < Encoded.SizeofData; array++)
                for (int index = 0; index < Encoded.Data[array].Length; index++)
                    for (int iterator = 0; iterator < Encoded.SizeofDelimiter; iterator++)
                        if (Encoded.Data[array][index] == Encoded.Delimiter[iterator])
                            return false;

            // Arrived final function
            FillEncodedList();

            return true;
        }

        /// <summary>
        /// Clear last stored decoded list on memory.
        /// </summary>
        /// <param name="isAllData">This parameter is related with result data.</param>
        protected void ClearDecodedList(bool isAllData)
        {
            // IMPORTANT NOTICE: seems so confused, right? 
            // When you free up a pointer, you can not use it again anymore 
            // But when you make reassigning NULL to a pointer after the freeing up, 
            // You can this pointer again very well
            Decoded.Delimiter = null;
            Decoded.Data = null;

            if (isAllData)
                Decoded.Result = null;
        }

        /// <summary>
        /// Clear last stored encoded list on memory.
        /// </summary>
        /// <param name="isAllData">This parameter is related with result data.</param>
        protected void ClearEncodedList(bool isAllData)
        {
            // IMPORTANT NOTICE: seems so confused, right? 
            // When you free up a pointer, you can not use it again anymore 
            // But when you make reassigning NULL to a pointer after the freeing up, 
            // You can this pointer again very well
            Encoded.Delimiter = null;
            Encoded.Data = null;

            if (isAllData)
                Encoded.Result = null;
        }

        #endregion

        #region Public

        public string[] Decode(string data, char[] delimiter)
        {
            // Clear last stored data
            ClearDecodedList(true);

            if (data == null)
                return null;

            if (delimiter == null)
                return null;

            // Best case. When we arrive there, that is mean all control is ok
            // And we can start to decoding operation now
            Decoded.Data = data;
            Decoded.Delimiter = delimiter;

            // Store all received data size on lib
            Decoded.SizeofData = data.Length;
            Decoded.SizeofDelimiter = delimiter.Length;

            // -----

            // Decode and store status flag at the here, we will process it
            bool decodeDataFlag = DecodeData();

            if (!decodeDataFlag)
                ClearDecodedList(true);
            else
                ClearDecodedList(false);

            // Return calculated data depending on flag status
            return (decodeDataFlag ? Decoded.Result : null);
        }

        public string Encode(string[] data, char[] Delimiter)
        {
            // Clear last stored data
            ClearEncodedList(true);

            if (data == null)
                return null;

            if (Delimiter == null)
                return null;

            // Best case. When we arrive there, that is mean all control is ok
            // And we can start to encoding operation now
            Encoded.Data = data;
            Encoded.Delimiter = Delimiter;

            // Store all received data size on lib
            Encoded.SizeofData = data.Length;
            Encoded.SizeofDelimiter = Delimiter.Length;

            // -----

            // Encode and store status flag at the here, we will process it
            bool encodeDataFlag = EncodeData();

            if (!encodeDataFlag)
                ClearEncodedList(true);
            else
                ClearEncodedList(false);

            // Return calculated data depending on flag status
            return (encodeDataFlag ? Encoded.Result : null);
        }

        #endregion
    }
}
