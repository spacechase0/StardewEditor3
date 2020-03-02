using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.ContentPackControllers
{
    public class JsonAssetsController : ContentPackController
    {
        public const string MOD_NAME = "Json Assets";
        public const string MOD_UNIQUE_ID = "spacechase0.JsonAssets";
        public const string MOD_ABBREVIATION = "JA";

        public JsonAssetsController()
        :   base(MOD_NAME, MOD_UNIQUE_ID, MOD_ABBREVIATION)
        {
        }

        public override void OnModCreated(UI ui, TreeItem mod)
        {
            string[] sections = new string[]
            {
                "Objects",
                "Crops",
                "Fruit Trees",
                "Big Craftables",
                "Hats",
                "Weapons",
                "Shirts",
                "Pants",
                "Tailoring Recipes",
                "Boots",
            };

            foreach ( var section in sections )
            {
                var item = ui.ProjectTree.CreateItem(mod);
                item.SetText(0, section);
                item.AddButton(0, ui.AddIcon, UI.ADD_BUTTON_INDEX, tooltip: "Add new entry");
            }
        }
    }
}
