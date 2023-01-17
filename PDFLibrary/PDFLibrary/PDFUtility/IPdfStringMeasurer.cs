using Xamarin.Forms;

namespace PDFLibrary
{
    public interface IPdfStringMeasurer
    {
        // Note: returned size is in units of points
        Size Measure(string text, PdfFont font);
    }
}