using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Input.Inking;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace ExampleGallery
{
    public sealed partial class InkExample : UserControl
    {
        bool needToCreateSizeDependentResources;
        float strokeThickness = 16;

        CanvasRenderTarget[] accumulationBuffers = new CanvasRenderTarget[ 2 ];
        int currentBuffer = 0;

        private CanvasImageBrush imageBrush;
        private TouchPointsRenderer TouchPointR;

        GaussianBlurEffect InkGlow;
        GaussianBlurEffect InkDissolve;
        Transform2DEffect OutputEffect;


        public InkExample()
        {
            this.InitializeComponent();

            InkGlow = new GaussianBlurEffect()
            {
                BlurAmount = 3 * strokeThickness,
                BorderMode = EffectBorderMode.Soft
            };

            canvasControl.PointerPressed += CanvasControl_PointerPressed;
            canvasControl.PointerMoved += CanvasControl_PointerMoved;
            canvasControl.PointerReleased += CanvasControl_PointerReleased;

            InkDissolve = new GaussianBlurEffect() { BlurAmount = 1 };
            OutputEffect = new Transform2DEffect() { Source = InkDissolve };
            TouchPointR = new TouchPointsRenderer();

            needToCreateSizeDependentResources = true;
        }

        private void canvasControl_CreateResources( ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args )
        {
            CreateSizeDependentResources();
        }

        private void CreateSizeDependentResources()
        {
            imageBrush = new CanvasImageBrush( canvasControl );
            imageBrush.Opacity = 0.8f;

            needToCreateSizeDependentResources = false;
        }




        private void CanvasControl_PointerMoved( object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e )
        {
            lock ( TouchPointR )
            {
                TouchPointR.OnPointerMoved( e.GetCurrentPoint( canvasControl ) );
            }

            canvasControl.Invalidate();
        }

        private void CanvasControl_PointerPressed( object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e )
        {
            lock ( TouchPointR )
            {
                TouchPointR.OnPointerPressed( e.GetCurrentPoint( canvasControl ) );
            }

            canvasControl.Invalidate();
        }

        private void CanvasControl_PointerReleased( object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e )
        {
            lock ( TouchPointR )
            {
                TouchPointR.OnPointerReleased( e.GetCurrentPoint( canvasControl ) );
            }

            canvasControl.Invalidate();
        }

        private void DrawLink( CanvasDrawingSession ds, IReadOnlyList<InkStroke> strokes )
        {
            using ( CanvasCommandList cdl = new CanvasCommandList( canvasControl ) )
            using ( CanvasDrawingSession cds = cdl.CreateDrawingSession() )
            {
                foreach ( var stroke in strokes )
                {
                    var inkPoints = stroke.GetInkPoints().Select( point => point.Position.ToVector2() ).ToList();

                    for ( int i = 1; i < inkPoints.Count; i++ )
                    {
                        cds.DrawLine( inkPoints[ i - 1 ], inkPoints[ i ], Colors.Yellow, 1.5f * strokeThickness );
                        ds.DrawLine( inkPoints[ i - 1 ], inkPoints[ i ], Colors.White, strokeThickness );
                    }
                }

                InkGlow.Source = cdl;
                ds.DrawImage( InkGlow );
            }

            // ds.DrawInk( strokes );
        }

        CanvasRenderTarget FrontBuffer { get { return accumulationBuffers[ currentBuffer ]; } }
        CanvasRenderTarget BackBuffer { get { return accumulationBuffers[ ( currentBuffer + 1 ) % 2 ]; } }

        private void canvasControl_Draw( ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args )
        {
            if ( needToCreateSizeDependentResources )
            {
                CreateSizeDependentResources();
            }

            // SwapAccumulationBuffers
            currentBuffer = ( currentBuffer + 1 ) % 2;
            EnsureCurrentBufferMatches();

            using ( CanvasDrawingSession ds = FrontBuffer.CreateDrawingSession() )
            {
                ds.Clear( Colors.Transparent );
                AccumulateBackBufferOntoFrontBuffer( ds );

                lock ( TouchPointR )
                {
                    TouchPointR.Draw( ds );
                }
            }

            args.DrawingSession.DrawImage( FrontBuffer );
        }

        private void canvasControl_Update( ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args )
        {
        }

        void EnsureCurrentBufferMatches()
        {
            Size canvasSize = canvasControl.Size;
            float dpi = canvasControl.Dpi;

            var buffer = accumulationBuffers[ currentBuffer ];

            if ( buffer == null || !( SizeEqualsWithTolerance( buffer.Size, canvasSize ) ) || buffer.Dpi != dpi )
            {
                if ( buffer != null )
                    buffer.Dispose();

                buffer = new CanvasRenderTarget( canvasControl, ( float ) canvasSize.Width, ( float ) canvasSize.Height, dpi );
                accumulationBuffers[ currentBuffer ] = buffer;
            }
        }

        public bool SizeEqualsWithTolerance( Size sizeA, Size sizeB )
        {
            const float tolerance = 0.1f;

            if ( Math.Abs( sizeA.Width - sizeB.Width ) > tolerance )
                return false;

            if ( Math.Abs( sizeA.Height - sizeB.Height ) > tolerance )
                return false;

            return true;
        }

        void AccumulateBackBufferOntoFrontBuffer( CanvasDrawingSession ds )
        {
            // If this is the first frame then there's no back buffer
            if ( BackBuffer == null )
                return;

            InkDissolve.Source = BackBuffer;

            // Adjust the scale, so that if the front and back buffer are different sizes (eg the window was resized) 
            // then the contents is scaled up as appropriate.
            var scaleX = FrontBuffer.Size.Width / BackBuffer.Size.Width;
            var scaleY = FrontBuffer.Size.Height / BackBuffer.Size.Height;

            imageBrush.Image = OutputEffect;
            imageBrush.SourceRectangle = new Rect( 0, 0, FrontBuffer.Size.Width, FrontBuffer.Size.Height );
            ds.FillRectangle( imageBrush.SourceRectangle.Value, imageBrush );
        }

        private void Canvas_SizeChanged( object sender, SizeChangedEventArgs e )
        {
            needToCreateSizeDependentResources = true;
            canvasControl.Invalidate();
        }

        private void control_Unloaded( object sender, RoutedEventArgs e )
        {
            // Explicitly remove references to allow the Win2D controls to get garbage collected
            canvasControl.RemoveFromVisualTree();
            canvasControl = null;
        }

        private class ContinuousSlope
        {
            private Vector2[] Points = new Vector2[ 2 ];

            public float Slope { get; private set; }

            public bool SlopeChanged = false;

            public void FeedPoint( Vector2 P )
            {
                Points[ 1 ] = Points[ 0 ];
                Points[ 0 ] = P;

                float OSlope = Slope;
                Slope = ( A.Y - B.Y ) / ( A.X - B.X );

                if ( ( Slope == 0 || OSlope == 0 ) && Math.Abs( OSlope - Slope ) < 0.2f )
                {
                    SlopeChanged = false;
                    return;
                }

                SlopeChanged =
                    ( Slope < 0 && 0 < OSlope )
                    || ( OSlope < 0 && 0 < Slope )
                    || ( OSlope == 0 && 0 != Slope )
                    || ( OSlope != 0 && 0 == Slope )
                    ;
            }

            Vector2 A { get { return Points[ 0 ]; } }
            Vector2 B { get { return Points[ 1 ]; } }
        }

        class TouchPointsRenderer
        {
            ContinuousSlope CSlope = new ContinuousSlope();
            Queue<Tuple<Vector2, bool>> points = new Queue<Tuple<Vector2, bool>>();

            const int maxPoints = 20;
            const int DqRate = 1;
            int DqIndex = 0;

            bool Reg = false;
            Vector2 CurrPoint;

            public void OnPointerPressed( PointerPoint point )
            {
                CurrPoint = point.Position.ToVector2();
                Reg = true;
            }

            public void OnPointerReleased( PointerPoint point )
            {
                RegPoint( point.Position.ToVector2() );
                Reg = false;
            }

            public void OnPointerMoved( PointerPoint point )
            {
                if ( Reg ) RegPoint( point.Position.ToVector2() );
            }

            private void RegPoint( Vector2 p )
            {
                if ( Vector2.Distance( p, CurrPoint ) < 20 ) return;
                CurrPoint = p;

                if ( points.Count > maxPoints )
                {
                    points.Dequeue();
                }

                CSlope.FeedPoint( p );

                points.Enqueue( new Tuple<Vector2, bool>( p, CSlope.SlopeChanged ) );
            }

            public void Draw( CanvasDrawingSession ds )
            {
                int pointerPointIndex = 0;

                Vector2 prev = new Vector2( 0, 0 );
                const float penRadius = 1;

                foreach ( Tuple<Vector2, bool> t in points )
                {
                    float penFactor = t.Item2 ? 12 : 1;
                    float penr = penRadius * penFactor;
                    Vector2 p = t.Item1;

                    if ( pointerPointIndex != 0 )
                    {
                        ds.DrawLine( prev, p, Colors.White, penRadius );
                    }

                    ds.DrawEllipse( p, 6 * penRadius, 6 * penRadius, Colors.White );
                    ds.DrawEllipse( prev, penr, penr, t.Item2 ? Colors.Red : Colors.White );

                    prev = p;
                    pointerPointIndex++;
                }

                if ( 0 < points.Count && DqRate < DqIndex++ )
                {
                    DqIndex = 0;
                    points.Dequeue();
                }

            }
        }
    }
}
