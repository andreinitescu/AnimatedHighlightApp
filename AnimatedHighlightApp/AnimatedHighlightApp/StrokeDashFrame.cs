namespace AnimatedHighlightApp
{
    class StrokeDashFrame
    {
        public float IntervalOn { get; }
        public float IntervalOff { get; }
        public float Phase { get; }

        public StrokeDashFrame(float intervalOn, float intervalOff, float phase)
        {
            IntervalOn = intervalOn;
            IntervalOff = intervalOff;
            Phase = phase;
        }
    }
}
