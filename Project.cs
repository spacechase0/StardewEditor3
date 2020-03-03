using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3
{
    public class Project
    {
        public const int LATEST_VERSION = 0;
        public int FormatVersion { get; set; } = LATEST_VERSION;

        public string Name { get; set; } = "My Project";
        public string Description { get; set; } = "...";
        public string UniqueId { get; set; } = "username.MyProject";
        public string Version { get; set; } = "1.0.0";
        public string Author { get; set; } = "username";
        public List<Dependency> Dependencies { get; } = new List<Dependency>();
        public List<UpdateKey> UpdateKeys { get; } = new List<UpdateKey>();

        public List<ModData> Mods { get; } = new List<ModData>();
    }
}
