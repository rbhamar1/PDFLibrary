using Dawn;
using Xamarin.Forms;

namespace PDFLibrary
{
    public abstract class PdfTextNodeBase
    {
        protected PdfTextNodeBase(PdfFont font, TextAlignment alignment, PdfTextPlacement placement = null)
        {
            Font = font;

            Alignment = Guard.Argument(alignment, nameof(alignment))
                             .Defined();

            Placement = placement ?? new PdfTextPlacement();
        }

        public PdfFont Font { get; private set; }

        public void AssignFont(PdfFont font)
        {
            Guard.Operation(Font == null, "Font has already been set");

            Font = Guard.Argument(font, nameof(font))
                        .NotNull();
        }

        public TextAlignment Alignment { get; }

        public PdfTextPlacement Placement { get; }

        public abstract string GetText(PdfRenderContext renderContext);

        public abstract PdfTextBlockBase CreateBlock(Rectangle bounds);
    }
}