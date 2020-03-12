using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.JsonAssets.Data
{
    public class PantsData : BaseDataWithTexture
    {
        public string Description { get; set; }

        public int Price { get; set; }

        public Color DefaultColor { get; set; } = new Color(255 / 255f, 235 / 255f, 203 / 255f);
        public bool Dyeable { get; set; } = false;

        public string Metadata { get; set; } = "";

        public Dictionary<string, string> NameLocalization = new Dictionary<string, string>();
        public Dictionary<string, string> DescriptionLocalization = new Dictionary<string, string>();
    }
}
