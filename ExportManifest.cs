using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3
{
    public class ExportContentPackFor
    {
        public string UniqueID { get; set; }
        public string MinimumVersion { get; set; }
    }

    public class ExportManifest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Version { get; set; }
        public string MinimumApiVersion { get; set; } = "3.0.0";
        public string UniqueID { get; set; }
        // EntryDll
        public ExportContentPackFor ContentPackFor { get; set; }
        public List<Dependency> Dependencies { get; set; } = new List<Dependency>();
        public List<string> UpdateKeys { get; set; } = new List<string>();
        // extra fields?
    }
}
