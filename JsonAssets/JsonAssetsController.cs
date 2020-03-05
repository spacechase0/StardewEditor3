using Godot;
using JsonAssets.Data;
using Newtonsoft.Json;
using StardewEditor3.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.JsonAssets
{
    public class JsonAssetsController : ContentPackController
    {
        public const string MOD_NAME = "Json Assets";
        public const string MOD_UNIQUE_ID = "spacechase0.JsonAssets";
        public const string MOD_ABBREVIATION = "JA";

        private readonly Dictionary<string, TreeItem> roots = new Dictionary<string, TreeItem>();
        private readonly Dictionary<TreeItem, ObjectData> objects = new Dictionary<TreeItem, ObjectData>();

        public JsonAssetsController()
        :   base(MOD_NAME, MOD_UNIQUE_ID, MOD_ABBREVIATION)
        {
        }

        public override ModData OnModCreated(UI ui, TreeItem mod)
        {
            string[] sections = new string[]
            {
                "Objects",
                "Crops",
                "Fruit Trees",
                "Big Craftables",
                "Hats",
                "Weapons",
                "Shirts",
                "Pants",
                "Tailoring Recipes",
                "Boots",
            };

            foreach ( var section in sections )
            {
                var item = ui.ProjectTree.CreateItem(mod);
                item.SetText(0, section);
                item.AddButton(0, ui.AddIcon, UI.ADD_BUTTON_INDEX, tooltip: "Add new entry");
                item.SetMeta(Meta.CorrespondingController, ModUniqueId);
                roots.Add(section, item);
            }

            return new JsonAssetsModData();
        }

        public override void OnSave(UI ui, ModData data)
        {
            // todo
        }

        public override void OnLoad(UI ui, ModData data)
        {
            // todo
        }

        public override void OnExport(UI ui, ModData data_, string exportPath)
        {
            var data = data_ as JsonAssetsModData;

            var path = System.IO.Path.Combine(exportPath, $"[{MOD_ABBREVIATION}] {ui.ModProject.Name}");
            System.IO.Directory.CreateDirectory(path);

            GD.Print("Exporting manifest");
            ExportManifest manifest = new ExportManifest()
            {
                Name = $"[{MOD_ABBREVIATION}] {ui.ModProject.Name}",
                Description = ui.ModProject.Description,
                Author = ui.ModProject.UniqueId,
                Version = ui.ModProject.Version,
                UniqueID = ui.ModProject.UniqueId + "." + MOD_ABBREVIATION,
                ContentPackFor = new ExportContentPackFor()
                {
                    UniqueID = MOD_UNIQUE_ID,
                },
                Dependencies = ui.ModProject.Dependencies, // Using the same list/object is fine since we're just immediately serializing it
            };
            foreach ( var key in ui.ModProject.UpdateKeys )
            {
                manifest.UpdateKeys.Add($"{key.Platform}:{key.Id}");
            }
            System.IO.File.WriteAllText(System.IO.Path.Combine(path, "manifest.json"), JsonConvert.SerializeObject(manifest, Formatting.Indented));

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                ContractResolver = new IgnorePropertiesOfTypeJsonContractResolver(new Type[] { typeof(ImageResourceReference) }),
            };

            string objPath = System.IO.Path.Combine(path, "Objects");
            System.IO.Directory.CreateDirectory(objPath);
            foreach ( var obj in data.Objects )
            {
                string objDir = System.IO.Path.Combine(objPath, obj.Name);
                System.IO.Directory.CreateDirectory(objDir);
                System.IO.File.WriteAllText(System.IO.Path.Combine(objDir, "object.json"), JsonConvert.SerializeObject(obj, settings));
                
                var image = obj.Texture.MakeImage(ui.ModProjectDir);
                image.SavePng(System.IO.Path.Combine(objDir, "object.png"));
                image.Dispose();

                var colImage = obj.TextureColor?.MakeImage(ui.ModProjectDir);
                colImage?.SavePng(System.IO.Path.Combine(objDir, "color.png"));
                colImage?.Dispose();
            }
        }

        public override void OnAdded(UI ui, ModData data, TreeItem root)
        {

        }

        public override void OnRemoved(UI ui, ModData data, TreeItem entry)
        {
            // todo
        }

        public override void OnEditingAreaChanged(UI ui, ModData data, Node area)
        {
            // todo
        }

        public override void OnResourceRenamed(UI ui, ModData data, string oldFilename, string newFilename)
        {
            // todo
        }

        public override void OnResourceDeleted(UI ui, ModData data, string filename)
        {
            // todo
        }
    }
}
