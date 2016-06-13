using System;
using System.Numerics;

namespace wenku8.Effects.P2DFlow.ForceFields
{
    class GlobalForceField : IForceField
    {
        private Vector2 a;

        public GlobalForceField( Vector2 a )
        {
            this.a = a;
        }

        public void Apply( Particle P )
        {
            P.a += a;
        }
    }
}
