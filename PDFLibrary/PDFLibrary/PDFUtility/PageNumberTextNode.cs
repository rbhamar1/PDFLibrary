using Xamarin.Forms;

namespace PDFLibrary
{
    public sealed class PageNumberTextNode : PdfDynamicTextNode
    {
        public PageNumberTextNode(PdfFont font, TextAlignment alignment = TextAlignment.End, PdfTextPlacement placement = null)
            : base(font, alignment, placement) {}

        public override string GetText(PdfRenderContext renderContext) =>
            //string.Format(Resources.DocumentPageHeaderTemplate, renderContext.PageNumber, renderContext.PageCount);
        
        string.Format("{0} {1}", renderContext.PageNumber, renderContext.PageCount);
    }
}