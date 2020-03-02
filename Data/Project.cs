using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.Data
{
    public class Project
    {
        public List<Dependency> Dependencies { get; } = new List<Dependency>();
    }
}
