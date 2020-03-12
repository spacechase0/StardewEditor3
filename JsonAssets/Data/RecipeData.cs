using StardewEditor3.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.JsonAssets.Data
{
    public class RecipeData
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
        [DoNotAutoConnect]
        public bool CanPurchase { get; set; } = false;
        public int PurchasePrice { get; set; }
        public string PurchaseFrom { get; set; } = "Gus";
        public List<string> PurchaseRequirements { get; set; } = new List<string>();
    }
}
