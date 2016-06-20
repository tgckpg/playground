using System;
using System.Numerics;

namespace wenku8.Effects.P2DFlow
{
    class Particle
    {
        public Vector2 Pos;
        public float ttl = 0;

        // Velocity
        public Vector2 v;

        // Acceleration
        public Vector2 a;

        // Mass factor
        public float mf = 0;

        // Gravity factor
        public float gf = 0;

        // Terminal velocity
        public Vector2 vt;

        public bool Immortal;
        public Vector2 loss;

        public Particle() { Reset(); }

        public void Reset()
        {
            gf = mf = ttl
                = Pos.X = Pos.Y
                = v.X = v.Y
                = a.X = a.Y = 0;

            vt.X = vt.Y = 1;

            loss.X = loss.Y = 0.995f;
            Immortal = true;
        }
    }
} 