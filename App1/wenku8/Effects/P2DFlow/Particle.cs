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

        // Friction
        public Vector2 f;

        public Particle() { Reset(); }

        public void Reset()
        {
            v *= 0f;
            a *= 0f;
            f.X = f.Y = 1;
            Pos *= 0f;

            ttl = 0;
        }
    }
}
