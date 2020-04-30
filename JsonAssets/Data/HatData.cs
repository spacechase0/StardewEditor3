using Newtonsoft.Json;
using System.Collections.Generic;

namespace StardewEditor3.JsonAssets.Data
{
    public class HatData : BaseDataWithTexture
    {
        public string Description { get; set; }
        public int PurchasePrice { get; set; }
        public bool ShowHair { get; set; }
        public bool IgnoreHairstyleOffset { get; set; }

        public string Metadata { get; set; } = "";

        public Dictionary<string, string> NameLocalization { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> DescriptionLocalization { get; set; } = new Dictionary<string, string>();
    }
}
