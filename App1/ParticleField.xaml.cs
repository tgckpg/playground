using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using wenku8.Effects.P2DFlow;
using wenku8.Effects.P2DFlow.ForceFields;

namespace App1
{
    public sealed partial class ParticleField : UserControl
    {
        static Random Rand = new Random();

        private PFSimulator PFSim = new PFSimulator();
        private CanvasBitmap pNote;

        public ParticleField()
        {
            this.InitializeComponent();
        }

        private void Stage_CreateResources( CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args )
        {
            PFSim.Create( 100 );
            PFSim.AddField( GenericForce.EARTH_GRAVITY );

            args.TrackAsyncAction( LoadTextures( sender ).AsAsyncAction() );
        }

        private async Task LoadTextures( CanvasAnimatedControl CC )
        {
            pNote = await CanvasBitmap.LoadAsync( CC, "Assets/plus.dds" );
        }

        private void Stage_SizeChanged( object sender, SizeChangedEventArgs e )
        {
            Size s = e.NewSize;
            PFSim.Boundary( new Rect( 0, 0, s.Width, s.Height ) );
            PFSim.Spawner = new PointSpawner( new Vector2( ( float ) s.Width / 2, 100 ), new Vector2( 100, 0 ) );
        }

        private void Stage_Update( ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args )
        {

        }

        private Vector4 PTint = new Vector4( 1, 1, 1, 1 );
        private Vector2 PCenter = new Vector2( 16, 16 );
        private Vector2 PScale = new Vector2( 1, 1 );

        private void Stage_Draw( ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args )
        {
            var Snapshot = PFSim.Snapshot();
            using ( CanvasSpriteBatch ds = args.DrawingSession.CreateSpriteBatch() )
            {
                while ( Snapshot.MoveNext() )
                {
                    ds.Draw( pNote, Snapshot.Current.Pos, PTint, PCenter, 0, PScale, CanvasSpriteFlip.None );
                }
            }
        }
    }
}
