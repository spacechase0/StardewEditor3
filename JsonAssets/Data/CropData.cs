﻿using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StardewEditor3;
using StardewEditor3.Util;
using System.Collections.Generic;

namespace StardewEditor3.JsonAssets.Data
{
    public class CropData : BaseDataWithTexture
    {
        public ImageResourceReference SeedTexture { get; set; }
        public ImageResourceReference GiantTexture { get; set; }

        public object Product { get; set; }
        public string SeedName { get; set; }
        public string SeedDescription { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum CropType_
        {
            Normal,
            IndoorsOnly,
            Paddy,
        }
        public CropType_ CropType { get; set; } = CropType_.Normal;

        public List<string> Seasons { get; set; } = new List<string>();
        public List<int> Phases { get; set; } = new List<int>();
        public int RegrowthPhase { get; set; } = -1;
        public bool HarvestWithScythe { get; set; } = false;
        public bool TrellisCrop { get; set; } = false;
        public List<Color> Colors { get; set; } = new List<Color>();
        public class Bonus_
        {
            public int MinimumPerHarvest { get; set; }
            public int MaximumPerHarvest { get; set; }
            public int MaxIncreasePerFarmLevel { get; set; }
            public double ExtraChance { get; set; }
        }
        public Bonus_ Bonus { get; set; } = null;

        public List<string> SeedPurchaseRequirements { get; set; } = new List<string>();
        public int SeedPurchasePrice { get; set; }
        public int SeedSellPrice { get; set; } = -1;
        public string SeedPurchaseFrom { get; set; } = "Pierre";
        
        public Dictionary<string, string> SeedNameLocalization { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SeedDescriptionLocalization { get; set; } = new Dictionary<string, string>();
    }
}
