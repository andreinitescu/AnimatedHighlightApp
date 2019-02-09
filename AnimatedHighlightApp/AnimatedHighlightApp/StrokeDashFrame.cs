namespace AnimatedHighlightApp
{
    class StrokeDashFrame
    {
        public int Key { get; }
        public float[] Intervals { get; }
        public float Phase { get; }

        public StrokeDashFrame(int key, float[] intervals, float phase)
        {
            Key = key;
            Intervals = intervals;
            Phase = phase;
        }
    }
}
