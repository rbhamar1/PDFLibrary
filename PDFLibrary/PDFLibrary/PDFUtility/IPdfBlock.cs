using Xamarin.Forms;

namespace PDFLibrary
{
    /// <summary>
    /// Represents a rectangular region of output on a PDF page.
    /// </summary>
    public interface IPdfBlock
    {
        Rectangle Bounds { get; }

        IPdfBlock Clone(double offsetX, double offsetY);
    }
}