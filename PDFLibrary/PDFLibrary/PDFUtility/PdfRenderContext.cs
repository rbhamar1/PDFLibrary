namespace PDFLibrary
{
    public sealed class PdfRenderContext
    {
        // Note: the page number is 1-based
        public int PageNumber { get; private set; }

        public void AdvancePage() => PageNumber++;

        public void ResetPageNumber() => PageNumber = 1;

        public void AddPage() => PageCount++;

        public int PageCount { get; private set; }
    }
}