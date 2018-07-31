using System;

namespace intelliPWR.Serializer
{
    public class Core
    {
        #region Encapsulation

        private bool startWithDelimiter;
        protected bool StartWithDelimiter
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

        #endregion

        #region Structure

        protected struct SDecoded : IDecoded
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
        }

        protected struct SEncoded : IEncoded
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

            private string[] data;
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

            private string result;
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
        }

        #endregion

        #region Variable

        protected SDecoded Decoded = new SDecoded();
        protected SEncoded Encoded = new SEncoded();

        #endregion
    };
}
