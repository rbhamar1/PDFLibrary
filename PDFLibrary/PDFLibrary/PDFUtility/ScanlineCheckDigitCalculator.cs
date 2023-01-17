namespace PDFLibrary
{
    public static class ScanlineCheckDigitCalculator
    {
        // Note: this is just a modernized version of the code in HH4
        public static string CalculateCheckDigit(string scanlinePrefix)
        {
            var scanlineSum = 0;

            for (var index = 0; index < scanlinePrefix.Length; index++)
            {
                var digit = scanlinePrefix[index] - '0';

                var multiplierIndex = index % __multiplierArray.Length;

                scanlineSum += digit * __multiplierArray[multiplierIndex];
            }

            scanlineSum %= CHECK_DIGIT_CONSTANT;

            scanlineSum = CHECK_DIGIT_CONSTANT - scanlineSum;

            var resultString = scanlineSum switch
                {
                    11 => "0",
                    10 => "X",
                    _ => scanlineSum.ToString()
                };

            return resultString;
        }

        private const int CHECK_DIGIT_CONSTANT = 11;

        private static readonly int[] __multiplierArray = {7, 6, 5, 4, 3, 2};
    }
}