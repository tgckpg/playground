using System.Numerics;

namespace wenku8.Effects.P2DFlow.ForceFields
{
    class PointSpawner : ISpawner
    {
        private Vector2 Pos;
        private Vector2 v;

        public float Chaos = 0.5f;

        public PointSpawner( Vector2 Pos, Vector2 InitVe )
        {
            this.Pos = Pos;
            v = InitVe;
        }

        public void Spawn( Particle P )
        {
            P.Pos += Pos + v * Ext.RFloat() * Chaos;
            P.f *= ( 1 - 0.5f * Ext.LFloat() );
        }
    }
}
