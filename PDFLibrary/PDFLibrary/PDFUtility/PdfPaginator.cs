using System;
using System.Collections.Generic;
using System.Linq;
using Dawn;
using PDFLibrary.PDFUtility.Barcode;
using Xamarin.Forms;

namespace PDFLibrary
{
    /// <summary>
    /// Accepts nodes representing PDF output and paginates them across one or more pages,
    /// with optional page headers and page footers.
    /// </summary>
    public sealed class PdfPaginator
    {
        public PdfPaginator(PdfPageTemplate pageTemplate, IPdfStringMeasurer stringMeasurer)
        {
            _pageTemplate = Guard.Argument(pageTemplate, nameof(pageTemplate))
                                 .NotNull();

            _stringMeasurer = Guard.Argument(stringMeasurer, nameof(stringMeasurer))
                                   .NotNull()
                                   .Value;

            // __logger.Debug("{Member}: page size=({PageWidth}, {PageHeight})"
            //              + ", margins=({MarginLeft}, {MarginTop}, {MarginRight}, {MarginBottom})"
            //              + ", content=({ContentLeft}, {ContentTop}, {ContentRight}, {ContentBottom})",
            //                "ctor", _pageTemplate.MaximumSizeMm.Width, _pageTemplate.MaximumSizeMm.Height,
            //                _pageTemplate.MarginsMm.Left, _pageTemplate.MarginsMm.Top, _pageTemplate.MarginsMm.Right, _pageTemplate.MarginsMm.Bottom,
            //                _pageTemplate.MaximumContentBounds.Left, _pageTemplate.MaximumContentBounds.Top, _pageTemplate.MaximumContentBounds.Right, _pageTemplate.MaximumContentBounds.Bottom);

            _renderContext = new PdfRenderContext();
        }

        public PdfPaginator(PdfPageTemplate pageTemplate, IPdfStringMeasurer stringMeasurer, PdfTextRegion header)
            : this(pageTemplate, stringMeasurer)
        {
            Guard.Argument(header, nameof(header))
                 .NotNull();

            _headerBlocks = ConvertToBlocks(header);
        }

        public PdfPaginator(PdfPageTemplate pageTemplate, IPdfStringMeasurer stringMeasurer,
                            PdfTextRegion header, PdfTextRegion intermediateFooter, PdfTextRegion finalFooter)
            : this(pageTemplate, stringMeasurer)
        {
            Guard.Argument(header, nameof(header))
                 .NotNull();

            _headerBlocks = ConvertToBlocks(header);

            Guard.Argument(intermediateFooter, nameof(intermediateFooter))
                 .NotNull();

            _intermediateFooterBlocks = ConvertToBlocks(intermediateFooter);

            Guard.Argument(finalFooter, nameof(finalFooter))
                 .NotNull();

            _finalFooterBlocks = ConvertToBlocks(finalFooter);
        }

        public PdfPaginator WithFont(PdfFont font)
        {
            _defaultFont = Guard.Argument(font, nameof(font))
                                .NotNull()
                                .Value;

            return this;
        }

        public PdfPaginator AddBlankRow()
        {
            Guard.Operation(_defaultFont != null, $"{nameof(WithFont)} must be called first");

            AddBlankRowHelper(_defaultFont);

            return this;
        }

        public PdfPaginator AddBlankRow(PdfFont font)
        {
            Guard.Argument(font, nameof(font))
                 .NotNull();

            AddBlankRowHelper(font);

            return this;
        }

        public PdfPaginator AddBlankRow(PdfTextPlacement placement)
        {
            Guard.Operation(_defaultFont != null, $"{nameof(WithFont)} must be called first");

            Guard.Argument(placement, nameof(placement))
                 .NotNull();

            AddBlankRowHelper(_defaultFont, placement);

            return this;
        }

