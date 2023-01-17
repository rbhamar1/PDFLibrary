using Dawn;
using PDFLibrary.PDFUtility.Barcode;
using Xamarin.Forms;

namespace PDFLibrary
{
    public sealed class PdfLinearBarcodeBlock : IPdfBlock
    {
        public PdfLinearBarcodeBlock(LinearBarcode barcode, Rectangle bounds)
        {
            Barcode = Guard.Argument(barcode, nameof(barcode))
                           .NotNull();

            Bounds = Guard.Argument(bounds, nameof(bounds))
                          .Member(x => x.Left, info => info.NotNegative())
                          .Member(x => x.Top, info => info.NotNegative())
                          .Member(x => x.Right, info => info.NotNegative())
                          .Member(x => x.Bottom, info => info.Positive());
        }

        //TODO: current code is short term until the tech debt item is addressed
        public PdfLinearBarcodeBlock(Point origin, LinearBarcode barcode, Rectangle bounds)
        {
            Barcode = Guard.Argument(barcode, nameof(barcode))
                           .NotNull();

            Bounds = Guard.Argument(bounds, nameof(bounds))
                          .Member(x => x.Left, info => info.NotNegative())
                          .Member(x => x.Top, info => info.HasValue())
                          .Member(x => x.Right, info => info.NotNegative())
                          .Member(x => x.Bottom, info => info.HasValue());

            Origin = origin;
        }

        public Point Origin { get; }

        public LinearBarcode Barcode { get; }

        public Rectangle Bounds { get; }

        public IPdfBlock Clone(double offsetX, double offsetY)
        {
            Guard.Argument(offsetX, nameof(offsetX))
                 .NotNegative();

            Guard.Argument(offsetY, nameof(offsetY))
                 .NotNegative();

            var bounds = Bounds.Offset(offsetX, offsetY);

            return new PdfLinearBarcodeBlock(Barcode, bounds);
        }
    }
}