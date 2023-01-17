using System;
using System.Collections.Generic;
using CoreGraphics;
using Dawn;
using Foundation;
using PDFLibrary;
using UIKit;

namespace PDFGenerator.iOS.Pdf
{
    /// <summary>
    /// This is class is used to get native font and scale it
    /// </summary>
    public sealed class FontCache
    {
        public UIFont GetFont(PdfFont pdfFont)
        {
            Guard.Argument(pdfFont, nameof(pdfFont))
                 .NotNull();

            if (!_fonts.TryGetValue(pdfFont, out var nativeFont))
            {
                nativeFont = CreateNativeFont(pdfFont.FontName, pdfFont.CharWidthMm, pdfFont.LineHeightMm);

                _fonts.Add(pdfFont, nativeFont);
            }

            return nativeFont;
        }

        private static UIFont CreateNativeFont(string fontName, double charWidthMm, double lineHeightMm)
        {
            var sourceFont = UIFont.FromName(fontName, __arbitraryFontSize);
            double fontSizeScaling = __arbitraryFontSize / sourceFont.LineHeight;

            var fontSize = PdfUnitConversion.ConvertMmsToPoints(lineHeightMm) * fontSizeScaling;
            var sizedFont = UIFont.FromName(fontName, (nfloat) fontSize);

            var nativeString = new NSAttributedString(MEASURE_TEXT, sizedFont);
            var characterSize = nativeString.Size;

            var horizontalScaling = PdfUnitConversion.ConvertMmsToPoints(charWidthMm) / characterSize.Width;

            var scaledFont = CreateNativeFont(sizedFont, fontSize, horizontalScaling, 1);

            return scaledFont;
        }

        private static UIFont CreateNativeFont(UIFont sourceFont, double fontSize, double horizontalScaling, double verticalScaling)
        {
            var sourceFontDescriptor = sourceFont.FontDescriptor;

            var scaledFontDescriptor = sourceFontDescriptor.CreateWithMatrix(CGAffineTransform.MakeScale((nfloat) horizontalScaling, (nfloat) verticalScaling));
            var scaledFont = UIFont.FromDescriptor(scaledFontDescriptor, (nfloat) fontSize);

            return scaledFont;
        }

        // Note: the font is assumed to be monospace, so this can be any single character
        private const string MEASURE_TEXT = "0";

        private static readonly nfloat __arbitraryFontSize = (nfloat) 10.0;

        private readonly Dictionary<PdfFont, UIFont> _fonts = new Dictionary<PdfFont, UIFont>();
    }
}