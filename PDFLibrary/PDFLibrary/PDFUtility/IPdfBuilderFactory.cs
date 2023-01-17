using PdfGenerator.Pdf;

namespace PDFLibrary
{
    public interface IPdfBuilderFactory
    {
        IPdfBuilder Create(bool enableDebugLogging = false);
    }
}