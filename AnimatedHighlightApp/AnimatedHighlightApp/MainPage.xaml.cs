using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace AnimatedHighlightApp
{
    public partial class MainPage : ContentPage
    {
        readonly HighlightSettings _highlightSettings;
        HighlightPath _highlightPath;
        StrokeDash _currStrokeDash;
        SKPaint _skPaint;
        int _iCurrFocusedView = -1;

        public MainPage()
        {
            InitializeComponent();

            _highlightSettings = new HighlightSettings()
            {
                StrokeWidth = 6,
                StrokeStartColor = Color.FromHex("#FF4600"),
                StrokeEndColor = Color.FromHex("#CC00AF"),
                AnimationDuration = TimeSpan.FromMilliseconds(900),
                AnimationEasing = Easing.CubicInOut,
            };
        }

        void SkCanvasViewSizeChanged(object sender, EventArgs e)
        {
            InvalidateHighlight();
        }

        void SkCanvasViewRequiredPainting(object sender, SKPaintSurfaceEventArgs e)
        {
            SKSurface skSurface = e.Surface;
            SKCanvas skCanvas = skSurface.Canvas;
            skCanvas.Clear();

            if (_highlightSettings == null || _currStrokeDash == null)
                return;

            // Comment the next line to see whole path without dash effect
            _skPaint.PathEffect = SKPathEffect.CreateDash(intervals: _currStrokeDash.Intervals, phase: _currStrokeDash.Phase);
            skCanvas.DrawPath(_highlightPath.Path, _skPaint);
        }

        void EntryFocused(object sender, FocusEventArgs e)
        {
            StartHighlightAnimationToFormElement((View)sender);
        }

        void ButtonClicked(object sender, EventArgs e)
        {
            StartHighlightAnimationToFormElement((View)sender);
        }

        void StartHighlightAnimationToFormElement(View view)
        {
            int iFocusedView = _formLayout.Children.IndexOf(view);
            if (_iCurrFocusedView == iFocusedView)
                return;

            _iCurrFocusedView = iFocusedView;

            if (_currStrokeDash != null)
            {
                var frameTo = _highlightPath.StrokeDashFrameList.First(sd => sd.Key == _iCurrFocusedView);

                var anim = new StrokeDashAnimation(
                    from: _currStrokeDash,
                    to: new StrokeDash(frameTo.Intervals, frameTo.Phase),
                    duration: _highlightSettings.AnimationDuration);

                anim.Start((strokeDash) =>
                {
                    _currStrokeDash = strokeDash;
                    _skCanvasView.InvalidateSurface();
                });
            }
            else
            {
                InvalidateHighlight();
            }
        }

        void InvalidateHighlight()
        {
            if (_iCurrFocusedView == -1)
                return;

            _highlightPath = CreatePathHighlightInfo(_skCanvasView, _formLayout, (float)_highlightSettings.StrokeWidth);
            _skPaint = CreateHighlightSkPaint(_skCanvasView, _highlightPath, _highlightSettings);

            var currFrame = _highlightPath.StrokeDashFrameList.First(sd => sd.Key == _iCurrFocusedView);
            _currStrokeDash = new StrokeDash(currFrame.Intervals, currFrame.Phase);
            _skCanvasView.InvalidateSurface();
        }

        static SKPaint CreateHighlightSkPaint(
            SKCanvasView sKCanvasView,
            HighlightPath highlightPath,
            HighlightSettings highlightSettings)
        {
            var skPaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Red,
                StrokeWidth = (float)sKCanvasView.FromPixels(new Point(0, highlightSettings.StrokeWidth)).Y
            };

            float firstInterval = highlightPath.StrokeDashFrameList[0].Intervals[0];
            skPaint.Shader = SKShader.CreateLinearGradient(
                                start: new SKPoint(firstInterval * 0.30f, 0),
                                end: new SKPoint(firstInterval, 0),
                                colors: new SKColor[] {
                                            highlightSettings.StrokeStartColor.ToSKColor(),
                                            highlightSettings.StrokeEndColor.ToSKColor()
                                },
                                colorPos: new float[] { 0, 1 },
                                mode: SKShaderTileMode.Clamp);

            return skPaint;
        }

        static HighlightPath CreatePathHighlightInfo(SKCanvasView sKCanvasView, Layout<View> layout, float strokeWidth)
        {
            var path = new SKPath();
            var strokeDashFrameList = new List<StrokeDashFrame>();
            strokeWidth = (float)sKCanvasView.FromPixels(new Point(0, strokeWidth)).Y;

            IList<View> layoutChildren = layout.Children;
            for (int iLayoutChild = 0; iLayoutChild < layoutChildren.Count; ++iLayoutChild)
            {
                int iStrokeDashCount = strokeDashFrameList.Count;
                View view = layoutChildren[iLayoutChild];
                Rectangle viewBounds = sKCanvasView.FromPixels(view.Bounds);

                if (!(view is Entry) && !(view is Button))
                {
                    continue;
                }

                if (path.Points.Length == 0)
                {
                    // Move path point at the left and below of the view
                    path.MoveTo(
                        x: (float)viewBounds.X,
                        y: (float)viewBounds.Y + (float)viewBounds.Height + strokeWidth / 2);
                }

                float xCurr = path.LastPoint.X;
                float yCurr = path.LastPoint.Y;

                // Add arch for views except first one
                if (iStrokeDashCount > 0)
                {
                    float d = iStrokeDashCount % 2 == 0 ? -1 : 1;
                    float arcHeight = (float)viewBounds.Y + (float)viewBounds.Height - path.LastPoint.Y + strokeWidth / 2;
                    path.ArcTo(new SKRect(xCurr - arcHeight / 2, yCurr, xCurr + arcHeight / 2, yCurr + arcHeight), -90, 180 * d, false);
                }

                float dashOffset = new SKPathMeasure(path).Length;

                // Add line below the view
                // If it's not the first view, the start point is the end from arc end point 
                //  and line direction is either to view start or view end
                path.LineTo((float)viewBounds.X + (float)viewBounds.Width * (iStrokeDashCount % 2 == 0 ? 1 : 0), path.LastPoint.Y);

                if (view is Button)
                {
                    xCurr = path.LastPoint.X;
                    yCurr = path.LastPoint.Y;

                    // Draw arc from below button to above button
                    float d = iStrokeDashCount % 2 == 0 ? -1 : 1;
                    float arcHeight = (float)viewBounds.Height + strokeWidth;
                    path.ArcTo(new SKRect(xCurr - arcHeight / 2, yCurr - arcHeight, xCurr + arcHeight / 2, yCurr), 90, 180 * d, false);

                    // Draw horizontal line above the button
                    path.LineTo(xCurr + (float)viewBounds.Width * d, path.LastPoint.Y);

                    // Draw arc pointing down
                    xCurr = path.LastPoint.X;
                    yCurr = path.LastPoint.Y;
                    path.ArcTo(new SKRect(xCurr - arcHeight / 2, yCurr, xCurr + arcHeight / 2, yCurr + arcHeight), -90, 180 * d, false);
                }

                float solidDashLength = new SKPathMeasure(path).Length - dashOffset;

                strokeDashFrameList.Add(new StrokeDashFrame(
                    iLayoutChild,
                    new float[] { solidDashLength, 0 },
                    -dashOffset));
            }

            // Compute the 2nd value of interval, which is the length of remaining path
            float pathLength = new SKPathMeasure(path).Length;
            for (int i = 0; i < strokeDashFrameList.Count; ++i)
            {
                strokeDashFrameList[i].Intervals[1] = pathLength - strokeDashFrameList[i].Intervals[0];
            }

            return new HighlightPath(path, strokeDashFrameList);
        }
    }
}
