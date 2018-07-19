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

using Serializer.Interface;
using Serializer.Struct;
using System;

namespace Serializer
{
    public static class Serializer
    {
        #region Decoded and Encoded Data

        static DecodedData DecodedList = new DecodedData();
        static EncodedData EncodedList = new EncodedData();

        #endregion

        #region Public Operations

        public static string[] Decode(string data, char[] delimiter)
        {
            // Clear last stored data
            ClearDecodedList(true);

            if (data == null)
                return null;

            if (delimiter == null)
                return null;

            // Best case. When we arrive there, that is mean all control is ok
            // And we can start to decoding operation now
            DecodedList.GivenData = data;
            DecodedList.Delimiter = delimiter;

            // Store all received data size on lib
            DecodedList.SizeofGivenData = data.Length;
            DecodedList.SizeofDelimiter = delimiter.Length;

            // -----

            // Decode and store status flag at the here, we will process it
            bool decodeDataFlag = DecodeData();

            if (!decodeDataFlag)
                ClearDecodedList(true);
            else
                ClearDecodedList(false);

            // Return calculated data depending on flag status
            return (decodeDataFlag ? DecodedList.ResultData : null);
        }

        public static string Encode(string[] data, char[] Delimiter)
        {
            // Clear last stored data
            ClearEncodedList(true);

            if (data == null)
                return null;

            if (Delimiter == null)
                return null;

            // Best case. When we arrive there, that is mean all control is ok
            // And we can start to encoding operation now
            EncodedList.GivenData = data;
            EncodedList.Delimiter = Delimiter;

            // Store all received data size on lib
            EncodedList.SizeofGivenData = data.Length;
            EncodedList.SizeofDelimiter = Delimiter.Length;

            // -----

            // Encode and store status flag at the here, we will process it
            bool encodeDataFlag = EncodeData();

            if (!encodeDataFlag)
                ClearEncodedList(true);
            else
                ClearEncodedList(false);

            // Return calculated data depending on flag status
            return (encodeDataFlag ? EncodedList.ResultData : null);
        }

        #endregion

        #region Private Operations

        private static void ClearDecodedList(bool isAllData)
        {
            // IMPORTANT NOTICE: seems so confused, right? 
            // When you free up a pointer, you can not use it again anymore 
            // But when you make reassigning NULL to a pointer after the freeing up, 
            // You can this pointer again very well
            DecodedList.Delimiter = null;
            DecodedList.GivenData = null;

            if (isAllData)
                DecodedList.ResultData = null;
        }

        private static void ClearEncodedList(bool isAllData)
        {
            // IMPORTANT NOTICE: seems so confused, right? 
            // When you free up a pointer, you can not use it again anymore 
            // But when you make reassigning NULL to a pointer after the freeing up, 
            // You can this pointer again very well
            EncodedList.Delimiter = null;
            EncodedList.GivenData = null;

            if (isAllData)
                EncodedList.ResultData = null;
        }

        private static void FillDecodedList()
        {
            // Declare and store separated words based on delimiter
            string[] tempofResultData = DecodedList.GivenData.Split(DecodedList.Delimiter);

            // Declare result data size based on newly calculated size
            DecodedList.ResultData = new string[tempofResultData.Length];

            // Clone all of them
            for (int index = 0; index < tempofResultData.Length; index++)
                DecodedList.ResultData[index] = tempofResultData[index];
        }

        private static void FillEncodedList()
        {
            // Store the size of received data at the here
            int sizeofAbsolute = EncodedList.SizeofDelimiter - EncodedList.SizeofGivenData;

            // Declare an variable for storing done separator
            int checkedDelimiter = 0;

            // If absolute value is bigger than 0, add a delimiter to the first index
            if (sizeofAbsolute >= 0)
                EncodedList.ResultData += EncodedList.Delimiter[checkedDelimiter++];

            for (int array = 0; array < EncodedList.SizeofGivenData; array++)
            {
                // Get line by line characters and fill result data
                EncodedList.ResultData += EncodedList.GivenData[array];

                if (checkedDelimiter != EncodedList.SizeofDelimiter)
                    EncodedList.ResultData += EncodedList.Delimiter[checkedDelimiter++];
            }
        }

        private static bool DecodeData()
        {
            // Declare an variable for storing done separator
            int checkedDelimiter = 0;

            for (int index = 0; index < DecodedList.SizeofGivenData; index++)
            {
                // Found status flag, using for to find operate
                bool foundFlag = false;

                for (int subIndex = 0; subIndex < DecodedList.SizeofDelimiter; subIndex++)
                {
                    if (DecodedList.Delimiter[subIndex] == DecodedList.GivenData[index])
                    {
                        foundFlag = true;
                        break;
                    }
                }

                // Is a Delimiter not found in Delimiter list, jump to next
                if (!foundFlag)
                    continue;

                // Check the overflow of counted Delimiter with the real size
                if (checkedDelimiter >= DecodedList.SizeofDelimiter)
                    return false;

                // IMPORTANT NOTICE: When we arrive there, we are on the right 
                // Way but we need to check again that the counted Delimiter position 
                // With real Delimiter position. This is important because the real 
                // Order can be different from the calculation order that processed 
                // at the here
                if (DecodedList.Delimiter[checkedDelimiter++] != DecodedList.Delimiter[index])
                    return false;
            }

            // Cross-check it again, must be equal with together
            if (checkedDelimiter < DecodedList.SizeofDelimiter)
                return false;

            // Arrived final function
            FillDecodedList();

            return true;
        }

        private static bool EncodeData()
        {
            // IMPORTANT NOTICE: The absolute value always must be 0 or zero
            // For example, If size of given data is bigger or smaller than 
            // the size of delimiters, we can not have enough delimiters for encoding
            // For this reason, When ABS(s) of delimiters and data is 0 or 1,
            // encoding can be performed very well
            if (Math.Abs(EncodedList.SizeofGivenData - EncodedList.SizeofDelimiter) > 1)
                return false;

            // Check that whether given data includes a delimiters or not
            for (int array = 0; array < EncodedList.SizeofGivenData; array++)
                for (int index = 0; index < EncodedList.GivenData[array].Length; index++)
                    for (int iterator = 0; iterator < EncodedList.SizeofDelimiter; iterator++)
                        if (EncodedList.GivenData[array][index] == EncodedList.Delimiter[iterator])
                            return false;

            // Arrived final function
            FillEncodedList();

            return true;
        }

        #endregion
    }
}