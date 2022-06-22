using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Manh.ILS.Utility.Interfaces;
using System.Diagnostics;

namespace BHS.UWT.BLL
{
    class OldDominionCheckDigitMod10 : ICheckDigitAlgorithm
    {
        public string ComputeCheckDigit(string ProNumber)
        {
            int SumOfEvenDigits;
            string ConcatenationOfOdd;
            string ConcatenationOfOddTimesTwo;
            int SumOfOddDigitsTimesTwo;
            int SumOfDigis;
            int CheckDigit;

            SumOfOddDigitsTimesTwo = 0;
            SumOfEvenDigits = 0;
            ConcatenationOfOdd = "";

            //sum all even digits
            for (int i = 0; i < ProNumber.Length; i = i + 2)
            {
                SumOfEvenDigits += int.Parse(ProNumber[i].ToString());
            }


            for (int i = 1; i < ProNumber.Length; i = i + 2)
            {
                ConcatenationOfOdd += ProNumber[i];
            }

            ConcatenationOfOddTimesTwo = Convert.ToString(2 * Convert.ToInt32(ConcatenationOfOdd));

            for (int i = 0; i < ConcatenationOfOddTimesTwo.Length; i++)
            {
                SumOfOddDigitsTimesTwo += int.Parse(ConcatenationOfOddTimesTwo[i].ToString());
            }

            SumOfDigis = SumOfEvenDigits + SumOfOddDigitsTimesTwo;


            CheckDigit = (Convert.ToInt32(Math.Ceiling(SumOfDigis / 10.0)) * 10 - SumOfDigis) % 10;

            Debug.WriteLine("BHS.UWT.BLL.ComputeCheckDigit.SumOfEvenDigits = " + SumOfEvenDigits);
            Debug.WriteLine("BHS.UWT.BLL.ComputeCheckDigit.concatenationOfOdd = " + ConcatenationOfOdd);
            Debug.WriteLine("BHS.UWT.BLL.ComputeCheckDigit.concatenationOfOddTimesTWo = " + ConcatenationOfOddTimesTwo);
            Debug.WriteLine("BHS.UWT.BLL.ComputeCheckDigit.SumOfOddDigitsTimesTwo = " + SumOfOddDigitsTimesTwo);
            Debug.WriteLine("BHS.UWT.BLL.ComputeCheckDigit.SumOfDigis = " + SumOfDigis);
            Debug.WriteLine("BHS.UWT.BLL.ComputeCheckDigit.CheckDigit = " + CheckDigit);

            return (ProNumber+CheckDigit.ToString());
        }
    }
}
