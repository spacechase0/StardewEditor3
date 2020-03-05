using Newtonsoft.Json;
using StardewEditor3;
using StardewEditor3.Util;
using System.Collections.Generic;

namespace JsonAssets.Data
{
    public class BigCraftableData : BaseDataWithTexture
    {
        public List<ImageResourceReference> ReserveExtraIndices { get; set; } = new List<ImageResourceReference>();

        public class Recipe_
        {
            public class Ingredient
            {
                public object Object { get; set; }
                public int Count { get; set; }
            }

            public string SkillUnlockName { get; set; } = null;
            public int SkillUnlockLevel { get; set; } = -1;

            public int ResultCount { get; set; } = 1;
            public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();

            public bool IsDefault { get; set; } = false;
            public bool CanPurchase { get; set; } = false;
            public int PurchasePrice { get; set; }
            public string PurchaseFrom { get; set; } = "Gus";
            public List<string> PurchaseRequirements { get; set; } = new List<string>();
        }

        public string Description { get; set; }

        public int Price { get; set; }

        public bool ProvidesLight { get; set; } = false;

        public Recipe_ Recipe { get; set; }

        public bool CanPurchase { get; set; } = false;
        public int PurchasePrice { get; set; }
        public string PurchaseFrom { get; set; } = "Pierre";
        public List<string> PurchaseRequirements { get; set; } = new List<string>();

        public Dictionary<string, string> NameLocalization = new Dictionary<string, string>();
        public Dictionary<string, string> DescriptionLocalization = new Dictionary<string, string>();
    }
}
