using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3
{
    public class Dependency
    {
        public string UniqueID { get; set; }
        public bool IsRequired { get; set; } = true;
        public string MinimumVersion { get; set; }
    }
}
