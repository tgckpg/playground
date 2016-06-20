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

        // Mass
        public float m = 1.0f;

        // Terminal velocity
        public Vector2 vt;

        public bool Immortal;
        public Vector2 loss;

        public Particle() { Reset(); }

        public void Reset()
        {
            v *= 0f;
            a *= 0f;
            vt.X = vt.Y = 1;
            Pos *= 0f;
            loss.X = loss.Y = 0.995f;
            Immortal = true;

            ttl = 0;
        }
    }
} 