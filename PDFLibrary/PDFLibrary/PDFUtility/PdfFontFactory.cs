using Dawn;

namespace PDFLibrary
{
    public static class PdfFontFactory
    {
        public static PdfFont CreateSmallRegularFont() => CreateRegularFont(SMALL_REGULAR_FONT_CHAR_WIDTH_MM, SMALL_FONT_LINE_HEIGHT_MM);

        public static PdfFont CreateSmallBoldFont() => CreateBoldFont(SMALL_DARKER_FONT_CHAR_WIDTH_MM, SMALL_FONT_LINE_HEIGHT_MM);

        public static PdfFont CreateLargeBoldFont() => CreateBoldFont(LARGE_FONT_CHAR_WIDTH_MM, LARGE_FONT_LINE_HEIGHT_MM);

        public static PdfFont CreateLargeRegularFont() => CreateRegularFont(SMALL_REGULAR_FONT_CHAR_WIDTH_MM, LARGE_FONT_LINE_HEIGHT_MM);

        public static PdfFont CreateScaledRegularFont(double horizontalScale, double verticalScale)
        {
            Guard.Argument(horizontalScale, nameof(horizontalScale))
                 .Positive();

            Guard.Argument(verticalScale, nameof(verticalScale))
                 .Positive();

            return CreateRegularFont(SMALL_REGULAR_FONT_CHAR_WIDTH_MM * horizontalScale,
                                     SMALL_FONT_LINE_HEIGHT_MM * verticalScale);
        }

        public static PdfFont CreateScaledBoldFont(double horizontalScale, double verticalScale)
        {
            Guard.Argument(horizontalScale, nameof(horizontalScale))
                 .Positive();

            Guard.Argument(verticalScale, nameof(verticalScale))
                 .Positive();

            return CreateBoldFont(SMALL_DARKER_FONT_CHAR_WIDTH_MM * horizontalScale, SMALL_FONT_LINE_HEIGHT_MM * verticalScale);
        }

        public static PdfFont CreateRegularFont(double charWidthMm, double lineHeightMm) => new PdfFont(REGULAR_FONT_NAME, charWidthMm, lineHeightMm);

        public static PdfFont CreateOCRRegularFont(double charWidthMm, double lineHeightMm) => new PdfFont(OCR_REGULAR_FONT_NAME, charWidthMm, lineHeightMm);

        public static PdfFont CreateBoldFont(double widthMm, double lineHeightMm) => new PdfFont(DARKER_FONT_NAME, widthMm, lineHeightMm);

        public static double SmallFontHeightMm => SMALL_FONT_LINE_HEIGHT_MM;

        public static double LargeFontHeightMm => LARGE_FONT_LINE_HEIGHT_MM;

        // Note: despite the name, this is the "regular" font
        private const string REGULAR_FONT_NAME = "Custom-01-Letter-Gothic-Bold";

        // Note: this is the "bold" font
        private const string DARKER_FONT_NAME = "Custom-01-Letter-Gothic-Heavy";

        private const string OCR_REGULAR_FONT_NAME = "OCR A Std";

        private const double SMALL_REGULAR_FONT_CHAR_WIDTH_MM = 1.25;

        private const double SMALL_DARKER_FONT_CHAR_WIDTH_MM = SMALL_REGULAR_FONT_CHAR_WIDTH_MM * 1.25;

        private const double SMALL_FONT_LINE_HEIGHT_MM = 3.22;

        private const double LARGE_FONT_SCALING = 1.2;

        private const double LARGE_FONT_CHAR_WIDTH_MM = SMALL_REGULAR_FONT_CHAR_WIDTH_MM * LARGE_FONT_SCALING;
        private const double LARGE_FONT_LINE_HEIGHT_MM = SMALL_FONT_LINE_HEIGHT_MM * LARGE_FONT_SCALING;
    }
}