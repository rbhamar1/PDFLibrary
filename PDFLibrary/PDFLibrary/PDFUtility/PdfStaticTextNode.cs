using Dawn;
using Xamarin.Forms;

namespace PDFLibrary
{
    /// <summary>
    /// Represents a predefined text node.
    /// </summary>
    public sealed class PdfStaticTextNode : PdfTextNodeBase
    {
        public PdfStaticTextNode(PdfFont font = null, TextAlignment alignment = TextAlignment.Start, PdfTextPlacement placement = null)
            : this(string.Empty, font, alignment, placement) {}

        public PdfStaticTextNode(string text, PdfFont font = null, TextAlignment alignment = TextAlignment.Start, PdfTextPlacement placement = null)
            : base(font, alignment, placement) =>
            _text = Guard.Argument(text, nameof(text))
                         .NotNull();

        public override string GetText(PdfRenderContext renderContext) => _text;

        public override PdfTextBlockBase CreateBlock(Rectangle bounds) => new PdfStaticTextBlock(_text, Font, bounds, Alignment);

        private readonly string _text;
    }
}