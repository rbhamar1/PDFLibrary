using Xamarin.Forms;

namespace PDFLibrary
{
    /// <summary>
    /// Represents a text node where the computation of the rendered text must be deferred until after pagination is complete.
    /// </summary>
    public abstract class PdfDynamicTextNode : PdfTextNodeBase
    {
        protected PdfDynamicTextNode(PdfFont font, TextAlignment alignment = TextAlignment.Start, PdfTextPlacement placement = null)
            : base(font, alignment, placement) {}

        public override PdfTextBlockBase CreateBlock(Rectangle bounds) => new PdfDynamicTextBlock(GetText, Font, bounds, Alignment);
    }
}