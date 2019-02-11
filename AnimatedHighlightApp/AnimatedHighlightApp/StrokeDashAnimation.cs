using System;
using Xamarin.Forms;

namespace AnimatedHighlightApp
{
    class StrokeDashAnimation
    {
        StrokeDash _currStrokeDash;

        public StrokeDash From { get; }
        public StrokeDash To { get; }
        public TimeSpan Duration { get; }
        public Easing Easing { get; }

        public StrokeDashAnimation(StrokeDash from, StrokeDash to, TimeSpan duration)
        {
            From = from;
            To = to;
            Duration = duration;
        }

        public void Start(Action<StrokeDash> onValueCallback)
        {
            _currStrokeDash = From;

            var anim = new Animation(_ => onValueCallback(_currStrokeDash));

            anim.Add(0, 1, new Animation(
                callback: v => _currStrokeDash.Phase = (float)v,
                start: From.Phase,
                end: To.Phase,
                easing: Easing));

            anim.Add(0, 1, new Animation(
                callback: v => _currStrokeDash.Intervals[0] = (float)v,
                start: From.Intervals[0],
                end: To.Intervals[0],
                easing: Easing));

            anim.Add(0, 1, new Animation(
                callback: v => _currStrokeDash.Intervals[1] = (float)v,
                start: From.Intervals[1],
                end: To.Intervals[1],
                easing: Easing));

            anim.Commit(
                owner: Application.Current.MainPage,
                name: "highlightAnimation",
                length: (uint)Duration.TotalMilliseconds);
        }
    }
}
