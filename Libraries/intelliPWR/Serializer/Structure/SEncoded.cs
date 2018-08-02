namespace intelliPWR.Serializer
{
    public class SEncoded : IEncoded
    {
        private bool startWithDelimiter = false;
        public bool StartWithDelimiter
        {
            get
            {
                return startWithDelimiter;
            }

            set
            {
                startWithDelimiter = value;
            }
        }

        private char[] delimiter = null;
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

        private string[] data = null;
        public string[] Data
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

        private string result = null;
        public string Result
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

        private int sizeofDelimiter = 0;
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

        private int sizeofData = 0;
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

        public SEncoded() { }

        public SEncoded(bool startWithDelimiter, char[] delimiter, string[] data)
        {
            // Best case. When we arrive there, that is mean all control is ok
            // And we can start to encoding operation now
            StartWithDelimiter = startWithDelimiter;
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

        #endregion
    }
}
