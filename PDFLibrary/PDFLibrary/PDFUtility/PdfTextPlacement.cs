using Dawn;

namespace PDFLibrary
{
    /// <summary>
    /// Represents how to place text within its respective container.
    /// </summary>
    public sealed class PdfTextPlacement
    {
        public PdfTextPlacement(double? x = null, double deltaY = 0, double? width = null, bool allowNegativeDelta = false)
        {
            Guard.Argument(x, nameof(x))
                 .NotNegative();

            X = x;

            DeltaY = allowNegativeDelta
                         ? Guard.Argument(deltaY, nameof(deltaY))
                                .NotNaN()
                         : Guard.Argument(deltaY, nameof(deltaY))
                                .NotNegative();

            Guard.Argument(width, nameof(width))
                 .NotNegative();

            Width = width;
        }

        // Note: null means to place this node to the right of the previous node
        // Non-null means to place this node relative to the start of the row.
        public double? X { get; }

        // Note: value indicates how much below the previous row this node should be placed.
        public double DeltaY { get; }

        // Note: null indicates to use the remaining space on the row
        public double? Width { get; }
    }
}