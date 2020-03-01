using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.ContentPackControllers
{
    public abstract class ContentPackController
    {
        public string ModName { get; }
        public string ModUniqueId { get; }

        protected ContentPackController(string modName, string modUniqueId)
        {
            ModName = modName;
            ModUniqueId = modUniqueId;
        }

        private static Dictionary<string, ContentPackController> controllers = GetInitialControllers();

        public static string[] GetRegisteredControllerTypes()
        {
            return controllers.Keys.ToArray();
        }

        public static ContentPackController GetControllerForMod(string modUniqueId)
        {
            return controllers[modUniqueId];
        }

        public static void RegsiterController(ContentPackController controller)
        {
            controllers.Add(controller.ModUniqueId, controller);
        }

        private static Dictionary<string, ContentPackController> GetInitialControllers()
        {
            var ret = new Dictionary<string, ContentPackController>(); ;
            ret.Add(JsonAssetsController.MOD_UNIQUE_ID, new JsonAssetsController());
            return ret;
        }
    }
}
