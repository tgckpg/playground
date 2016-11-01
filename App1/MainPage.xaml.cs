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

        CanvasBitmap bmpImage;
        Vector2 grumpySize;

        PixelShaderEffect dissolveEffect;
        PixelShaderEffect rippleEffect;

        private void AnimaControl_CreateResources( CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args )
        {
            args.TrackAsyncAction( Canvas_CreateResourcesAsync( sender ).AsAsyncAction() );
        }

        async Task Canvas_CreateResourcesAsync( CanvasAnimatedControl sender )
        {
            bmpImage = await CanvasBitmap.LoadAsync( sender, "Assets/grumpy.jpg" );
            grumpySize = bmpImage.Size.ToVector2();

            // See Win2D custom effect example
            dissolveEffect = new PixelShaderEffect( await ReadAllBytes( "Shaders/Dissolve.bin" ) );
            rippleEffect = new PixelShaderEffect( await ReadAllBytes( "Shaders/Ripples.bin" ) );

            rippleEffect.Properties[ "dpi" ] = sender.Dpi;
            rippleEffect.Properties[ "center" ] = grumpySize / 2;

            dissolveEffect.Properties[ "dissolveAmount" ] = 0.5f;
        }

        private void AnimaControl_Update( ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args )
        {
        }

        private volatile bool Restarted = false;
        private volatile float pt = 0;

        private void AnimaControl_Draw( ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args )
        {
            float t = ( float ) args.Timing.TotalTime.TotalMilliseconds;

            if( Restarted )
            {
                Restarted = false;
                pt = t;
            }

            t -= pt;

            // Center in the control.
            Vector2 position = ( sender.Size.ToVector2() - grumpySize ) / 2;
            position.Y -= grumpySize.Y * 0.5f;

            rippleEffect.Properties[ "t1" ] = EaseOutCubic( t, 1500 );
            rippleEffect.Properties[ "t2" ] = EaseOutCubic( t, 1900 );

            // Draw the custom effect.
            dissolveEffect.Source1 = bmpImage;
            dissolveEffect.Source2 = rippleEffect;

            args.DrawingSession.DrawImage( dissolveEffect, position );

            if ( 2000 < t ) sender.Paused = true;
        }

        private float EaseOutCubic( float t, float d )
        {
            if ( d <= t ) return 1;
            return ( ( t = t / d - 1 ) * t * t * t * t + 1 );
        }

        public static async Task<byte[]> ReadAllBytes( string filename )
        {
            var uri = new Uri( "ms-appx:///" + filename );
            var file = await StorageFile.GetFileFromApplicationUriAsync( uri );
            var buffer = await FileIO.ReadBufferAsync( file );

            return buffer.ToArray();
        }

        private void Button_Click( object sender, RoutedEventArgs e )
        {
            AnimaControl.Paused = false;
            Restarted = true;
        }
    }
}