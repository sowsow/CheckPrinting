using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckPrinting
{
    public static class Utilities
    {

        #region Init Number-Text Dictionary
        private static string[] dicOneDigit =
        {
            "ZERO",
            "ONE",
            "TWO",
            "THREE",
            "FOUR",
            "FIVE",
            "SIX",
            "SEVEN",
            "EIGHT",
            "NINE"
        };

        private static string[] dicTwoDigits_1X =
        {
            "TEN",
            "ELEVEN",
            "TWELVE",
            "THIRTEEN",
            "FORTEEN",
            "FIFTEEN",
            "SIXTEEN",
            "SEVENTEEN",
            "EIGHTEEN",
            "NINETEEN"
        };

        private static string[] dicTwoDigits_2X = 
        {
            "TWENTY",
            "THIRTY",
            "FORTY",
            "FIFTY",
            "SIXTY",
            "SEVENTY",
            "EIGHTY",
            "NINETY",
        };

        private static string[] dicCommaUnit =
        {
            "",
            "THOUSAND",
            "MILLION",
            "BILLION",
            "TRILLION",
            "QUADRILLION",
            "QUINTILLION",
        };
        #endregion Init Number-Text Dictionary


        /// <summary>
        /// Convert number to printing text
        /// </summary>
        /// <param name="number">Input decimal number</param>
        /// <returns>Printing text in uppercase</returns>
        public static string ConvertNumberToPrinting(decimal number)
        {
            StringBuilder strBuilder = new StringBuilder();

            if (number == 0)
            {
                strBuilder.Append(dicOneDigit[0] + " DOLLAR");
                return strBuilder.ToString();
            }

            // Convert input number into string of 2 decimals 
            string strNum = Math.Round(number, 2).ToString();

            // Split input into left & right by decimal dot
            string strLeft = string.Empty;
            string strRight = string.Empty;
            if (strNum.Contains('.'))
            {
                strLeft = strNum.Split('.')[0];    // Retrieve left handside of dot                
                strRight = "0." + strNum.Split('.')[1];    // Retrieve right handside of dot
            }
            else
            {
                strLeft = strNum;
            }

            // Handle Left handside of dot      
            int leftLength = strLeft.Length;

            // Identify the biggest unit of left handside
            int div = leftLength / 3;
            int mod = leftLength % 3;
            int biggestCommaUnit = div;

            if (div != 0 && mod == 0)
                biggestCommaUnit = div - 1;

            // Segment left handside by every 3 digits
            StringBuilder segStrBuilder = new StringBuilder();
            segStrBuilder.Append(strLeft.Substring(0, mod) + ",");

            if ( 0 <= strLeft.Length && strLeft.Length <= 3)    // e.g. 999.0, 99.0 or 9.0
            {                
                strBuilder.Append(ConvertSegmentToText(strLeft));
            }
            else  // At least have 4 digits
            {
                for (int i = mod; i <= strLeft.Length - 1; i += 3)
                {
                    segStrBuilder.Append(strLeft.Substring(i, 3) + ",");
                }

                // e.g. 1, 234, 567, 890 ---> ["1", "234", "567", "890"]
                string[] segments = segStrBuilder.ToString().Trim(',').Split(',');

                // Append comma unit by biggestCommaUnit--
                for (int j = 0; j < segments.Length; j++)
                {
                    strBuilder.AppendFormat("{0}", ConvertSegmentToText(segments[j]));

                    if (biggestCommaUnit >= 0)
                        strBuilder.AppendFormat("{0} ", dicCommaUnit[biggestCommaUnit--]);
                }
            }

            if (Int64.Parse(strLeft) == 1)
                strBuilder.Append("DOLLAR ");
            else if (Int64.Parse(strLeft) > 1)
                strBuilder.Append("DOLLARS ");

            // Process right handside of dot
            if (!string.IsNullOrEmpty(strRight))
            {
                int numRight = (int)(Decimal.Parse(strRight) * 100);
                if (numRight != 0) // if decimal digits is not 0
                {
                    if (strBuilder.Length > 0)
                        strBuilder.Append(" AND ");

                    // Call segment conversion
                    strBuilder.Append(ConvertSegmentToText(numRight.ToString()));

                    // Append currency unit
                    if (numRight == 1)
                        strBuilder.Append("CENT");
                    else
                        strBuilder.Append("CENTS");
                }
            }


            return strBuilder.ToString();
        }


        /// <summary>
        /// Convert 3-digt segment to printing text
        /// </summary>
        /// <param name="segment">3-digt segment</param>
        /// <returns>Printing text in uppercase</returns>
        private static string ConvertSegmentToText(string segment)
        {
            if (string.IsNullOrEmpty(segment))
                return string.Empty;

            StringBuilder strBuilder = new StringBuilder();
            int index = 0;  // Index of priting text dictionary
            char[] segArr = segment.ToArray();  // char array of each glyph

            switch (segment.Length)
            {
                case 1:  // // e.g. 1 to 9
                    index = (int)char.GetNumericValue(segArr[0]);
                    if (index != 0)
                        strBuilder.AppendFormat("{0} ", dicOneDigit[index]);
                    break;  // End case 1

                case 2:
                    if (segArr[0].Equals('1'))  // e.g. 11 to 19
                    {
                        index = Int32.Parse(segment) - 10;
                        strBuilder.AppendFormat("{0} ", dicTwoDigits_1X[index]);
                    }
                    else  // e.g. 2X to 9X
                    {
                        index = (int)char.GetNumericValue(segArr[0]) - 2;
                        int index2 = (int)char.GetNumericValue(segArr[1]);
                        strBuilder.AppendFormat("{0} ", dicTwoDigits_2X[index]);
                        if (index2 !=0 )
                            strBuilder.AppendFormat("{0} ", dicOneDigit[index2]);
                    }
                    break;   // End case 2

                case 3:
                    index = (int)char.GetNumericValue(segArr[0]);
                    strBuilder.AppendFormat("{0} HUNDERED ", dicOneDigit[index]);
                    string twoDigt = ConvertSegmentToText(new string(segArr, 1, 2));

                    if (!string.IsNullOrEmpty(twoDigt))
                        strBuilder.AppendFormat("AND {0}", twoDigt);
                    break; // End case 3

                default:
                    return string.Empty;
            }   // // End switch (segment.Length)

            return strBuilder.ToString();
        }

    }
}
