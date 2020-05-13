using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.JsonAssets.Data
{
    public class TailoringRecipeData : BaseData
    {
        public List<string> FirstItemTags { get; set; } = new List<string>(new string[] { "item_cloth" });
        public List<string> SecondItemTags { get; set; } = new List<string>();

        public bool ConsumeSecondItem { get; set; } = true;

        public List<object> CraftedItems { get; set; } = new List<object>();
        public Color CraftedItemColor { get; set; } = new Color(1, 1, 1);
    }
}
