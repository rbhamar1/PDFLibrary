using System;
using Dawn;
using Xamarin.Forms;

namespace PDFLibrary
{
    /// <summary>
    /// Represents a text block where the computation of the rendered text must be deferred until after pagination is complete.
    /// </summary>
    public sealed class PdfDynamicTextBlock : PdfTextBlockBase
    {
        public PdfDynamicTextBlock(Func<PdfRenderContext, string> getText, PdfFont font, Rectangle bounds, TextAlignment alignment)
            : base(font, bounds, alignment) =>
            _getText = Guard.Argument(getText, nameof(getText))
                            .NotNull()
                            .Value;

        public override IPdfBlock Clone(double offsetX, double offsetY)
        {
            var bounds = Bounds.Offset(offsetX, offsetY);

            return new PdfDynamicTextBlock(_getText, Font, bounds, Alignment);
        }

        public PdfStaticTextBlock Resolve(PdfRenderContext renderContext)
        {
            var text = _getText(renderContext);

            return new PdfStaticTextBlock(text, Font, Bounds, Alignment);
        }

        private readonly Func<PdfRenderContext, string> _getText;
    }
}