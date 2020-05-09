using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StardewEditor3.Util;
using System.Collections.Generic;

namespace StardewEditor3.JsonAssets.Data
{
    public class WeaponData : BaseDataWithTexture
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public enum Type_
        {
            Dagger,
            Club,
            Sword,
        }
        
        public string Description { get; set; }
        public Type_ Type { get; set; }

        public int MinimumDamage { get; set; }
        public int MaximumDamage { get; set; }
        public double Knockback { get; set; }
        public int Speed { get; set; }
        public int Accuracy { get; set; }
        public int Defense { get; set; }
        public int MineDropVar { get; set; }
        public int MineDropMinimumLevel { get; set; }
        public int ExtraSwingArea { get; set; }
        public double CritChance { get; set; }
        public double CritMultiplier { get; set; }

        [DoNotAutoConnect]
        public bool CanPurchase { get; set; } = false;
        public int PurchasePrice { get; set; }
        public string PurchaseFrom { get; set; } = "Pierre";
        public List<string> PurchaseRequirements { get; set; } = new List<string>();

        public bool CanTrash { get; set; } = true;

        public Dictionary<string, string> NameLocalization { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> DescriptionLocalization { get; set; } = new Dictionary<string, string>();
    }
}