        public PdfPaginator AddRow(string text, TextAlignment alignment = TextAlignment.Start, PdfTextPlacement placement = null)
        {
            Guard.Argument(text, nameof(text))
                 .NotNull();

            Guard.Operation(_defaultFont != null, $"{nameof(WithFont)} must be called first");

            AddRow(new PdfStaticTextNode(text, _defaultFont, alignment, placement));

            return this;
        }

        public PdfPaginator AddRow(string text, PdfFont font, TextAlignment alignment = TextAlignment.Start, PdfTextPlacement placement = null)
        {
            Guard.Argument(text, nameof(text))
                 .NotNull();

            Guard.Argument(font, nameof(font))
                 .NotNull();

            AddRow(new PdfStaticTextNode(text, font, alignment, placement));

            return this;
        }

        public PdfPaginator AddRow(PdfTextNodeBase textNode)
        {
            Guard.Argument(textNode, nameof(textNode))
                 .NotNull();

            EnsureFont(textNode);

            EnsureCurrentPage();

            var bounds = ReserveTextBounds(textNode);
            var block = textNode.CreateBlock(bounds);

            _currentPage.AddBlock(block);
            _contentTop = Math.Max(_contentTop, block.Bounds.Bottom);

            return this;
        }

        public PdfPaginator AddRow(PdfTextRow row)
        {
            Guard.Argument(row, nameof(row))
                 .NotNull();

            Guard.Argument(row.TextNodes, nameof(row) + "." + nameof(row.TextNodes))
                 .NotEmpty();

            EnsureCurrentPage();

            var rowBlocks = GetRowBlocks(row, 0, 0);
            var rowBottom = rowBlocks.Max(b => b.Bounds.Bottom);

            // Note: since we aren't adding _contentTop to the blocks here, the y coordinates
            // remain the same, even if ReserveSpace moves to the next page.
            ReserveSpace(_contentTop + rowBottom);

            AddBlocksToCurrentPage(rowBlocks);

            return this;
        }

        public PdfPaginator AddRows(IEnumerable<string> rows, TextAlignment textAlignment = TextAlignment.Start)
        {
            var rowArray = Guard.Argument(rows, nameof(rows))
                                .NotNull()
                                .Value.ToArray();

            foreach (var row in rowArray)
            {
                AddRow(row, textAlignment);
            }

            return this;
        }

        public PdfPaginator AddRows(IEnumerable<string> rows, PdfFont font, TextAlignment textAlignment = TextAlignment.Start)
        {
            var rowArray = Guard.Argument(rows, nameof(rows))
                                .NotNull()
                                .Value.ToArray();

            Guard.Argument(font, nameof(font))
                 .NotNull();

            foreach (var row in rowArray)
            {
                AddRow(row, font, textAlignment);
            }

            return this;
        }

        public PdfPaginator AddRows(IEnumerable<PdfTextRow> rows)
        {
            var rowArray = Guard.Argument(rows, nameof(rows))
                                .NotNull()
                                .Value.ToArray();

            foreach (var row in rowArray)
            {
                AddRow(row);
            }

            return this;
        }

        public PdfPaginator AddImage(ImageSource pdfImage, double x, double y, double width,
                                     double height)
        {
            Guard.Argument(pdfImage, nameof(pdfImage))
                 .NotNull();

            Guard.Argument(x, nameof(x))
                 .NotNegative();

            Guard.Argument(y, nameof(y))
                 .NotNegative();

            Guard.Argument(width, nameof(width))
                 .Positive();

            Guard.Argument(height, nameof(height))
                 .Positive();

            var blocks = new List<PdfImageBlock>();

            var imageBlock = new PdfImageBlock(pdfImage, new Rectangle(x, y, width, height));
            blocks.Add(imageBlock);
            AddBlocksToCurrentPage(blocks);

            return this;
        }

