using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

            Vector2[][] Data = new Vector2[][] {
                new Vector2[] // 0
                {
                    new Vector2( 1, 1 ), new Vector2( 1, 1 ), new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 0, 0 ), new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 0, 0 ) , new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 0, 0 ) , new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 1, 1 ) , new Vector2( 1, 1 )
                }

                , new Vector2[] // 1
                {
                    new Vector2( 0, 0 ), new Vector2( 1, 1 ), new Vector2( 0, 0 )
                    , new Vector2( 0, 0 ), new Vector2( 1, 1 ), new Vector2( 0, 0 )
                    , new Vector2( 0, 0 ), new Vector2( 1, 1 ) , new Vector2( 0, 0 )
                    , new Vector2( 0, 0 ), new Vector2( 1, 1 ) , new Vector2( 0, 0 )
                    , new Vector2( 0, 0 ), new Vector2( 1, 1 ) , new Vector2( 0, 0 )
                }
                , new Vector2[] // 2
                {
                    new Vector2( 1, 1 ), new Vector2( 1, 1 ), new Vector2( 1, 1 )
                    , new Vector2( 0, 0 ), new Vector2( 0, 0 ), new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 1, 1 ) , new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 0, 0 ) , new Vector2( 0, 0 )
                    , new Vector2( 1, 1 ), new Vector2( 1, 1 ) , new Vector2( 1, 1 )
                }
                , new Vector2[] // 3
                {
                    new Vector2( 1, 1 ), new Vector2( 1, 1 ), new Vector2( 1, 1 )
                    , new Vector2( 0, 0 ), new Vector2( 0, 0 ), new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 1, 1 ) , new Vector2( 1, 1 )
                    , new Vector2( 0, 0 ), new Vector2( 0, 0 ) , new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 1, 1 ) , new Vector2( 1, 1 )
                }
                , new Vector2[] // 4
                {
                    new Vector2( 1, 1 ), new Vector2( 0, 0 ), new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 0, 0 ), new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 1, 1 ) , new Vector2( 1, 1 )
                    , new Vector2( 0, 0 ), new Vector2( 0, 0 ) , new Vector2( 1, 1 )
                    , new Vector2( 0, 0 ), new Vector2( 0, 0 ) , new Vector2( 1, 1 )
                }
                , new Vector2[] // 5
                {
                    new Vector2( 1, 1 ), new Vector2( 1, 1 ), new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 0, 0 ), new Vector2( 0, 0 )
                    , new Vector2( 1, 1 ), new Vector2( 1, 1 ) , new Vector2( 1, 1 )
                    , new Vector2( 0, 0 ), new Vector2( 0, 0 ) , new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 1, 1 ) , new Vector2( 1, 1 )
                }
                , new Vector2[] // 6
                {
                    new Vector2( 1, 1 ), new Vector2( 1, 1 ), new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 0, 0 ), new Vector2( 0, 0 )
                    , new Vector2( 1, 1 ), new Vector2( 1, 1 ) , new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 0, 0 ) , new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 1, 1 ) , new Vector2( 1, 1 )
                }
                , new Vector2[] // 7
                {
                    new Vector2( 1, 1 ), new Vector2( 1, 1 ), new Vector2( 1, 1 )
                    , new Vector2( 0, 0 ), new Vector2( 0, 0 ), new Vector2( 1, 1 )
                    , new Vector2( 0, 0 ), new Vector2( 0, 0 ) , new Vector2( 1, 1 )
                    , new Vector2( 0, 0 ), new Vector2( 0, 0 ) , new Vector2( 1, 1 )
                    , new Vector2( 0, 0 ), new Vector2( 0, 0 ) , new Vector2( 1, 1 )
                }
                , new Vector2[] // 8
                {
                    new Vector2( 1, 1 ), new Vector2( 1, 1 ), new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 0, 0 ), new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 1, 1 ) , new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 0, 0 ) , new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 1, 1 ) , new Vector2( 1, 1 )
                }
                , new Vector2[] // 9
                {
                    new Vector2( 1, 1 ), new Vector2( 1, 1 ), new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 0, 0 ), new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 1, 1 ) , new Vector2( 1, 1 )
                    , new Vector2( 0, 0 ), new Vector2( 0, 0 ) , new Vector2( 1, 1 )
                    , new Vector2( 1, 1 ), new Vector2( 1, 1 ) , new Vector2( 1, 1 )
                }
            };

            Perceptron2 p = new Perceptron2();
            int i = 0;
            foreach( Vector2[] Sample in Data )
            {
                p.Train( Sample, i == 6 ? 1 : 0 );
            }


            foreach( Vector2[] Sample in Data )
            {
                Debug.WriteLine( p.Output( Sample ) );
            }
        }

        class Perceptron2
        {
            public List<Vector2> Weights = new List<Vector2>();
            int Acq = 0;
            int NumWeight = 1;

            public Perceptron2()
            {
                // Bias
                Weights.Add( new Vector2( 0.67957f, 0.67957f ) );
            }

            public float Output( IList<Vector2> InputVector )
            {
                Vector2 Output = Vector2.Zero;

                foreach( Vector2 x in InputVector )
                {
                     Output += x * AcquireWeight();
                }

                return 0.5f * ( Output.X + Output.Y );
            }

            public void Train( IList<Vector2> D, float d )
            {
                Acq = 0;
                Vector2 Output = Vector2.Zero;

                Vector2 dOut = new Vector2( d, d );

                foreach( Vector2 x in D )
                {
                     Output += x * AcquireWeight();
                }

                int l = Math.Min( NumWeight, D.Count ) - 1;
                for( int i = 0; i < l; i ++ )
                {
                    Weights[ i + 1 ] = Weights[ i ] + ( dOut - Output ) * D[ i ];
                }
            }

            private Vector2 AcquireWeight()
            {
                if ( Acq < NumWeight )
                {
                    return Weights[ Acq++ ];
                }

                Weights.Add( new Vector2( NTime.RFloat(), NTime.RFloat() ) );
                NumWeight++;
                return Weights[ Acq++ ];
            }
        }

        class NTime
        {
            private static Random R = new Random();
            public static float RFloat()
            {
                return ( float ) ( 1 - 2 * R.NextDouble() );
            }
        }
    }
}
