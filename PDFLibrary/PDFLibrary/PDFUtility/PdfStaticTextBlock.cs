using Dawn;
using Xamarin.Forms;

namespace PDFLibrary
{
    /// <summary>
    /// Represents a predefined block of text.
    /// </summary>
    public sealed class PdfStaticTextBlock : PdfTextBlockBase
    {
        public PdfStaticTextBlock(string text, PdfFont font, Rectangle bounds, TextAlignment alignment)
            : base(font, bounds, alignment) =>
            Text = Guard.Argument(text, nameof(text))
                        .NotNull();

        public string Text { get; }

        public override IPdfBlock Clone(double offsetX, double offsetY)
        {
            Guard.Argument(offsetX, nameof(offsetX))
                 .NotNegative();

            Guard.Argument(offsetY, nameof(offsetY))
                 .NotNegative();

            var bounds = Bounds.Offset(offsetX, offsetY);

            return new PdfStaticTextBlock(Text, Font, bounds, Alignment);
        }
    }
}