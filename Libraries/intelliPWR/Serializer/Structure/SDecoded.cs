namespace intelliPWR.Serializer
{
    public class SDecoded : IDecoded
    {
        private char[] delimiter;
        public char[] Delimiter
        {
            get
            {
                return delimiter;
            }

            set
            {
                delimiter = value;
            }
        }

        private string data;
        public string Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
            }
        }

        private string[] result;
        public string[] Result
        {
            get
            {
                return result;
            }

            set
            {
                result = value;
            }
        }

        private int sizeofDelimiter;
        public int SizeofDelimiter
        {
            get
            {
                return sizeofDelimiter;
            }

            set
            {
                sizeofDelimiter = value;
            }
        }

        private int sizeofData;
        public int SizeofData
        {
            get
            {
                return sizeofData;
            }

            set
            {
                sizeofData = value;
            }
        }

        #region Constructor

        public SDecoded() { }

        public SDecoded(char[] delimiter, string data)
        {
            // Best case. When we arrive there, that is mean all control is ok
            // And we can start to encoding operation now
            Data = data;
            Delimiter = delimiter;

            // Store all received data size on lib
            SizeofData = data.Length;
            SizeofDelimiter = delimiter.Length;
        }

        #endregion

        #region Function

        public void Clear()
        {
            Delimiter = null;
            Data = null;
            Result = null;
        }

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

        #endregion
    }
}
