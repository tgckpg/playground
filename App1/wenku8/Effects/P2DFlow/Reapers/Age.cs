using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wenku8.Effects.P2DFlow.Reapers
{
    class Age : IReaper
    {
        private static Age _age = new Age();

        public static Age Instance { get { return _age; } }

        private Age() { }

        public bool Reap( Particle p )
        {
            if ( p.Immortal ) return false;

            return p.ttl <= 0;
        }
    }
}
