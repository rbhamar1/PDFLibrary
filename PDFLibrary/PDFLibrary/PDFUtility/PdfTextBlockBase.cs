using Dawn;
using Xamarin.Forms;

namespace PDFLibrary
{
    public abstract class PdfTextBlockBase : IPdfBlock
    {
        protected PdfTextBlockBase(PdfFont font, Rectangle bounds, TextAlignment alignment)
        {
            Font = Guard.Argument(font, nameof(font))
                        .NotNull();

            Guard.Argument(bounds.Left, nameof(bounds) + "." + nameof(bounds.Left))
                 .NotNegative();

            Guard.Argument(bounds.Top, nameof(bounds) + "." + nameof(bounds.Top))
                 .NotNegative();

            Guard.Argument(bounds.Right, nameof(bounds) + "." + nameof(bounds.Right))
                 .NotNegative();

            Guard.Argument(bounds.Bottom, nameof(bounds) + "." + nameof(bounds.Bottom))
                 .Positive();

            Bounds = bounds;

            Alignment = alignment;
        }

        public PdfFont Font { get; }

        public Rectangle Bounds { get; }

        public TextAlignment Alignment { get; }

        public abstract IPdfBlock Clone(double offsetX, double offsetY);
    }
}