using intelliPWR.Serializer.Interface;

namespace intelliPWR.Serializer.Structure
{
    public class SEncoded : IEncoded
    {
        #region Variable

        public bool StartWithDelimiter;
        public char[] Delimiter;
        public string[] Data;
        public string Result;
        public int SizeofDelimiter;
        public int SizeofData;

        #endregion

        #region Constructor

        public SEncoded(string[] data, char[] delimiter)
        {
            Setup(true, data, delimiter);
        }

        public SEncoded(bool startWithDelimiter, string[] data, char[] delimiter)
        {
            Setup(startWithDelimiter, data, delimiter);
        }

        protected void Setup(bool startWithDelimiter, string[] data, char[] delimiter)
        {
            StartWithDelimiter = startWithDelimiter;

            // Best case. When we arrive there, that is mean all control is ok
            // And we can start to decoding operation now
            Data = data;
            Delimiter = delimiter;

            // Store all received data size on lib
            SizeofData = data.Length;
            SizeofDelimiter = delimiter.Length;
        }

        #endregion

        #region Function

        /// <summary>
        /// Fill result data of encoded array.
        /// </summary>
        public void Fill()
        {
            // Store the size of received data at the here
            int sizeofAbsolute = SizeofDelimiter - SizeofData;

            // Declare an variable for storing done separator
            int checkedDelimiter = 0;

            // If absolute value is bigger than 0, add a delimiter to the first index
            if (sizeofAbsolute >= 0 && StartWithDelimiter)
                Result += Delimiter[checkedDelimiter++];

            for (int array = 0; array < SizeofData; array++)
            {
                // Get line by line characters and fill result data
                Result += Data[array];

                if (checkedDelimiter != SizeofDelimiter)
                    Result += Delimiter[checkedDelimiter++];
            }
        }

        /// <summary>
        /// Main encoder function.
        /// </summary>
        /// <returns></returns>
        public bool Encode()
        {
            // Store difference of given data and delimiter data
            int AbsoluteofDifference = SizeofData - SizeofDelimiter;

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
            for (int array = 0; array < SizeofData; array++)
                for (int index = 0; index < Data[array].Length; index++)
                    for (int iterator = 0; iterator < SizeofDelimiter; iterator++)
                        if (Data[array][index] == Delimiter[iterator])
                            return false;

            // Arrived final function
            Fill();

            return true;
        }

        /// <summary>
        /// Clear last stored encoded list on memory.
        /// </summary>
        /// <param name="isAllData">This parameter is related with result data.</param>
        public void Clear()
        {
            // IMPORTANT NOTICE: seems so confused, right? 
            // When you free up a pointer, you can not use it again anymore 
            // But when you make reassigning NULL to a pointer after the freeing up, 
            // You can this pointer again very well
            Delimiter = null;
            Data = null;
            Result = null;
        }

        #endregion
    }
}
