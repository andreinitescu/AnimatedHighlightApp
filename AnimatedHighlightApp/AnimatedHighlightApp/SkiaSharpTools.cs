using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace AnimatedHighlightApp
{
    static class SkiaSharpTools
    {
        public static Rectangle FromPixels(this SKCanvasView skCanvasView, Rectangle rc) =>
            new Rectangle(skCanvasView.FromPixels(rc.Location), skCanvasView.FromPixels(rc.Size));

        public static Size FromPixels(this SKCanvasView skCanvasView, Size rc) =>
            (Size)skCanvasView.FromPixels(new Point(rc.Width, rc.Height));

        public static Point FromPixels(this SKCanvasView skCanvasView, Point pt)
        {
            double wf = skCanvasView.CanvasSize.Width / skCanvasView.Width;
            double hf = skCanvasView.CanvasSize.Height / skCanvasView.Height;
            return new Point(pt.X * wf, pt.Y * hf);
        }
    }
}
