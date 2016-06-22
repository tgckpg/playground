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

        private int i = 0;

        public float Chaos = 1f;

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
            P.v = inVe - 2 * inVe * Ext.LFloat() * Chaos;
            P.Pos += Pos + Distrib * new Vector2( Ext.RFloat(), Ext.RFloat() );

            P.gf = 1;
            P.mf = 0.5f;
            float ot = 100.0f + 65.0f * Ext.LFloat();
            P.vt = new Vector2( ot, ot );
            P.Tint.M44 = 0;
        }

        public int Acquire( int Quota )
        {
            return ( i ++ ) % 10 == 0 ? 1 : 0;
        }
    }
}
