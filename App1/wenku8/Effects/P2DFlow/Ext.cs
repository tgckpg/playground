using System;
using System.Numerics;
using Windows.Foundation;

namespace wenku8.Effects.P2DFlow
{
    static class Ext 
    {
        static Random R = new Random();
        public static bool Contains( this Rect R, Vector2 V )
        {
            return R.Contains( V.ToPoint() );
        }

        public static float RFloat()
        {
            return 2.0f * ( ( float ) R.NextDouble() ) - 1;
        }

        public static float LFloat()
        {
            return ( float ) R.NextDouble();
        }

    }
}
