using System;
using System.Collections.Generic;
using System.Numerics;

namespace wenku8.Effects.P2DFlow.ForceFields
{
    class PointSpawner : ISpawner
    {
        private Vector2 Pos;
        private Vector2 Distrib;
        private Vector2 inVe;

        public float Chaos = 0.5f;

        public PointSpawner() { }

        public PointSpawner( Vector2 Pos, Vector2 Distrib, Vector2 InitVe )
        {
            this.Pos = Pos;
            this.Distrib = Distrib;
            inVe = InitVe;
        }

        public void Prepare( IEnumerable<Particle> currParticles ) { }

        public void Spawn( Particle P )
        {
            P.v = inVe * Ext.RFloat() * Chaos;
            P.Pos += Pos + Distrib * new Vector2( Ext.RFloat(), Ext.RFloat() );

            float ot = 100.0f + 15.0f * Ext.LFloat();
            P.vt = new Vector2( ot, ot );
        }

        public int Acquire( int Quota )
        {
            return ( int ) Math.Ceiling( 0.1 * Quota );
        }
    }
}
