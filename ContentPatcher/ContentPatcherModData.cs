using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.ContentPatcher
{
    public class ContentPatcherModData : ModData
    {
        public ContentPatcherModData()
        :   base(ContentPatcherController.MOD_UNIQUE_ID)
        {
        }

        public List<ConfigToken> ConfigTokens { get; set; } = new List<ConfigToken>();
    }
}
