using System;
using intelliPWR.Interface;

namespace intelliPWR.Struct
{
    struct EncodedData : IEncodedData
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

        private string[] givenData;
        public string[] GivenData
        {
            get
            {
                return givenData;
            }

            set
            {
                givenData = value;
            }
        }

        private string resultData;
        public string ResultData
        {
            get
            {
                return resultData;
            }

            set
            {
                resultData = value;
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

        private int sizeofGivenData;
        public int SizeofGivenData
        {
            get
            {
                return sizeofGivenData;
            }

            set
            {
                sizeofGivenData = value;
            }
        }
    };
}
