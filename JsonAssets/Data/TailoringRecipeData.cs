using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonAssets.Data
{
    public class TailoringRecipeData
    {
        public string EnableWithMod { get; set; }
        public string DisableWithMod { get; set; }

        public List<string> FirstItemTags { get; set; } = new List<string>(new string[] { "item_cloth" });
        public List<string> SecondItemTags { get; set; }

        public bool ConsumeSecondItem { get; set; } = true;

        public List<object> CraftedItems { get; set; }
        public Color CraftedItemColor { get; set; } = new Color(1, 1, 1);
    }
}
