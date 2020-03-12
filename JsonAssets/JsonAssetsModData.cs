using StardewEditor3.JsonAssets.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.JsonAssets
{
    public class JsonAssetsModData : ModData
    {
        public JsonAssetsModData()
        :   base(JsonAssetsController.MOD_UNIQUE_ID)
        {
        }

        public List<ObjectData> Objects { get; set; } = new List<ObjectData>();
        public List<CropData> Crops { get; set; } = new List<CropData>();
        public List<FruitTreeData> FruitTrees { get; set; } = new List<FruitTreeData>();
        public List<BigCraftableData> BigCraftables { get; set; } = new List<BigCraftableData>();
        public List<HatData> Hats { get; set; } = new List<HatData>();
        public List<WeaponData> Weapons { get; set; } = new List<WeaponData>();
        public List<ShirtData> Shirts { get; set; } = new List<ShirtData>();
        public List<PantsData> Pantss { get; set; } = new List<PantsData>();
        public List<TailoringRecipeData> TailoringRecipes { get; set; } = new List<TailoringRecipeData>();
        public List<BootsData> Bootss { get; set; } = new List<BootsData>();
    }
}
