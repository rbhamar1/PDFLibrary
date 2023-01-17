using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreGraphics;
using Dawn;
using Foundation;
using Pdfgenerator.iOS.Pdf;
using PdfGenerator.Pdf;
using PDFLibrary;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace PDFGenerator.iOS.Pdf
{
    //This class generates the final pdf
    public sealed class PdfBuilder : IPdfBuilder
    {
        public PdfBuilder(FontCache fontCache, TextAttributesCache textAttributesCache, double documentTrailerMm, bool enableDebugLogging = false)
        {
            _fontCache = Guard.Argument(fontCache, nameof(fontCache))
                              .NotNull();

            _textAttributesCache = Guard.Argument(textAttributesCache, nameof(textAttributesCache))
                                        .NotNull();

            _documentTrailerMm = Guard.Argument(documentTrailerMm, nameof(documentTrailerMm))
                                      .NotNegative();

            _enableDebugLogging = enableDebugLogging;
        }

        public PDFLibrary.Pdf Generate(PdfPageTemplate pageTemplate, IEnumerable<PdfPage> pages, bool rotatePage, double rotateMargin)
        {
            Guard.Argument(pageTemplate, nameof(pageTemplate))
                 .NotNull();

            var pageArray = pages.ToArray();

            Guard.Argument(pageArray, nameof(pages))
                 .NotNull()
                 .NotEmpty();

            NSData pdfData;

            using (var renderer = new UIGraphicsPdfRenderer())
            {
                // it creates pdf data
                pdfData = renderer.CreatePdf(context => { GeneratePages(context, pageTemplate, pageArray, rotatePage, rotateMargin); });
            }

            return new PDFLibrary.Pdf(pdfData.ToArray());
        }

        private void GeneratePages(UIGraphicsPdfRendererContext context, PdfPageTemplate pageTemplate, PdfPage[] pageArray, bool rotatePage,
                                   double rotateMargin)
        {
            var extraPageInfo = new NSDictionary<NSString, NSObject>();

            var offset = new Point(PdfUnitConversion.ConvertMmsToPoints(pageTemplate.MarginsMm.Left),
                                   PdfUnitConversion.ConvertMmsToPoints(pageTemplate.MarginsMm.Top));

            foreach (var page in pageArray)
            {
                var pageHeightExtraMm = (page == pageArray.Last()) ? _documentTrailerMm : 0;

                var pageHeightMm = pageTemplate.MarginsMm.Top + page.ContentHeight + pageTemplate.MarginsMm.Bottom + pageHeightExtraMm;

                if (rotatePage)
                {
                    context.CGContext.SaveState();

                    var pageRect = new CGRect(0,
                                              0,
                                              PdfUnitConversion.ConvertMmsToPoints(pageHeightMm),
                                              PdfUnitConversion.ConvertMmsToPoints(pageTemplate.MaximumSizeMm.Width));

                    context.BeginPage(pageRect, extraPageInfo);

                    context.CGContext.RotateCTM(90 * ((float) Math.PI) / 180);
                    context.CGContext.TranslateCTM(0, -1f * ((nfloat) pageTemplate.MaximumSizeMm.Width + (nfloat) rotateMargin));
                }
                else
                {
                    var pageRect = new CGRect(0,
                                              0,
                                              PdfUnitConversion.ConvertMmsToPoints(pageTemplate.MaximumSizeMm.Width),
                                              PdfUnitConversion.ConvertMmsToPoints(pageHeightMm));
                    
                    context.BeginPage(pageRect, extraPageInfo);
                }

                foreach (var block in page.Blocks)
                {
                    RenderBlock(block, context.CGContext, offset, _enableDebugLogging);
                }

                if (rotatePage)
                {
                    context.CGContext.RestoreState();
                }
            }
        }

        /// <summary>
        /// It takes the blocks built by paginator and draws the views on ths page
        /// using cgContext
        /// </summary>
        /// <param name="block"></param>
        /// <param name="cgContext"></param>
        /// <param name="offset"></param>
        /// <param name="enableDebugLogging"></param>
        private void RenderBlock(IPdfBlock block, CGContext cgContext, Point offset, bool enableDebugLogging)
        {
            CGRect bounds;

            switch (block)
            {
                
                case PdfStaticTextBlock textBlock:
                    var nativeString = BuildNativeString(textBlock.Text, textBlock.Font, textBlock.Alignment);

                    bounds = new CGRect(offset.X + PdfUnitConversion.ConvertMmsToPoints(textBlock.Bounds.X),
                                        offset.Y + PdfUnitConversion.ConvertMmsToPoints(textBlock.Bounds.Y),
                                        PdfUnitConversion.ConvertMmsToPoints(textBlock.Bounds.Width),
                                        PdfUnitConversion.ConvertMmsToPoints(textBlock.Bounds.Height));
                    

                    nativeString.DrawString(bounds);

                    break;
                case PdfImageBlock imageBlock:
                    bounds = new CGRect(offset.X + PdfUnitConversion.ConvertMmsToPoints(imageBlock.Bounds.X),
                                        offset.Y + PdfUnitConversion.ConvertMmsToPoints(imageBlock.Bounds.Y),
                                        PdfUnitConversion.ConvertMmsToPoints(imageBlock.Bounds.Width),
                                        PdfUnitConversion.ConvertMmsToPoints(imageBlock.Bounds.Height));

                    if (imageBlock.ImagePdf is StreamImageSource)
                    {
                        var loadImageTask = Task.Run(async () => await LoadImageAsync(imageBlock));
                        var originalImage = loadImageTask.Result;

                        var renderer = new UIGraphicsImageRenderer(bounds.Size);

                        // ReSharper disable once AccessToDisposedClosure
                        var resizedImage = renderer.CreateImage(context => originalImage.Draw(new CGRect(0, 0, bounds.Width, bounds.Height)));

                        cgContext.DrawImage(FlipImageBounds(bounds), resizedImage.CGImage);
                    }

                    break;

                case PdfLinearBarcodeBlock barcodeBlock:
                    bounds = new CGRect(offset.X + PdfUnitConversion.ConvertMmsToPoints(barcodeBlock.Bounds.X),
                                        offset.Y + PdfUnitConversion.ConvertMmsToPoints(barcodeBlock.Bounds.Y),
                                        PdfUnitConversion.ConvertMmsToPoints(barcodeBlock.Bounds.Width),
                                        PdfUnitConversion.ConvertMmsToPoints(barcodeBlock.Bounds.Height));
                    

                    var barcodeImage = BuildBarcodeImage(barcodeBlock);

                    cgContext.DrawImage(FlipImageBounds(bounds), barcodeImage);

                    break;
                
                // This block is used to draw empty boxes

                case PdfBoxBlock boxBlock:
                    var marginttop = (nfloat) PdfUnitConversion.ConvertMmsToPoints(boxBlock.MarginTop);

                    bounds = new CGRect(PdfUnitConversion.ConvertMmsToPoints(boxBlock.Bounds.X),
                                        PdfUnitConversion.ConvertMmsToPoints(boxBlock.Bounds.Y),
                                        PdfUnitConversion.ConvertMmsToPoints(boxBlock.Bounds.Width),
                                        PdfUnitConversion.ConvertMmsToPoints(boxBlock.Bounds.Height));
                    
                    CoreGraphicsHelper.DrawHorizontalLineStartingAtPoint(cgContext, new CGColor(0, 0, 0), bounds.X, bounds.X + bounds.Width, bounds.Height + marginttop, (nfloat) boxBlock.Thickness);
                    CoreGraphicsHelper.DrawVertictalLineStartingAtPoint(cgContext, new CGColor(0, 0, 0), bounds.X, bounds.Y, bounds.Height + marginttop, (nfloat) boxBlock.Thickness);
                    CoreGraphicsHelper.DrawVertictalLineStartingAtPoint(cgContext, new CGColor(0, 0, 0), bounds.X + bounds.Width, bounds.Y, bounds.Height + marginttop, (nfloat) boxBlock.Thickness);
                    CoreGraphicsHelper.DrawHorizontalLineStartingAtPoint(cgContext, new CGColor(0, 0, 0), bounds.X, bounds.X + bounds.Width, bounds.Y, (nfloat) boxBlock.Thickness);

                    break;
            }
        }

        private static Task<UIImage> LoadImageAsync(PdfImageBlock imageBlock)
        {
            var handler = new StreamImagesourceHandler();

            return handler.LoadImageAsync(imageBlock.ImagePdf);
        }

        private static CGImage BuildBarcodeImage(PdfLinearBarcodeBlock barcodeBlock)
        {
            UIGraphics.BeginImageContext(new CGSize(barcodeBlock.Barcode.Count, 1));
            var context = UIGraphics.GetCurrentContext();
            context.SetFillColor(new CGColor(0, 0, 0));

            for (var x = 0; x < barcodeBlock.Barcode.Count; x++)
            {
                if (barcodeBlock.Barcode[x])
                {
                    context.FillRect(new CGRect(x, 0, 1, 1));
                }
            }

            var uiImage = UIGraphics.GetImageFromCurrentImageContext();

            UIGraphics.EndImageContext();

            return uiImage.CGImage;
        }

        private static CGRect FlipImageBounds(CGRect bounds)
        {
            var flippedBounds = bounds;

            flippedBounds.Height = flippedBounds.Height * -1;
            flippedBounds.Y = bounds.Height + flippedBounds.Y;

            return flippedBounds;
        }

        private static string DescribeCgRectPoints(CGRect bounds) => $"({bounds.Left:F2}, {bounds.Top:F2}, {bounds.Right:F2}, {bounds.Bottom:F2}) points";

        private NSAttributedString BuildNativeString(string text, PdfFont font, TextAlignment alignment)
        {
            var nativeFont = _fontCache.GetFont(font);

            var textAttributes = _textAttributesCache.GetTextAttributes(nativeFont, alignment);

            var nativeString = new NSAttributedString(text, textAttributes);

            return nativeString;
        }

        public Size Measure(string text, PdfFont font)
        {
            var nativeString = BuildNativeString(text, font, TextAlignment.Start);
            var nativeSize = nativeString.Size;

            return new Size(nativeSize.Width, nativeSize.Height);
        }

        private readonly FontCache _fontCache;
        private readonly TextAttributesCache _textAttributesCache;
        private readonly double _documentTrailerMm;
        private readonly bool _enableDebugLogging;
    }
}