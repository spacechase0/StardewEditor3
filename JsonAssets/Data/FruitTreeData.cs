﻿using Newtonsoft.Json;
using StardewEditor3.Util;
using System.Collections.Generic;

namespace StardewEditor3.JsonAssets.Data
{
    public class FruitTreeData : BaseDataWithTexture
    {
        public ImageResourceReference SaplingTexture { get; set; }

        public object Product { get; set; }
        public string SaplingName { get; set; }
        public string SaplingDescription { get; set; }

        public string Season { get; set; }

        public List<string> SaplingPurchaseRequirements { get; set; } = new List<string>();
        public int SaplingPurchasePrice { get; set; }
        public string SaplingPurchaseFrom { get; set; } = "Pierre";

        public Dictionary<string, string> SaplingNameLocalization { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SaplingDescriptionLocalization { get; set; } = new Dictionary<string, string>();
    }
}
