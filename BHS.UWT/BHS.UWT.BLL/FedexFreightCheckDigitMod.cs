using System;
using Manh.ILS.Utility.Interfaces;

namespace SCALE.UWT
{
    class FedexFreightCheckDigitMod : ICheckDigitAlgorithm
	{
		public string ComputeCheckDigit(string ProNumber)
		{
			try
			{
				int ProNumberInt = Convert.ToInt32(ProNumber);
				int ProNumberOverSeven = Convert.ToInt32(ProNumber) / 7;
				return ProNumber + (ProNumberInt - ProNumberOverSeven * 7).ToString();
			}
			catch
			{
				return ProNumber;
			}
		}
	}
}
