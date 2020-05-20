using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.ContentPatcher
{
    public class ConfigToken
    {
        public string Name { get; set; }
        public List<string> AllowValues { get; } = new List<string>();
        public bool AllowBlank { get; set; } = false;
        public bool AllowMultiple { get; set; } = false;
        public string Default { get; set; }
    }
}
