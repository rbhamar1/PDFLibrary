using Dawn;
using Xamarin.Forms;

namespace PDFLibrary
{
    public sealed class PdfImageBlock : IPdfBlock
    {
        public PdfImageBlock(ImageSource pdfImage, Rectangle bounds)
        {
            ImagePdf = Guard.Argument(pdfImage, nameof(pdfImage))
                            .NotNull();

            Bounds = Guard.Argument(bounds, nameof(bounds))
                          .Member(x => x.Left, info => info.NotNegative())
                          .Member(x => x.Top, info => info.NotNegative())
                          .Member(x => x.Right, info => info.NotNegative())
                          .Member(x => x.Bottom, info => info.Positive());
        }

        public ImageSource ImagePdf { get; }
        public Rectangle Bounds { get; }

        public IPdfBlock Clone(double offsetX, double offsetY)
        {
            Guard.Argument(offsetX, nameof(offsetX))
                 .NotNegative();

            Guard.Argument(offsetY, nameof(offsetY))
                 .NotNegative();

            var bounds = Bounds.Offset(offsetX, offsetY);

            return new PdfImageBlock(ImagePdf, bounds);
        }
    }
}