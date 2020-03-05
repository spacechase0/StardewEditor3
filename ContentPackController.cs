using Godot;
using StardewEditor3.JsonAssets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3
{
    public abstract class ContentPackController
    {
        public string ModName { get; }
        public string ModUniqueId { get; }
        public string ModAbbreviation { get; }

        protected ContentPackController(string modName, string modUniqueId, string modAbbrev)
        {
            ModName = modName;
            ModUniqueId = modUniqueId;
            ModAbbreviation = modAbbrev;
        }

        public abstract ModData OnModCreated(UI ui, TreeItem mod);

        public abstract void OnSave(UI ui, ModData data);

        public abstract void OnLoad(UI ui, ModData data, TreeItem entry);

        public abstract void OnExport(UI ui, ModData data, string exportPath);

        public abstract void OnAdded(UI ui, ModData data, TreeItem root);

        public abstract void OnRemoved(UI ui, ModData data, TreeItem entry);

        public abstract void OnEditingAreaChanged(UI ui, ModData data, Node area);

        public abstract void OnResourceRenamed(UI ui, ModData data, string oldFilename, string newFilename);

        public abstract void OnResourceDeleted(UI ui, ModData data, string filename);

        private static readonly Dictionary<string, ContentPackController> controllers = GetInitialControllers();

        public static string[] GetRegisteredControllerTypes()
        {
            return controllers.Keys.ToArray();
        }

        public static ContentPackController GetControllerForMod(string modUniqueId)
        {
            return controllers[modUniqueId];
        }

        public static void RegisterController(ContentPackController controller)
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
