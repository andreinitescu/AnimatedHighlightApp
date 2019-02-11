using SkiaSharp;
using SkiaSharp.Views.Forms;
using System.Collections.Generic;
using Xamarin.Forms;

namespace AnimatedHighlightApp
{
    class HighlightForm
    {
        readonly HighlightSettings _highlightSettings;
        SKPaint _skPaint;
        HighlightState _highlightState;

        public HighlightForm(HighlightSettings highlightSettings)
        {
            _highlightSettings = highlightSettings;
        }

        public void Draw(SKCanvasView skCanvasView, SKCanvas skCanvas)
        {
            skCanvas.Clear();

            if (_highlightState == null)
                return;

            if (_skPaint == null)
                _skPaint = CreateHighlightSkPaint(skCanvasView, _highlightSettings, _highlightState.HighlightPath);

            StrokeDash strokeDash = _highlightState.StrokeDash;
            // Comment the next line to see whole path without dash effect
            _skPaint.PathEffect = SKPathEffect.CreateDash(strokeDash.Intervals, strokeDash.Phase);

            skCanvas.DrawPath(_highlightState.HighlightPath.Path, _skPaint);
        }

        public void Invalidate(SKCanvasView skCanvasView, Layout<View> formLayout)
        {
            if (_highlightState == null)
                return;

            View viewToHighlight = _highlightState.HighlightPath.GetView(formLayout.Children, _highlightState.CurrHighlightedViewId);
            _highlightState = null;
            HighlightElement(viewToHighlight, skCanvasView, formLayout);
        }

        public void HighlightElement(View viewToHighlight, SKCanvasView skCanvasView, Layout<View> formLayout)
        {
            IList<View> layoutChildren = formLayout.Children;

            if (_highlightState == null)
            {
                var path = HighlightPath.Create(skCanvasView, layoutChildren, _highlightSettings.StrokeWidth);
                _highlightState = new HighlightState()
                {
                    HighlightPath = path
                };
            }

            HighlightPath highlightPath = _highlightState.HighlightPath;

            int currHighlightViewId = _highlightState.CurrHighlightedViewId;
            int iViewIdToHighlight = highlightPath.GetViewId(layoutChildren, viewToHighlight);
            if (currHighlightViewId == iViewIdToHighlight)
                return;

            StrokeDash fromDash;
            if (currHighlightViewId != -1)
                fromDash = _highlightState.StrokeDash;
            else
                fromDash = new StrokeDash(highlightPath.GetDashForView(layoutChildren, iViewIdToHighlight));

            _highlightState.CurrHighlightedViewId = iViewIdToHighlight;

            StrokeDash toDash = new StrokeDash(highlightPath.GetDashForView(layoutChildren, viewToHighlight));
            DrawDash(skCanvasView, fromDash, toDash);
        }

        void DrawDash(SKCanvasView skCanvasView, StrokeDash fromDash, StrokeDash toDash)
        {
            if (fromDash != null)
            {
                var anim = new StrokeDashAnimation(
                    from: fromDash,
                    to: toDash,
                    duration: _highlightSettings.AnimationDuration);

                anim.Start((strokeDashToDraw) => RequestDraw(skCanvasView, strokeDashToDraw));
            }
            else
                RequestDraw(skCanvasView, toDash);
        }

        void RequestDraw(SKCanvasView skCanvasView, StrokeDash strokeDashToDraw)
        {
            _highlightState.StrokeDash = strokeDashToDraw;
            skCanvasView.InvalidateSurface();
        }

        static SKPaint CreateHighlightSkPaint(SKCanvasView skCanvasView, HighlightSettings highlightSettings, HighlightPath highlightPath)
        {
            var skPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Red,
                StrokeWidth = (float)skCanvasView.FromPixels(new Point(0, highlightSettings.StrokeWidth)).Y
            };

            float firstDashIntervalOn = highlightPath.FirstDash.Intervals[0];
            skPaint.Shader = SKShader.CreateLinearGradient(
                                start: new SKPoint(firstDashIntervalOn * 0.30f, 0),
                                end: new SKPoint(firstDashIntervalOn, 0),
                                colors: new SKColor[] {
                                            highlightSettings.StrokeStartColor.ToSKColor(),
                                            highlightSettings.StrokeEndColor.ToSKColor()
                                },
                                colorPos: new float[] { 0, 1 },
                                mode: SKShaderTileMode.Clamp);

            return skPaint;
        }
    }
}
