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
using Microsoft.Graphics.Canvas.Effects;

using wenku8.Effects.P2DFlow;
using wenku8.Effects.P2DFlow.ForceFields;
using wenku8.Effects.P2DFlow.Reapers;

namespace App1
{
    public sealed partial class ParticleField : UserControl
    {
        static Random Rand = new Random();

        private PFSimulator PFSim = new PFSimulator();
        private CanvasBitmap pNote;

        private bool ShowWireFrame = true;

        private ColorMatrixEffect TintEffect;

        public ParticleField()
        {
            this.InitializeComponent();
            SetTemplate();
        }

        private void SetTemplate()
        {
            PFSim.Create( 300 );
            PFSim.AddField( GenericForce.EARTH_GRAVITY );
            PFSim.AddField( new Wind() { A = new Vector2( 30, 350 ), B = new Vector2( 600, 300 ) } );
            PFSim.AddField( new Wind() { A = new Vector2( 700, 0 ), B = new Vector2( 700, 600 ), MaxDist = 500.0f } );
            PFSim.AddField( new Wind() { A = new Vector2( 0, 200 ), B = new Vector2( 100, 550 ), Strength = 50f } );

            TintEffect = new ColorMatrixEffect();
            TintEffect.BufferPrecision = CanvasBufferPrecision.Precision8UIntNormalized;
        }

        private void Stage_CreateResources( CanvasAnimatedControl sender, CanvasCreateResourcesEventArgs args )
        {
            args.TrackAsyncAction( LoadTextures( sender ).AsAsyncAction() );
        }

        private async Task LoadTextures( CanvasAnimatedControl CC )
        {
            pNote = await CanvasBitmap.LoadAsync( CC, "Assets/plus.dds" );
            TintEffect.Source = pNote;
            PBounds = pNote.Bounds;
        }

        private void Stage_SizeChanged( object sender, SizeChangedEventArgs e )
        {
            lock ( PFSim )
            {
                Size s = e.NewSize;
                PFSim.Reapers.Clear();
                PFSim.Reapers.Add( Age.Instance );
                PFSim.Reapers.Add( new Boundary( new Rect( 0, 0, s.Width * 1.2, s.Height * 1.2 ) ) );

                float HSW = ( float ) s.Width / 2;
                PFSim.Spawners.Clear();
                PFSim.Spawners.Add( new SpawnerParticle() );
                PFSim.Spawners.Add( new PointSpawner( new Vector2( HSW, 0 ), new Vector2( HSW, 0 ), new Vector2( 0, 50 ) ) );
            }
        }

        private void Stage_Update( ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args )
        {

        }

        private Vector2 PCenter = new Vector2( 16, 16 );
        private Rect PBounds;
        private Vector2 PScale = Vector2.One;

        private void Stage_Draw( ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args )
        {
            lock ( PFSim )
            {
                var Snapshot = PFSim.Snapshot();
                using ( CanvasDrawingSession ds = args.DrawingSession )
                using ( CanvasSpriteBatch SBatch = ds.CreateSpriteBatch() )
                {
                    while ( Snapshot.MoveNext() )
                    {
                        Particle P = Snapshot.Current;

                        float A = P.Immortal ? 1 : P.ttl * 0.033f;

                        P.Tint.M12 = 1 - A;
                        P.Tint.M21 = A;
                        TintEffect.ColorMatrix = P.Tint;

                        ds.DrawImage( TintEffect, P.Pos.X - PCenter.X, P.Pos.Y - PCenter.Y, PBounds, A, CanvasImageInterpolation.Linear, CanvasComposite.Add );

                        // SBatch.Draw( pNote, P.Pos, new Vector4( P.Tint.M11, P.Tint.M22, P.Tint.M33, A ), PCenter, 0, PScale, CanvasSpriteFlip.None );
                    }

                    if ( ShowWireFrame )
                    {
                        foreach ( IForceField IFF in PFSim.Fields )
                        {
                            Vector2 prev = Vector2.Zero;
                            IFF.WireFrame( ds );
                        }
                    }
                }
            }
        }

    }
}
