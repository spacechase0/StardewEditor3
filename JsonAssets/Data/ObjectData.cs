using Godot;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StardewEditor3;
using StardewEditor3.JsonAssets.Data;
using StardewEditor3.Util;
using System.Collections.Generic;

namespace StardewEditor3.JsonAssets.Data
{
    public class ObjectData : BaseDataWithTexture
    {
        public ImageResourceReference TextureColor { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public enum Category_
        {
            Vegetable,
            Fruit,
            Flower,
            Gem,
            Fish,
            Egg,
            Milk,
            Cooking,
            Crafting,
            Mineral,
            Meat,
            Metal,
            Junk,
            Syrup,
            MonsterLoot,
            ArtisanGoods,
            Seeds,
            Ring,
            AnimalGoods,
            Greens,
            Artifact,
        }

        public class FoodBuffs_
        {
            public int Farming { get; set; } = 0;
            public int Fishing { get; set; } = 0;
            public int Mining { get; set; } = 0;
            public int Luck { get; set; } = 0;
            public int Foraging { get; set; } = 0;
            public int MaxStamina { get; set; } = 0;
            public int MagnetRadius { get; set; } = 0;
            public int Speed { get; set; } = 0;
            public int Defense { get; set; } = 0;
            public int Attack { get; set; } = 0;
            public int Duration { get; set; } = 0;
        }

        public string Description { get; set; }
        public Category_ Category { get; set; }
        public string CategoryTextOverride { get; set; }
        public Color? CategoryColorOverride { get; set; } = new Color(0, 0, 0, 0);
        [DoNotAutoConnect]
        public bool IsColored { get; set; } = false;

        public int Price { get; set; }

        public bool CanTrash { get; set; } = true;
        [DoNotAutoConnect]
        public bool CanSell { get; set; } = true;
        public bool CanBeGifted { get; set; } = true;

        public RecipeData Recipe { get; set; }

        public int Edibility { get; set; } = -300;
        public bool EdibleIsDrink { get; set; } = false;
        public FoodBuffs_ EdibleBuffs { get; set; } = new FoodBuffs_();

        [DoNotAutoConnect]
        public bool CanPurchase { get; set; } = false;
        public int PurchasePrice { get; set; }
        public string PurchaseFrom { get; set; } = "Pierre";
        public List<string> PurchaseRequirements { get; set; } = new List<string>();

        public class GiftTastes_
        {
            public List<string> Love { get; set; } = new List<string>();
            public List<string> Like { get; set; } = new List<string>();
            public List<string> Neutral { get; set; } = new List<string>();
            public List<string> Dislike { get; set; } = new List<string>();
            public List<string> Hate { get; set; } = new List<string>();
        }
        public GiftTastes_ GiftTastes { get; set; }

        public Dictionary<string, string> NameLocalization { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> DescriptionLocalization { get; set; } = new Dictionary<string, string>();

        public List<string> ContextTags { get; set; }  = new List<string>();
    }
}
