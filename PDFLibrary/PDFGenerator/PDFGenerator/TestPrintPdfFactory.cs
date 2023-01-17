using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using PdfGenerator.Pdf;
using PDFLibrary;
using Xamarin.Forms;

namespace PDFGenerator
{
    public sealed class TestPrintPdfFactory : ITestPrintPdfFactory
    {
        public TestPrintPdfFactory()
        {
            IPdfBuilderFactory factory = DependencyService.Get<IPdfBuilderFactory>();
            _pdfBuilder = factory.Create();
        }

        public  Pdf Create()
        {
            var pageTemplate = new PdfPageTemplate(__pageMargins);
            var pdfBuilder = _pdfBuilder;
            var pages = BuildTestPrint(pageTemplate, pdfBuilder);

            var pdf = pdfBuilder.Generate(pageTemplate, pages);

            return pdf;
        }

        private IEnumerable<PdfPage> BuildTestPrint(PdfPageTemplate pageTemplate, IPdfStringMeasurer stringMeasurer)
        {
            //PdfFontFactory class has the methods to select the font and Create the font size
            var font = PdfFontFactory.CreateSmallRegularFont();
            var decimalLine = new string('=', PAGE_WIDTH_CHARACTERS);
            var now = DateTime.Now;
            var dateText = now.Date.ToString("MM-dd-yy");
            var timeText = now.ToString("HH:mm:ss");
            
            // Paginator is the main class which layouts different views on the page 
            // according to thier height and width
            // PdfPageTemplate contains info like hieght, width and margins of the page
            var paginator = new PdfPaginator(pageTemplate, stringMeasurer)
                .WithFont(font);

            paginator.AddBlankRow();
            
            var mainHeaderRegion = AddHeaderSection(PdfFontFactory.CreateRegularFont(2,4));
            paginator.MakeSureCurrentPage();

            paginator.AddStaticImage(EmbeddedSourceror.SourceFor("Ibm_logo.png"), 5, 5, 40, 20)
                .AddRegions(mainHeaderRegion);
            
            //this will provide space between two blocks
            paginator.AddBlankRow();

            // Note: this document is mostly decimal-spaced, which is very uncommon
            paginator.AddBlankRow()
                .AddRow(decimalLine)
                .AddBlankRow()
                .AddRow(new PdfTextRow()
                    .AddPromptText("DATE", new PdfTextPlacement(FIRST_COLUMN_MM, 0, 7 * font.CharWidthMm))
                    .AddText(dateText, TextAlignment.Start, new PdfTextPlacement(SECOND_COLUMN_MM)))
                .AddRow(new PdfTextRow()
                    .AddPromptText("TIME", new PdfTextPlacement(FIRST_COLUMN_MM, 0, 7 * font.CharWidthMm))
                    .AddText(timeText, TextAlignment.Start, new PdfTextPlacement(SECOND_COLUMN_MM)))
                .AddBlankRow()
                .AddBlankRow();
            AddProductsHeaderSection(paginator, PdfFontFactory.CreateSmallBoldFont());
            AddProducts(paginator,font);
            
                paginator.AddBlankRow()
                           .AddRow(decimalLine)
                           .AddBlankRow()
                           .AddRow("This is a sample report", TextAlignment.Center)
                           .AddBlankRow()
                           .AddBlankRow()
                           .AddRow("-------End of the Report--------", TextAlignment.Center);

            return paginator.GetPages();
        }
        
         private PdfTextRegion AddHeaderSection( PdfFont smallBoldFont)
        {
            var mainHeaderRegion = new PdfTextRegion(new Point(PAGE_WIDTH_CHARACTERS - 30, 10))
                                  .AddRow(new PdfTextRow()
                                             .AddText($"Heading 1", smallBoldFont, 10, 40))
                                  .AddBlankRow();

            return mainHeaderRegion;
        }

        
        private void AddProductsHeaderSection(PdfPaginator paginator, PdfFont regularBoldFont)
        {
            paginator.AddRow(new PdfTextRow()
                .AddText($"ItemName", regularBoldFont, 3, 10, TextAlignment.Center)
                .AddText($"ItemDescription", regularBoldFont, 13, 20, TextAlignment.Center)
                .AddText($"ItemCount", regularBoldFont, 33 , 12, TextAlignment.Center)
                .AddText($"ItemPrice", regularBoldFont, 45, 14, TextAlignment.Center));
        }
        
        private void AddProducts(PdfPaginator paginator, PdfFont regularBoldFont)
        {
            for (int i = 1; i < 5; i++)
            {
                paginator.AddRow(new PdfTextRow()
                    .AddText($"ItemName{i}", regularBoldFont, 5, 10, TextAlignment.Center)
                    .AddText($"SampleDescription{i}", regularBoldFont, 19, 20, TextAlignment.Center)
                    .AddText($"ItemCount{i}", regularBoldFont, 42 , 12, TextAlignment.Center)
                    .AddText($"ItemPrice{i}", regularBoldFont, 58, 14, TextAlignment.Center));
            }
            
        }

        private static readonly Thickness __pageMargins = new Thickness(2.0, 0);

        private const int PAGE_WIDTH_CHARACTERS = 80;

        private const double FIRST_COLUMN_MM = 78.75;
        private const double SECOND_COLUMN_MM = 86.25;
        private readonly IPdfBuilder _pdfBuilder;
    }
    
    public sealed class EmbeddedSourceror
    {
        public static ImageSource SourceFor(string filenameWithExtension)
        {
            var resources = typeof(EmbeddedSourceror).GetTypeInfo()
                .Assembly.GetManifestResourceNames();

            return ImageSource.FromResource(resources.First(r => r.EndsWith(filenameWithExtension, StringComparison.OrdinalIgnoreCase)));
        }
    }
}