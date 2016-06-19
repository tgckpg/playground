using System;
using System.Numerics;

namespace wenku8.Effects.P2DFlow
{
    class Particle
    {
        public Vector2 Pos;
        public int ttl = 0;

        // Velocity
        public Vector2 v;

        // Acceleration
        public Vector2 a;

        // Mass
        public float m = 1.0f;

        // Terminal velocity
        public Vector2 vt;

        public bool Immortal;

        public Particle() { Reset(); }

        public void Reset()
        {
            v *= 0f;
            a *= 0f;
            vt.X = vt.Y = 1;
            Pos *= 0f;
            Immortal = true;

            ttl = 0;
        }
    }
} 