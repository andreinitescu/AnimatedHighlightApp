using SkiaSharp;
using System.Collections.Generic;

namespace AnimatedHighlightApp
{
    class HighlightPath
    {
        public SKPath Path { get; }
        public IList<StrokeDashFrame> StrokeDashFrameList { get; }

        public HighlightPath(SKPath path, IList<StrokeDashFrame> strokeDashFrameList)
        {
            Path = path;
            StrokeDashFrameList = strokeDashFrameList;
        }
    }
}
