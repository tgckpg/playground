﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using wenku8.Effects.P2DFlow.ForceFields;
using Windows.Foundation;

namespace wenku8.Effects.P2DFlow
{
    using Reapers;

    class PFSimulator
    {
        private int NumParticles = 0;

        private List<Particle> LifeParticles = new List<Particle>();
        private Stack<Particle> ParticleQueue = new Stack<Particle>();

        public List<ISpawner> Spawners = new List<ISpawner>();
        public List<IReaper> Reapers = new List<IReaper>();

        public List<IForceField> Fields = new List<IForceField>();

        private const float TimeFactor = 0.05f;

        public void Create( int Num )
        {
            for ( int i = 0; i < Num; i++ )
            {
                ParticleQueue.Push( new Particle() );
            }

            NumParticles += Num;
        }

        public void AddField( IForceField Field )
        {
            Fields.Add( Field );
        }

        private void ForceField( Particle P )
        {
            foreach ( IForceField Field in Fields )
            {
                Field.Apply( P );
            }
        }

        private void UpdateParticles()
        {
            foreach ( Particle P in LifeParticles )
            {
                ForceField( P );

                P.v += P.a;

                // Terminal Velocity
                P.v = Vector2.Min( P.v, P.vt );

                P.Pos += P.v * TimeFactor;

                P.a = Vector2.Zero;
                P.ttl--;
            }
        }

        private void ReapParticles()
        {
            int l = LifeParticles.Count;
            Particle[] Ps = LifeParticles.ToArray();

            ParticleReap:
            foreach ( Particle P in Ps )
            foreach ( IReaper Reaper in Reapers )
            {
                if ( Reaper.Reap( P ) )
                {
                    LifeParticles.Remove( P );
                    ParticleQueue.Push( P );
                    P.Reset();
                    goto ParticleReap;
                }
            }
        }

        private void SpawnParticles()
        {
            foreach ( ISpawner Spawner in Spawners )
            {
                Spawner.Prepare( LifeParticles );

                int l = Spawner.Acquire( ParticleQueue.Count );
                int i = 0;

                while ( 0 < ParticleQueue.Count && i++ < l )
                {
                    Particle P = ParticleQueue.Pop();
                    Spawner.Spawn( P );
                    LifeParticles.Add( P );
                }
            }
        }

        public IEnumerator<Particle> Snapshot()
        {
            UpdateParticles();
            ReapParticles();
            SpawnParticles();

            return LifeParticles.GetEnumerator();
        }
    }
}
