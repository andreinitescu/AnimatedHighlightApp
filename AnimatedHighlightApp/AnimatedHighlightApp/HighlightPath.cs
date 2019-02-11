using SkiaSharp;
using System.Collections.Generic;

namespace AnimatedHighlightApp
{
    class HighlightPath
    {
        public SKPath Path { get; }
        public Dictionary<int, StrokeDash> StrokeDashList { get; }

        public HighlightPath(SKPath path, Dictionary<int, StrokeDash> strokeDashList)
        {
            Path = path;
            StrokeDashList = strokeDashList;
        }
    }
}
