using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StardewEditor3.Util;
using System;
using System.Collections.Generic;

namespace StardewEditor3.JsonAssets.Data
{
    public class BootsData : BaseDataWithTexture
    {
        [JsonIgnore]
        public Color[] ColorPalette { get; set; } = new Color[4];
        
        public string Description { get; set; }

        public int Price { get; set; }
        
        [DoNotAutoConnect]
        public bool CanPurchase { get; set; } = false;
        public int PurchasePrice { get; set; }
        public string PurchaseFrom { get; set; } = "Marlon";
        public List<string> PurchaseRequirements { get; set; } = new List<string>();

        public Dictionary<string, string> NameLocalization { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> DescriptionLocalization { get; set; } = new Dictionary<string, string>();
        
        public int Defense { get; set; }
        public int Immunity { get; set; }
    }
}
