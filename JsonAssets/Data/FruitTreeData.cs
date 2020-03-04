using Newtonsoft.Json;
using System.Collections.Generic;

namespace JsonAssets.Data
{
    public class FruitTreeData : BaseDataWithTexture
    {
        public object Product { get; set; }
        public string SaplingName { get; set; }
        public string SaplingDescription { get; set; }

        public string Season { get; set; }

        public List<string> SaplingPurchaseRequirements { get; set; } = new List<string>();
        public int SaplingPurchasePrice { get; set; }
        public string SaplingPurchaseFrom { get; set; } = "Pierre";

        public Dictionary<string, string> SaplingNameLocalization = new Dictionary<string, string>();
        public Dictionary<string, string> SaplingDescriptionLocalization = new Dictionary<string, string>();
    }
}
