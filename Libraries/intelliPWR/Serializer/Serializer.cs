/*
 * SERIALIZER - 19.07.2018
 * 
 * =============================================================================
 *
 * The MIT License (MIT)
 * 
 * Copyright (c) 2018 Berk Altun - vberkaltun.com
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sub license, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 *
 * =============================================================================
 */

using intelliPWR.Serializer.Structure;
using intelliPWR.Serializer.Interface;

namespace intelliPWR.Serializer
{
    public class Serializer : ISerializer
    {
        #region Variable

        protected SDecoded Decoded;
        protected SEncoded Encoded;

        #endregion

        #region Public

        public string[] Decode(string data, char[] delimiter)
        {
            if (data == null)
                return null;

            if (delimiter == null)
                return null;

            // Clear last stored data
            Decoded.Clear();

            // Best case. All control is ok
            Decoded = new SDecoded(data, delimiter);

            // Return calculated data depending on flag status
            return (Decoded.Decode() ? Decoded.Result : null);
        }

        public string Encode(bool startWithDelimiter, string[] data, char[] delimiter)
        {
            if (data == null)
                return null;

            if (delimiter == null)
                return null;

            // Clear last stored data
            Encoded.Clear();

            // Best case. All control is ok
            Encoded = new SEncoded(startWithDelimiter, data, delimiter);

            // Return calculated data depending on flag status
            return (Encoded.Encode() ? Encoded.Result : null);
        }

        #endregion
    }
}
