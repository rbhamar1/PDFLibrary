using Dawn;
using Xamarin.Forms;

namespace PDFLibrary
{
    public sealed class PdfBoxBlock : IPdfBlock
    {
        public PdfBoxBlock(Rectangle bounds, double thickness, double marginTop)
        {
            Bounds = Guard.Argument(bounds, nameof(bounds))
                          .Member(x => x.Left, info => info.NotNegative())
                          .Member(x => x.Top, info => info.NotNegative())
                          .Member(x => x.Right, info => info.NotNegative())
                          .Member(x => x.Bottom, info => info.Positive());

            Thickness = thickness;
            MarginTop = marginTop;
        }

        public Rectangle Bounds { get; }

        public readonly double Thickness;
        public readonly double MarginTop;

        public IPdfBlock Clone(double offsetX, double offsetY)
        {
            Guard.Argument(offsetX, nameof(offsetX))
                 .NotNegative();

            Guard.Argument(offsetY, nameof(offsetY))
                 .NotNegative();

            var bounds = Bounds.Offset(offsetX, offsetY);

            return new PdfBoxBlock(bounds, Thickness, MarginTop);
        }
    }
}