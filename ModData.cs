using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3
{
    public abstract class ModData
    {
        public string ContentPackFor { get; }
        
        protected ModData(string cpFor)
        {
            ContentPackFor = cpFor;
        }
    }
}
