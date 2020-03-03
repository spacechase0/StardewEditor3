﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3
{
    public class Project
    {
        public string Name { get; set; } = "My Project";
        public string Description { get; set; } = "...";
        public string UniqueId { get; set; } = "username.MyProject";
        public string Version { get; set; } = "1.0.0";
        public string Author { get; set; } = "username";
        public List<Dependency> Dependencies { get; } = new List<Dependency>();
        public Dictionary<string, int> UpdateKeys { get; } = new Dictionary<string, int>();

        public List<ModData> Mods { get; } = new List<ModData>();
    }
}