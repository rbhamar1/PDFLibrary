namespace PDFLibrary
{
    public static class PdfUnitConversion
    {
        public static double ConvertMmsToPoints(double mm) => ConvertInchesToPoints(mm / MM_PER_INCH);

        public static double ConvertPointsToMms(double points) => points / POINTS_PER_INCH * MM_PER_INCH;

        private static double ConvertInchesToPoints(double inches) => inches * POINTS_PER_INCH;

        private const double MM_PER_INCH = 25.4;

        private const double POINTS_PER_INCH = 72;
    }
}