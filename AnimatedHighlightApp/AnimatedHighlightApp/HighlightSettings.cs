using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AnimatedHighlightApp
{
    class HighlightSettings
    {
        public Color StrokeStartColor { get; set; }
        public Color StrokeEndColor { get; set; }
        public double StrokeWidth { get; set; }
        public TimeSpan AnimationDuration { get; set; }
        public Easing AnimationEasing { get; set; }
        public IList<View> Elements { get; set; }
    }
}
