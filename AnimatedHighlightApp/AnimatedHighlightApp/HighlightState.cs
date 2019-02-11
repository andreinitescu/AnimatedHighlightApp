namespace AnimatedHighlightApp
{
    class HighlightState
    {
        public int CurrHighlightedViewId { get; set; } = -1;
        public StrokeDash StrokeDash { get; set; }
        public HighlightPath HighlightPath { get; set; }
    }
}
