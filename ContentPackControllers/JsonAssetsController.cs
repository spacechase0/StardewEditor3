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

        public JsonAssetsController()
        :   base(MOD_NAME, MOD_UNIQUE_ID)
        {
        }
    }
}