        // ASSUMPTION: region(s) fit on the current page
        // Note: passing multiple regions is an easy way to generate multiple columns
        public PdfPaginator AddRegions(params PdfTextRegion[] regions)
        {
            Guard.Argument(regions, nameof(regions))
                 .NotNull()
                 .NotEmpty();

            EnsureCurrentPage();

            var combinedBlocks = regions.SelectMany(ConvertToBlocks);

            AddBlocksToCurrentPage(combinedBlocks);

            return this;
        }

        public PdfPaginator AddBarcode(LinearBarcode barcode, double x, double y, double width,
                                       double height, double bottomMargin)
        {
            Guard.Argument(barcode, nameof(barcode))
                 .NotNull();

            Guard.Argument(x, nameof(x))
                 .NotNegative();

            Guard.Argument(y, nameof(y))
                 .NotNegative();

            Guard.Argument(width, nameof(width))
                 .Positive();

            Guard.Argument(height, nameof(height))
                 .Positive();

            EnsureCurrentPage();

            var contentBottom = _contentTop + y + height + bottomMargin;
            ReserveSpace(contentBottom);

            var bounds = new Rectangle(x, y, width, height);

            var block = new PdfLinearBarcodeBlock(barcode, bounds);

            PdfLinearBarcodeBlock[] blocks = {block};
            AddBlocksToCurrentPage(blocks);

            _contentTop += bottomMargin;

            return this;
        }

        public PdfPaginator AddBarcode(Point origin, LinearBarcode barcode, double x, double y,
                                       double width,
                                       double height, double bottomMargin)
        {
            Guard.Argument(barcode, nameof(barcode))
                 .NotNull();

            Guard.Argument(x, nameof(x))
                 .NotNegative();

            Guard.Argument(y, nameof(y))
                 .NotNegative();

            Guard.Argument(width, nameof(width))
                 .Positive();

            Guard.Argument(height, nameof(height))
                 .Positive();

            var bounds = new Rectangle(origin.X, origin.Y, width, height);
            var block = new PdfLinearBarcodeBlock(origin, barcode, bounds);

            PdfLinearBarcodeBlock[] blocks = {block};
            AddBlocksToCurrentPage(blocks);

            _contentTop -= height;
            _contentTop += bottomMargin;

            return this;
        }

        public IEnumerable<PdfPage> GetPages()
        {
            if (!_isComplete)
            {
                _renderContext.ResetPageNumber();

                AddBlocksToCurrentPage(_finalFooterBlocks);

                foreach (var page in _pages)
                {
                    ResolveDynamicTextBlocks(page);

                    _renderContext.AdvancePage();
                }

                _isComplete = true;
            }

            return _pages;
        }

        public PdfPaginator MakeSureCurrentPage()
        {
            Guard.Operation(_defaultFont != null, $"{nameof(WithFont)} must be called first");

            EnsureCurrentPage();

            return this;
        }

        //TODO: current code is short term until the tech debt item is addressed
        public PdfPaginator AddStaticImage(ImageSource pdfImage, double x, double y, double width,
                                           double height)
        {
            Guard.Argument(pdfImage, nameof(pdfImage))
                 .NotNull();

            Guard.Argument(x, nameof(x))
                 .NotNegative();

            Guard.Argument(y, nameof(y))
                 .NotNegative();

            Guard.Argument(width, nameof(width))
                 .Positive();

            Guard.Argument(height, nameof(height))
                 .Positive();

            var blocks = new List<PdfImageBlock>();

            var imageBlock = new PdfImageBlock(pdfImage, new Rectangle(x, y, width, height));
            blocks.Add(imageBlock);
            AddBlocksToCurrentPage(blocks);
            _contentTop -= (height);

            return this;
        }

        public PdfPaginator AddBox(double x, double y, double width,
                                   double height, double thickness, double marginTop = 0)
        {
            Guard.Argument(x, nameof(x))
                 .NotNegative();

            Guard.Argument(y, nameof(y))
                 .NotNegative();

            Guard.Argument(width, nameof(width))
                 .Positive();

            Guard.Argument(height, nameof(height))
                 .Positive();

            var blocks = new List<PdfBoxBlock>();

            var boxBlock = new PdfBoxBlock(new Rectangle(x, y, width, height), thickness, marginTop);
            blocks.Add(boxBlock);
            AddBoxBlocksToCurrentPage(blocks);

            return this;
        }

