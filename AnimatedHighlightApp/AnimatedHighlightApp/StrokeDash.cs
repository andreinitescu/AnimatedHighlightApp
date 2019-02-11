namespace AnimatedHighlightApp
{
    class StrokeDash
    {
        public float[] Intervals { get; set; }
        public float Phase { get; set; }

        public StrokeDash(float[] intervals, float phase)
        {
            Intervals = new float[2] { intervals[0], intervals[1] };
            Phase = phase;
        }

        public StrokeDash(StrokeDash strokeDash)
            : this(strokeDash.Intervals, strokeDash.Phase)
        {
        }
    }
}
