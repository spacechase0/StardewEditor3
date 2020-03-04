using Newtonsoft.Json;
using StardewEditor3;

namespace JsonAssets.Data
{
    public abstract class BaseDataWithTexture : BaseData
    {
        public ImageResourceReference Texture { get; set; }
    }
}
