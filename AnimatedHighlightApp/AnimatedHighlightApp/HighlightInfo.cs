using SkiaSharp;
using System.Collections.Generic;

namespace AnimatedHighlightApp
{
    class HighlightPath
    {
        public SKPath Path { get; }
        public Dictionary<int, StrokeDashFrame> StrokeDashFrameList { get; }

        public HighlightPath(SKPath path, Dictionary<int, StrokeDashFrame> strokeDashFrameList)
        {
            Path = path;
            StrokeDashFrameList = strokeDashFrameList;
        }
    }
}
