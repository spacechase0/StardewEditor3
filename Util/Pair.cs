using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.Util
{
    class Pair< T1, T2 >
    {
        public T1 First;
        public T2 Second;

        public Pair(T1 t1, T2 t2)
        {
            First = t1;
            Second = t2;
        }
    }
}
