namespace intelliPWR.Serializer
{
    public class Core : IDecoded, IEncoded
    {
        #region Encapsulation

        private bool startWithDelimiter;
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

        // -----

        private char[] decodedDelimiter;
        public char[] DecodedDelimiter
        {
            get
            {
                return decodedDelimiter;
            }

            set
            {
                decodedDelimiter = value;
            }
        }

        private string decodedData;
        public string DecodedData
        {
            get
            {
                return decodedData;
            }

            set
            {
                decodedData = value;
            }
        }

        private string[] decodedResult;
        public string[] DecodedResult
        {
            get
            {
                return decodedResult;
            }

            set
            {
                decodedResult = value;
            }
        }

        private int sizeofDecodedDelimiter;
        public int SizeofDecodedDelimiter
        {
            get
            {
                return sizeofDecodedDelimiter;
            }

            set
            {
                sizeofDecodedDelimiter = value;
            }
        }

        private int sizeofDecodedData;
        public int SizeofDecodedData
        {
            get
            {
                return sizeofDecodedData;
            }

            set
            {
                sizeofDecodedData = value;
            }
        }

        // -----

        private char[] encodedDelimiter;
        public char[] EncodedDelimiter
        {
            get
            {
                return encodedDelimiter;
            }

            set
            {
                encodedDelimiter = value;
            }
        }

        private string[] encodedData;
        public string[] EncodedData
        {
            get
            {
                return encodedData;
            }

            set
            {
                encodedData = value;
            }
        }

        private string encodedResult;
        public string EncodedResult
        {
            get
            {
                return encodedResult;
            }

            set
            {
                encodedResult = value;
            }
        }

        private int sizeofEncodedDelimiter;
        public int SizeofEncodedDelimiter
        {
            get
            {
                return sizeofEncodedDelimiter;
            }

            set
            {
                sizeofEncodedDelimiter = value;
            }
        }

        private int sizeofEncodedData;
        public int SizeofEncodedData
        {
            get
            {
                return sizeofEncodedData;
            }

            set
            {
                sizeofEncodedData = value;
            }
        }

        #endregion
    };
}
