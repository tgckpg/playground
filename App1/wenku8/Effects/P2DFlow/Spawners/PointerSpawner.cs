using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;

using Net.Astropenguin.Helpers;

namespace wenku8.Effects.P2DFlow.Spawners
{
    class PointerSpawner : ISpawner
    {
        public float Chaos = 1.0f;

        private int i;

        public PFTrait SpawnTrait = PFTrait.NONE;

        private Stack<Vector2> PointerPos = new Stack<Vector2>();

        private Vector2 DrawPos;

        public void FeedPosition( Vector2 P )
        {
            lock ( this )
            {
                PointerPos.Push( P );
            }
        }

        public PointerSpawner() { }

        public int Acquire( int Quota )
        {
            lock ( this )
            {
                if ( 0 < PointerPos.Count )
                {
                    DrawPos = PointerPos.Pop();
                    return 1;
                }
                return 0;
            }
        }

        public void Prepare( IEnumerable<Particle> Ps )
        {
        }

        public void Spawn( Particle P )
        {
            P.Trait = SpawnTrait;
            P.ttl = 2;
            P.Pos = DrawPos;
        }
    }
}
