﻿using Godot;
using Newtonsoft.Json;
using StardewEditor3;
using StardewEditor3.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.JsonAssets.Data
{
    public class ClothingData : BaseData
    {
        public ImageResourceReference MaleTexture { get; set; }
        public ImageResourceReference FemaleTexture { get; set; }

        public string Description { get; set; }
        [DoNotAutoConnect]
        public bool HasFemaleVariant { get; set; } = false;

        public int Price { get; set; }

        public Color DefaultColor { get; set; } = new Color(255, 235, 203);
        public bool Dyeable { get; set; } = false;

        public string Metadata { get; set; } = "";

        public Dictionary<string, string> NameLocalization { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> DescriptionLocalization { get; set; } = new Dictionary<string, string>();
    }
}
