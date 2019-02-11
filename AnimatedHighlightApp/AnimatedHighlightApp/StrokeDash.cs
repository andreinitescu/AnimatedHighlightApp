using System;

namespace AnimatedHighlightApp
{
    class StrokeDash
    {
        public float[] Intervals { get; set; }
        public float Phase { get; set; }

        public StrokeDash(float[] intervals, float phase)
        {
            Intervals = new float[intervals.Length];
            Array.Copy(intervals, Intervals, intervals.Length);
            Phase = phase;
        }

        public StrokeDash(StrokeDash strokeDash)
            : this(strokeDash.Intervals, strokeDash.Phase)
        {
        }
    }
}
