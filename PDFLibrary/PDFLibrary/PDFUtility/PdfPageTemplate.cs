using Dawn;
using Xamarin.Forms;

namespace PDFLibrary
{
    /// <summary>
    /// Describes the maximum size and the margins for a PDF page.
    /// </summary>
    public sealed class PdfPageTemplate
    {
        public PdfPageTemplate()
            : this(__pageSize, __defaultPageMargins)
        {
        }

        public PdfPageTemplate(Thickness marginsMm)
            : this(__pageSize, marginsMm)
        {
        }

        public PdfPageTemplate(Size maximumPageSizeMm)
            : this(maximumPageSizeMm, __defaultPageMargins)
        {
        }

        public PdfPageTemplate(Size maximumPageSizeMm, Thickness marginsMm)
        {
            Guard.Argument(maximumPageSizeMm.Width, nameof(maximumPageSizeMm) + "." + nameof(maximumPageSizeMm.Width))
                .Positive();

            Guard.Argument(maximumPageSizeMm.Height, nameof(maximumPageSizeMm) + "." + nameof(maximumPageSizeMm.Height))
                .Positive();

            MaximumSizeMm = maximumPageSizeMm;

            Guard.Argument(marginsMm.Left, nameof(marginsMm) + "." + nameof(marginsMm.Left))
                .NotNegative();

            Guard.Argument(marginsMm.Top, nameof(marginsMm) + "." + nameof(marginsMm.Top))
                .NotNegative();

            Guard.Argument(marginsMm.Right, nameof(marginsMm) + "." + nameof(marginsMm.Right))
                .NotNegative();

            Guard.Argument(marginsMm.Bottom, nameof(marginsMm) + "." + nameof(marginsMm.Bottom))
                .NotNegative();

            MarginsMm = marginsMm;

            MaximumContentBounds = new Rectangle(marginsMm.Left,
                marginsMm.Top,
                maximumPageSizeMm.Width - marginsMm.Left - marginsMm.Right,
                maximumPageSizeMm.Height - marginsMm.Top - marginsMm.Bottom);
        }

        public Size MaximumSizeMm { get; }

        public Thickness MarginsMm { get; }

        public Rectangle MaximumContentBounds { get; }

        private const double PAGE_WIDTH_MM = 104;
        private const double PAGE_HEIGHT_MM = 266.7; // 10.5 inches

        private static readonly Size __pageSize = new Size(PAGE_WIDTH_MM, PAGE_HEIGHT_MM);

        // Note: 104 mm - 0.25 mm = 103.75 mm = 830 pixels, which for 83 characters, makes the small font exactly 10 pixels wide
        private static readonly Thickness __defaultPageMargins = new Thickness(0, 0, 0.25, 0);
    }
}