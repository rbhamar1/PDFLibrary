using System.Collections.Generic;
using PDFLibrary;

namespace PdfGenerator.Pdf
{
    /// <summary>
    /// Builds a PDF from a set of logical pages.
    /// </summary>
    public interface IPdfBuilder : IPdfStringMeasurer
    {
        PDFLibrary.Pdf Generate(PdfPageTemplate pageTemplate, IEnumerable<PdfPage> pages, bool rotatePage = false, double rotateMargin = 0);
    }
}