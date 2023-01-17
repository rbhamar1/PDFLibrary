using System;
using Dawn;

namespace PDFLibrary
{
    public sealed class ScanlineTextNode : PdfDynamicTextNode
    {
        public ScanlineTextNode(PdfFont font, string prefix, string payTypeCode)
            : base(font)
        {
            _prefix = Guard.Argument(prefix, nameof(prefix))
                           .NotWhiteSpace()
                           .Length(19);

            _payTypeCode = Guard.Argument(payTypeCode, nameof(payTypeCode))
                                .NotWhiteSpace()
                                .Length(2);
        }

        public override string GetText(PdfRenderContext renderContext)
        {
            var adjustedPageNumber = Math.Min(renderContext.PageNumber, 9);
            var pageNumberDigit = adjustedPageNumber.ToString();

            var scanlineData = _prefix + pageNumberDigit;

            var checkDigit = ScanlineCheckDigitCalculator.CalculateCheckDigit(scanlineData);

            var text = $"{scanlineData}{checkDigit}  {_payTypeCode}";

            return text;
        }

        private readonly string _prefix;
        private readonly string _payTypeCode;
    }
}