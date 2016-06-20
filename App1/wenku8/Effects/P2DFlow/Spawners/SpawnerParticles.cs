﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace wenku8.Effects.P2DFlow.ForceFields
{
    class SpawnerParticle : ISpawner
    {
        // public Vector2 Distrib = Vector2.Zero;

        public float Chaos = 1.0f;

        public SpawnerParticle() { }

        private int i;
        private Particle[] pp;

        public void Prepare( IEnumerable<Particle> part )
        {
            i = 0;
            pp = part.Where( p => p.Immortal ).ToArray();
        }

        public int Acquire( int Quota )
        {
            return pp.Length;
        }

        public void Spawn( Particle P )
        {
            Particle OP = pp[ i++ ];

            P.Immortal = false;
            P.ttl = 30;

            P.v = -OP.v * Chaos * new Vector2( Ext.RFloat(), Ext.RFloat() );
            P.Pos = OP.Pos; // + Distrib * new Vector2( Ext.RFloat(), Ext.RFloat() );

            float ot = 100.0f + 5.0f * Ext.LFloat();
            P.vt = new Vector2( ot, 0 );
        }
    }
}