        //TODO: current code is short term until the tech debt item is addressed
        public double GetCurrentYPosition() => _contentTop;

        private void EnsureCurrentPage()
        {
            if (_currentPage == null)
            {
                AddNewPage();
            }
        }

        private void AddNewPage()
        {
            _currentPage = new PdfPage();

            _pages.Add(_currentPage);
            _renderContext.AddPage();
            _renderContext.AdvancePage();

            _contentTop = 0;
            _contentBottom = _pageTemplate.MaximumContentBounds.Height;

            AddBlocksToCurrentPage(_headerBlocks);

            // ASSUMPTION: _finalFooter is not substantially larger than _intermediateFooter
            if (_intermediateFooterBlocks.Any())
            {
                _contentBottom -= _intermediateFooterBlocks.Max(b => b.Bounds.Bottom);
            }
        }

        // Note: returns true if there is room on this page or moves to the next page and returns false.
        private bool ReserveSpace(double bottom)
        {
            Guard.Argument(bottom, nameof(bottom))
                 .Positive();

            if (bottom <= _contentBottom)
            {
                return true;
            }

            // add footer to this page before moving to the next page
            AddBlocksToCurrentPage(_intermediateFooterBlocks);

            AddNewPage();

            return false;
        }

        // Note: these bounds are adjusted on both the x and y axes
        private Rectangle ReserveTextBounds(PdfTextNodeBase textNode)
        {
            Guard.Argument(textNode, nameof(textNode))
                 .NotNull();

            EnsureCurrentPage();

            var adjustedLeft = textNode.Placement?.X ?? 0;

            var adjustedTop = _contentTop + textNode.Placement.DeltaY;

            var size = GetSize(textNode, adjustedLeft);

            if (!ReserveSpace(adjustedTop + size.Height))
            {
                adjustedTop = _contentTop + textNode.Placement.DeltaY;
            }

            var bounds = new Rectangle(adjustedLeft, adjustedTop, size.Width, size.Height);

            return bounds;
        }

        private Size GetSize(PdfTextNodeBase textNode, double adjustedLeft)
        {
            // Note: the placeholder text may not precisely match the rendered text.
            // But the final version of the text will be retrieved before rendering.
            var placeholderText = textNode.GetText(_renderContext);

            var measuredPointSize = _stringMeasurer.Measure(placeholderText, textNode.Font);

            var measuredMmSize = new Size(PdfUnitConversion.ConvertPointsToMms(measuredPointSize.Width),
                                          PdfUnitConversion.ConvertPointsToMms(measuredPointSize.Height));

            double width;

            if (textNode.Placement.Width != null)
            {
                width = textNode.Placement.Width.Value;
            }
            else
            {
                if (textNode.Alignment == TextAlignment.Start)
                {
                    // fit to the size of the text
                    width = measuredMmSize.Width;
                }
                else
                {
                    // use the remaining space on the row
                    width = _pageTemplate.MaximumContentBounds.Width - adjustedLeft;
                }
            }

            var size = new Size(width, measuredMmSize.Height);

            return size;
        }

        // Note: this creates blocks that have been correctly offset on the x-axis but not the y-axis
        private List<PdfTextBlockBase> ConvertToBlocks(PdfTextRegion region)
        {
            var blocks = new List<PdfTextBlockBase>();

            double rowOffsetY = 0;

            foreach (var rowBlocks in region.Rows.Select(row => GetRowBlocks(row, region.Origin.X, region.Origin.Y + rowOffsetY)))
            {
                blocks.AddRange(rowBlocks);

                rowOffsetY = rowBlocks.Max(b => b.Bounds.Bottom) - region.Origin.Y;
            }

            return blocks;
        }

