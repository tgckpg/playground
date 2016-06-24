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

            // Load sample input patterns.
            Vector2[] inputs = new Vector2[] {
                new Vector2( 0.72f, 0.82f ), new Vector2( 0.91f, -0.69f ), new Vector2( 0.46f, 0.80f ),
                new Vector2( 0.03f, 0.93f ), new Vector2( 0.12f, 0.25f ), new Vector2( 0.96f, 0.47f ),
                new Vector2( 0.79f, -0.75f ), new Vector2( 0.46f, 0.98f ), new Vector2( 0.66f, 0.24f ),
                new Vector2( 0.72f, -0.15f ), new Vector2( 0.35f, 0.01f ), new Vector2( -0.16f, 0.84f ),
                new Vector2( -0.04f, 0.68f ), new Vector2( -0.11f, 0.10f ), new Vector2( 0.31f, -0.96f ),
                new Vector2( 0.00f, -0.26f ), new Vector2( -0.43f, -0.65f ), new Vector2( 0.57f, -0.97f ),
                new Vector2( -0.47f, -0.03f ), new Vector2( -0.72f, -0.64f ), new Vector2( -0.57f, 0.15f ),
                new Vector2( -0.25f, -0.43f ), new Vector2( 0.47f, -0.88f ), new Vector2( -0.12f, -0.90f ),
                new Vector2( -0.58f, 0.62f ), new Vector2( -0.48f, 0.05f ), new Vector2( -0.79f, -0.92f ),
                new Vector2( -0.42f, -0.09f ), new Vector2( -0.76f, 0.65f ), new Vector2( -0.77f, -0.76f )
            };

            // Load sample output patterns.
            int[] outputs = new int[] {
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

            int patternCount = inputs.GetUpperBound( 0 ) + 1;


            Perceptron p = new Perceptron();

            p.Train( inputs, outputs );
            // Display network generalisation.
            Debug.WriteLine( "" );
            Debug.WriteLine( "X, Y, Output" );
            for ( float x = -1; x <= 1; x += .5f )
            {
                for ( float y = -1; y <= 1; y += .5f )
                {
                    // Calculate output.
                    int output = p.Output( weights, x, y );
                    Debug.WriteLine( "{0}, {1}, {2}", x, y, ( output == 1 ) ? "Blue" : "Red" );
                }
            }
        }


        class Perceptron
        {
            public Perceptron()
            {
            }

            private void Train( IList<Vector2> Inputs, IList<int> Outputs )
            {
                // Randomise weights.
                float[] weights = { NTime.LFloat(), NTime.LFloat() };

                // Set learning rate.
                float learningRate = 0.1f;

                int iteration = 0;
                float globalError;
                int patternCount = Inputs.Count();

                do
                {
                    globalError = 0;
                    for ( int p = 0; p < patternCount; p++ )
                    {
                        // Calculate output.
                        int output = Output( weights, inputs[ p, 0 ], inputs[ p, 1 ] );

                        // Calculate error.
                        float localError = outputs[ p ] - output;

                        if ( localError != 0 )
                        {
                            // Update weights.
                            for ( int i = 0; i < 2; i++ )
                            {
                                weights[ i ] += learningRate * localError * inputs[ p, i ];
                            }
                        }

                        // Convert error to absolute value.
                        globalError += Math.Abs( localError );
                    }

                    Debug.WriteLine( "Iteration {0}\tError {1}", iteration, globalError );
                    iteration++;

                } while ( globalError != 0 );
            }

            private int Output( float[] weights, float x, float y )
            {
                float sum = x * weights[ 0 ] + y * weights[ 1 ];
                return ( sum >= 0 ) ? 1 : -1;
            }
        }

        class NTime
        {
            private static Random R = new Random();
            public static float LFloat()
            {
                return ( float ) R.NextDouble();
            }
            public static float RFloat()
            {
                return ( float ) ( 1 - 2 * R.NextDouble() );
            }
        }
    }
}
