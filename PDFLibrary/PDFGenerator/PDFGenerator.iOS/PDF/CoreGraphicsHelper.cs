using System;
using CoreGraphics;

namespace PDFGenerator.iOS.Pdf
{
    /// <summary>
    /// This is helper class to vertical and horizontal
    /// lines of given color, thickness and co-ordinates
    /// </summary>
    public class CoreGraphicsHelper
    {
        public static void DrawHorizontalLineStartingAtPoint(CGContext graphicsContext, CGColor color, nfloat startX, nfloat endX,
                                                             nfloat y, nfloat thickness)
        {
            graphicsContext.SetStrokeColor(color);
            graphicsContext.SetLineWidth(thickness);

            y += thickness / 2;

            graphicsContext.MoveTo(startX, y);
            graphicsContext.AddLineToPoint(endX, y);
            graphicsContext.StrokePath();
        }

        public static void DrawVertictalLineStartingAtPoint(CGContext graphicsContext, CGColor color, nfloat x, nfloat startY,
                                                            nfloat endY, nfloat thickness)
        {
            graphicsContext.SaveState();
            graphicsContext.SetStrokeColor(color);
            graphicsContext.SetLineWidth(thickness);
            graphicsContext.MoveTo(x, startY);
            graphicsContext.AddLineToPoint(x, endY);
            graphicsContext.StrokePath();
            graphicsContext.RestoreState();
        }
    }
}