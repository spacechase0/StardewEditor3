using Newtonsoft.Json;
using StardewEditor3;
using StardewEditor3.Util;

namespace StardewEditor3.JsonAssets.Data
{
    public abstract class BaseDataWithTexture : BaseData
    {
        public ImageResourceReference Texture { get; set; }
    }
}
