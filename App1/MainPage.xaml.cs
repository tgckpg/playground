using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace App1
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

        }

        CanvasBitmap bitmapTiger;
        Vector2 grumpySize;

        PixelShaderEffect dissolveEffect;
        PixelShaderEffect rippleEffect;

        float offset = 0;
        float dissolve = 0;

        private void AnimaControl_CreateResources( CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args )
        {
            args.TrackAsyncAction( Canvas_CreateResourcesAsync( sender ).AsAsyncAction() );
        }

        async Task Canvas_CreateResourcesAsync( CanvasAnimatedControl sender )
        {
            bitmapTiger = await CanvasBitmap.LoadAsync( sender, "Assets/grumpy.jpg" );
            grumpySize = bitmapTiger.Size.ToVector2();

            // The Dissolve shader has two input textures:
            //
            //  - The first is an image that will be dissolved away to nothing.
            //
            //  - The second is a dissolve mask whose red channel controls the order in which pixels
            //    of the first image disappear as the dissolveAmount property is animated.
            //
            // This example selects different dissolve masks depending on the CurrentEffect.

            dissolveEffect = new PixelShaderEffect( await ReadAllBytes( "Shaders/Dissolve.bin" ) );

            // The Ripples shader has no input textures.
            // It generates an animatable series of concentric circles.
            // This is used as a mask input to the dissolveEffect.
            rippleEffect = new PixelShaderEffect( await ReadAllBytes( "Shaders/Ripples.bin" ) );

            rippleEffect.Properties[ "frequency" ] = 0.05f;
            rippleEffect.Properties[ "dpi" ] = sender.Dpi;
            rippleEffect.Properties[ "center" ] = grumpySize / 3;
        }

        private void AnimaControl_Update( ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args )
        {
        }

        private void AnimaControl_Draw( ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args )
        {
            var elapsedTime = ( float ) args.Timing.TotalTime.TotalSeconds;

            float t = ( float ) args.Timing.TotalTime.TotalMilliseconds;
            float d = 3000f;
            float dt = EaseOutCubic( t , d );

            // Center in the control.
            var position = ( sender.Size.ToVector2() - grumpySize ) / 2;
            position.Y -= grumpySize.Y * 0.5f;

            // Is the sketch effect enabled?
            ICanvasImage sourceImage = bitmapTiger;

            // Which dissolve mode are we currently displaying?
            ICanvasImage dissolveMask;

            // Use the custom rippleEffect as our dissolve mask, and animate its offset.
            dissolveMask = rippleEffect;

            rippleEffect.Properties[ "offset" ] = offset;

            // Animate the dissolve amount.
            dissolveEffect.Properties[ "dissolveAmount" ] = dissolve;

            // Draw the custom effect.
            dissolveEffect.Source1 = sourceImage;
            dissolveEffect.Source2 = dissolveMask;

            args.DrawingSession.DrawImage( dissolveEffect, position );

            if ( 3 < elapsedTime )
            {
                // sender.Paused = true;
            }
        }

        private float EaseOutCubic( float t, float d )
        {
            return ( ( t = t / d - 1 ) * t * t + 1 );
        }

        public static async Task<byte[]> ReadAllBytes( string filename )
        {
            var uri = new Uri( "ms-appx:///" + filename );
            var file = await StorageFile.GetFileFromApplicationUriAsync( uri );
            var buffer = await FileIO.ReadBufferAsync( file );

            return buffer.ToArray();
        }

        private void Slider_ValueChanged( object sender, RangeBaseValueChangedEventArgs e )
        {
            offset = ( float ) e.NewValue;
        }

        private void Slider_ValueChanged_1( object sender, RangeBaseValueChangedEventArgs e )
        {
            dissolve = ( float ) e.NewValue;
        }
    }
}