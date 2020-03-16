using Newtonsoft.Json;
using StardewEditor3;
using StardewEditor3.JsonAssets.Data;
using StardewEditor3.Util;
using System.Collections.Generic;

namespace StardewEditor3.JsonAssets.Data
{
	public class BigCraftableData : BaseDataWithTexture
	{
		public List<ImageResourceReference> ReserveExtraIndices { get; set; } = new List<ImageResourceReference>();

		public string Description { get; set; }

        [DoNotAutoConnect]
		public int Price { get; set; }

		public bool ProvidesLight { get; set; } = false;

		public RecipeData Recipe { get; set; }

        [DoNotAutoConnect]
		public bool CanPurchase { get; set; } = false;
		public int PurchasePrice { get; set; }
		public string PurchaseFrom { get; set; } = "Pierre";
		public List<string> PurchaseRequirements { get; set; } = new List<string>();

		public Dictionary<string, string> NameLocalization = new Dictionary<string, string>();
		public Dictionary<string, string> DescriptionLocalization = new Dictionary<string, string>();
	}
}
