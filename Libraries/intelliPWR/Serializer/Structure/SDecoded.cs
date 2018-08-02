using intelliPWR.Serializer.Interface;

namespace intelliPWR.Serializer.Structure
{
    public class SDecoded : IDecoded
    {
        #region Variable

        public char[] Delimiter;
        public string Data;
        public string[] Result;
        public int SizeofDelimiter;
        public int SizeofData;

        #endregion

        #region Constructor

        public SDecoded(string data, char[] delimiter)
        {
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
        /// Fill result data of decoded array.
        /// </summary>
        public void Fill()
        {
            // Declare and store separated words based on delimiter
            string[] tempofResult = Data.Trim(Delimiter).Split(Delimiter);

            // Declare result data size based on newly calculated size
            Result = new string[tempofResult.Length];

            // Clone all of them
            for (int index = 0; index < tempofResult.Length; index++)
                Result[index] = tempofResult[index];
        }

        /// <summary>
        /// Main decoder function.
        /// </summary>
        /// <returns></returns>
        public bool Decode()
        {
            // Declare an variable for storing done separator
            int checkedDelimiter = 0;

            for (int index = 0; index < SizeofData; index++)
            {
                // Found status flag, using for to find operate
                bool foundFlag = false;

                for (int subIndex = 0; subIndex < SizeofDelimiter; subIndex++)
                {
                    if (Delimiter[subIndex] == Data[index])
                    {
                        foundFlag = true;
                        break;
                    }
                }

                // Is a Delimiter not found in Delimiter list, jump to next
                if (!foundFlag)
                    continue;

                // Check the overflow of counted Delimiter with the real size
                if (checkedDelimiter >= SizeofDelimiter)
                    return false;

                // IMPORTANT NOTICE: When we arrive there, we are on the right 
                // Way but we need to check again that the counted Delimiter position 
                // With real Delimiter position. This is important because the real 
                // Order can be different from the calculation order that processed 
                // at the here
                if (Delimiter[checkedDelimiter++] != Data[index])
                    return false;
            }

            // Cross-check it again, must be equal with together
            if (checkedDelimiter < SizeofDelimiter)
                return false;

            // Arrived final function
            Fill();

            return true;
        }

        /// <summary>
        /// Clear last stored decoded list on memory.
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
