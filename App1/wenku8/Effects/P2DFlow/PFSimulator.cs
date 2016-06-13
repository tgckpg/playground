using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using wenku8.Effects.P2DFlow.ForceFields;
using Windows.Foundation;

namespace wenku8.Effects.P2DFlow
{
    class PFSimulator
    {
        private int NumParticles = 0;

        private List<Particle> LifeParticles = new List<Particle>();
        private Stack<Particle> ParticleQueue = new Stack<Particle>();

        public ISpawner Spawner = new PointSpawner( new Vector2(), new Vector2() );

        private Rect Bounds = new Rect();

        private List<IForceField> Fields = new List<IForceField>();

        private const float TimeFactor = 0.02f;

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

        public void Boundary( Rect Bounds )
        {
            this.Bounds = Bounds;
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
                P.v *= P.f;

                P.Pos += P.v * TimeFactor;

                P.ttl--;
            }
        }

        private void ReapParticles()
        {
            int l = LifeParticles.Count;
            Particle[] Reaped = LifeParticles.Where( NotInBounds ).ToArray();

            foreach ( Particle P in Reaped )
            {
                LifeParticles.Remove( P );
                ParticleQueue.Push( P );
                P.Reset();
            }
        }

        private void SpawnParticles()
        {
            int l = 10;
            int i = 0;

            while ( 0 < ParticleQueue.Count && i++ < l )
            {
                Particle P = ParticleQueue.Pop();
                Spawner.Spawn( P );
                LifeParticles.Add( P );
            }
        }

        private bool NotInBounds( Particle arg )
        {
            return !Bounds.Contains( arg.Pos );
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