        // Note: this creates blocks that have been correctly offset on the x-axis but not the y-axis
        private List<PdfTextBlockBase> GetRowBlocks(PdfTextRow row, double originX, double originY)
        {
            var rowBlocks = new List<PdfTextBlockBase>();

            double nodeOffsetX = 0;

            foreach (var textNode in row.TextNodes)
            {
                EnsureFont(textNode);

                // Note: parentheses are necessary due to operator precedence
                var x = originX + (textNode.Placement?.X ?? nodeOffsetX);

                var y = originY + textNode.Placement.DeltaY;

                var size = GetSize(textNode, x);

                var bounds = new Rectangle(x, y, size.Width, size.Height);
                var block = textNode.CreateBlock(bounds);

                rowBlocks.Add(block);

                nodeOffsetX = bounds.Right - originX;
            }

            return rowBlocks;
        }

        private void AddBlocksToCurrentPage(IEnumerable<IPdfBlock> pdfBlocks)
        {
            var pdfBlockArray = pdfBlocks.ToArray();

            if (!pdfBlockArray.Any())
            {
                return;
            }

            var translatedPdfBlocks = pdfBlockArray.Select(b => b.Clone(0, _contentTop))
                                                   .ToList();

            foreach (var pdfBlock in translatedPdfBlocks)
            {
                _currentPage.AddBlock(pdfBlock);
            }

            _contentTop = Math.Max(_contentTop, translatedPdfBlocks.Max(b => b.Bounds.Bottom));
        }

        private void AddBoxBlocksToCurrentPage(IEnumerable<IPdfBlock> pdfBlocks)
        {
            var pdfBlockArray = pdfBlocks.ToArray();

            if (!pdfBlockArray.Any())
            {
                return;
            }

            var translatedPdfBlocks = pdfBlockArray.Select(b => b.Clone(0, _contentTop))
                                                   .ToList();

            foreach (var pdfBlock in translatedPdfBlocks)
            {
                _currentPage.AddBlock(pdfBlock);
            }
        }

        private void ResolveDynamicTextBlocks(PdfPage page)
        {
            for (var blockIndex = 0; blockIndex < page.Blocks.Count; blockIndex++)
            {
                var block = page.Blocks[blockIndex];

                if (block is PdfDynamicTextBlock dynamicTextBlock)
                {
                    var staticTextBlock = dynamicTextBlock.Resolve(_renderContext);
                    page.Blocks[blockIndex] = staticTextBlock;
                }
            }
        }

        private void EnsureFont(PdfTextNodeBase textNode)
        {
            if (textNode.Font != null)
            {
                return;
            }

            Guard.Operation(_defaultFont != null, $"{nameof(WithFont)} must be called first");

            textNode.AssignFont(_defaultFont);
        }

        private void AddBlankRowHelper(PdfFont font)
        {
            AddRow(new PdfStaticTextNode(string.Empty, font));
        }

        private void AddBlankRowHelper(PdfFont font, PdfTextPlacement placement)
        {
            AddRow(new PdfStaticTextNode(string.Empty, font, TextAlignment.Start, placement));
        }

        private readonly List<PdfTextBlockBase> _headerBlocks = new List<PdfTextBlockBase>();
        private readonly List<PdfTextBlockBase> _intermediateFooterBlocks = new List<PdfTextBlockBase>();
        private readonly List<PdfTextBlockBase> _finalFooterBlocks = new List<PdfTextBlockBase>();

        private readonly List<PdfPage> _pages = new List<PdfPage>();

        private readonly PdfPageTemplate _pageTemplate;
        private readonly PdfRenderContext _renderContext;
        private readonly IPdfStringMeasurer _stringMeasurer;
        private double _contentBottom;
        private double _contentTop;

        private PdfPage _currentPage;
        private PdfFont _defaultFont;
        private bool _isComplete;

        // private static readonly ILogger __logger = LoggingExtensions.ForContextEx<PdfPaginator>();
    }
}