using Dawn;

namespace PDFLibrary
{
    /// <summary>
    /// Represents a font used for building PDFs.
    /// </summary>
    /// <remarks>
    /// Units are in millimeters.
    /// </remarks>
    public sealed class PdfFont
    {
        public PdfFont(string fontName, double charWidthMm, double lineHeightMm)
        {
            FontName = Guard.Argument(fontName, nameof(fontName))
                            .NotNull()
                            .NotWhiteSpace();

            CharWidthMm = Guard.Argument(charWidthMm, nameof(charWidthMm))
                               .Positive();

            LineHeightMm = Guard.Argument(lineHeightMm, nameof(lineHeightMm))
                                .Positive();
        }

        public string FontName { get; }

        public double CharWidthMm { get; }

        public double LineHeightMm { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((PdfFont) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = FontName.GetHashCode();

                hashCode = (hashCode * 397) ^ ComparableValue(CharWidthMm)
                              .GetHashCode();

                hashCode = (hashCode * 397) ^ ComparableValue(LineHeightMm)
                              .GetHashCode();

                return hashCode;
            }
        }

        public static bool operator ==(PdfFont left, PdfFont right) => Equals(left, right);

        public static bool operator !=(PdfFont left, PdfFont right) => !Equals(left, right);

        private bool Equals(PdfFont other) =>
            FontName == other.FontName && ComparableValue(CharWidthMm)
               .Equals(ComparableValue(other.CharWidthMm)) && ComparableValue(LineHeightMm)
               .Equals(ComparableValue(other.LineHeightMm));

        // This method converts a double into a value that encodes significant digits,
        // while discarding insignificant digits far to the right of the decimal point.
        // This allows floating point values that are approximately equal to be compared
        // for equality.
        // ASSUMPTION: the values won't be ridiculously large
        private static int ComparableValue(double value) => (int) (value * 1_000_000);
    }
}