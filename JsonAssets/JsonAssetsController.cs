﻿using Godot;
using Newtonsoft.Json;
using StardewEditor3.JsonAssets.Data;
using StardewEditor3.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StardewEditor3.JsonAssets
{
    public partial class JsonAssetsController : ContentPackController
    {
        public const string MOD_NAME = "Json Assets";
        public const string MOD_UNIQUE_ID = "spacechase0.JsonAssets";
        public const string MOD_ABBREVIATION = "JA";

        private readonly Dictionary<string, TreeItem> roots = new Dictionary<string, TreeItem>();
        private readonly Dictionary<TreeItem, ObjectData> objects = new Dictionary<TreeItem, ObjectData>();
        private readonly Dictionary<TreeItem, BigCraftableData> bigs = new Dictionary<TreeItem, BigCraftableData>();
        private readonly Dictionary<TreeItem, CropData> crops = new Dictionary<TreeItem, CropData>();
        private readonly Dictionary<TreeItem, FruitTreeData> trees = new Dictionary<TreeItem, FruitTreeData>();
        private readonly Dictionary<TreeItem, HatData> hats = new Dictionary<TreeItem, HatData>();
        private readonly Dictionary<TreeItem, WeaponData> weapons = new Dictionary<TreeItem, WeaponData>();
        private readonly Dictionary<TreeItem, ShirtData> shirts = new Dictionary<TreeItem, ShirtData>();
        private readonly Dictionary<TreeItem, PantsData> pantss = new Dictionary<TreeItem, PantsData>();
        private readonly Dictionary<TreeItem, TailoringRecipeData> tailorings = new Dictionary<TreeItem, TailoringRecipeData>();
        private readonly Dictionary<TreeItem, BootsData> bootss = new Dictionary<TreeItem, BootsData>();

        private TreeItem activeEntry;
        private Node activeEditor;

        private readonly PackedScene ObjectEditor = GD.Load<PackedScene>("res://JsonAssets/ObjectEditor.tscn");
        private readonly PackedScene BigCraftableEditor = GD.Load<PackedScene>("res://JsonAssets/BigCraftableEditor.tscn");
        private readonly PackedScene CropEditor = GD.Load<PackedScene>("res://JsonAssets/CropEditor.tscn");
        private readonly PackedScene FruitTreeEditor = GD.Load<PackedScene>("res://JsonAssets/FruitTreeEditor.tscn");
        private readonly PackedScene HatEditor = GD.Load<PackedScene>("res://JsonAssets/HatEditor.tscn");
        private readonly PackedScene WeaponEditor = GD.Load<PackedScene>("res://JsonAssets/WeaponEditor.tscn");
        private readonly PackedScene ShirtEditor = GD.Load<PackedScene>("res://JsonAssets/ShirtEditor.tscn");
        private readonly PackedScene PantsEditor = GD.Load<PackedScene>("res://JsonAssets/PantsEditor.tscn");
        private readonly PackedScene TailoringEditor = GD.Load<PackedScene>("res://JsonAssets/TailoringRecipeEditor.tscn");
        private readonly PackedScene BootsEditor = GD.Load<PackedScene>("res://JsonAssets/BootsEditor.tscn");
        private readonly PackedScene TranslationHelperEditor = GD.Load<PackedScene>("res://JsonAssets/TranslationHelperEditor.tscn");

        public JsonAssetsController()
        :   base(MOD_NAME, MOD_UNIQUE_ID, MOD_ABBREVIATION)
        {
        }

        public override ModData OnModCreated(UI ui, TreeItem entry)
        {
            AddSections(ui, entry);
            return new JsonAssetsModData();
        }

        public override void OnSave(UI ui, ModData data)
        {
            // todo
        }

        public override void OnLoad(UI ui, ModData data_, TreeItem entry)
        {
            var data = data_ as JsonAssetsModData;

            AddSections(ui, entry);

            var objRoot = roots["Objects"];
            foreach ( var obj in data.Objects )
            {
                var item = ui.ProjectTree.CreateItem(objRoot);
                item.SetText(0, obj.Name);
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this object");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                objects.Add(item, obj);
            }
            
            var cropRoot = roots["Crops"];
            foreach (var crop in data.Crops)
            {
                var item = ui.ProjectTree.CreateItem(cropRoot);
                item.SetText(0, crop.Name);
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this crop");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                crops.Add(item, crop);
            }

            var treeRoot = roots["Fruit Trees"];
            foreach (var tree in data.FruitTrees)
            {
                var item = ui.ProjectTree.CreateItem(treeRoot);
                item.SetText(0, tree.Name);
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this fruit tree");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                trees.Add(item, tree);
            }

            var bigRoot = roots["Big Craftables"];
            foreach (var big in data.BigCraftables)
            {
                var item = ui.ProjectTree.CreateItem(bigRoot);
                item.SetText(0, big.Name);
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this big craftable");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                bigs.Add(item, big);
            }

            var hatRoot = roots["Hats"];
            foreach (var hat in data.Hats)
            {
                var item = ui.ProjectTree.CreateItem(hatRoot);
                item.SetText(0, hat.Name);
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this hat");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                hats.Add(item, hat);
            }

            var weaponRoot = roots["Weapons"];
            foreach (var weapon in data.Weapons)
            {
                var item = ui.ProjectTree.CreateItem(weaponRoot);
                item.SetText(0, weapon.Name);
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this weapon");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                weapons.Add(item, weapon);
            }

            var shirtRoot = roots["Shirts"];
            foreach (var shirt in data.Shirts)
            {
                var item = ui.ProjectTree.CreateItem(shirtRoot);
                item.SetText(0, shirt.Name);
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this shirt");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                shirts.Add(item, shirt);
            }

            var pantsRoot = roots["Pants"];
            foreach (var pants in data.Pantss)
            {
                var item = ui.ProjectTree.CreateItem(pantsRoot);
                item.SetText(0, pants.Name);
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove these pants");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                pantss.Add(item, pants);
            }

            var tailoringRoot = roots["Tailoring Recipes"];
            foreach (var trecipe in data.TailoringRecipes)
            {
                var item = ui.ProjectTree.CreateItem(tailoringRoot);
                item.SetText(0, trecipe.Name);
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this tailoring recipe");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                tailorings.Add(item, trecipe);
            }

            var bootsRoot = roots["Boots"];
            foreach (var boots in data.Bootss)
            {
                var item = ui.ProjectTree.CreateItem(bootsRoot);
                item.SetText(0, boots.Name);
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove these boots");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                bootss.Add(item, boots);
            }
        }

        private readonly Regex nameRegex = new Regex(@"^[a-zA-Z0-9_.,\- ]+$", RegexOptions.Compiled);
        private readonly Regex tagRegex = new Regex(@"^[a-zA-Z_]+$", RegexOptions.Compiled);
        public override void OnValidate(UI ui, ModData data_, List<string> errors)
        {
            var data = data_ as JsonAssetsModData;

            foreach (var obj in data.Objects)
            {
                if (!nameRegex.IsMatch(obj.Name))
                    errors.Add($"Object name \"{obj.Name}\" must only contain basic english characters.");
                if (obj.Texture == null || string.IsNullOrEmpty(obj.Texture.Resource))
                    errors.Add($"Object \"{obj.Name}\" must have a texture.");
                if (obj.Description != null && obj.Description.Contains('/'))
                    errors.Add($"Object description for \"{obj.Name}\" must not contain slashes.");
                if (obj.CategoryTextOverride != null && obj.CategoryTextOverride.Contains('/'))
                    errors.Add($"Object category text override for \"{obj.Name}\" must not contain slashes.");

                // todo - validate ingredient names?
                // todo - validate purchase requirements

                if (obj.ContextTags != null)
                {
                    foreach (var tag in obj.ContextTags)
                    {
                        if (!tagRegex.IsMatch(tag))
                            errors.Add($"Object \"{obj.Name}\" has invalid context tag: " + tag);
                    }
                }
            }

            foreach (var big in data.BigCraftables)
            {
                if (!nameRegex.IsMatch(big.Name))
                    errors.Add($"Big craftable name \"{big.Name}\" must only contain basic english characters.");
                if (big.Texture == null || string.IsNullOrEmpty(big.Texture.Resource))
                    errors.Add($"Big craftable \"{big.Name}\" must have a texture.");
                if (big.Description != null && big.Description.Contains('/'))
                    errors.Add($"Big craftable description for \"{big.Name}\" must not contain slashes.");

                // todo - validate ingredient names?
                // todo - validate purchase requirements
            }

            foreach (var crop in data.Crops)
            {
                if (!nameRegex.IsMatch(crop.Name))
                    errors.Add($"Crop name \"{crop.Name}\" must only contain basic english characters.");
                if (crop.Texture == null || string.IsNullOrEmpty(crop.Texture.Resource))
                    errors.Add($"Crop \"{crop.Name}\" must have a texture.");
                if (crop.SeedTexture == null || string.IsNullOrEmpty(crop.SeedTexture.Resource))
                    errors.Add($"Crop \"{crop.Name}\" must have a seed texture.");
                if (!nameRegex.IsMatch(crop.SeedName))
                    errors.Add($"Crop seed name \"{crop.Name}\" must only contain basic english characters.");
                if (crop.SeedDescription != null && crop.SeedDescription.Contains('/'))
                    errors.Add($"Crop seed description for \"{crop.SeedDescription}\" must not contain slashes.");

                // todo - validate seasons
                // todo - validate phases
                // todo - validate seed purchase requirements
            }
            
            foreach (var tree in data.FruitTrees)
            {
                if (!nameRegex.IsMatch(tree.Name))
                    errors.Add($"Fruit tree name \"{tree.Name}\" must only contain basic english characters.");
                if (tree.Texture == null || string.IsNullOrEmpty(tree.Texture.Resource))
                    errors.Add($"Fruit tree \"{tree.Name}\" must have a texture.");
                if (tree.SaplingTexture == null || string.IsNullOrEmpty(tree.SaplingTexture.Resource))
                    errors.Add($"Fruit tree \"{tree.Name}\" must have a seed texture.");
                if (!nameRegex.IsMatch(tree.SaplingName))
                    errors.Add($"Fruit tree sapling name for \"{tree.Name}\" must only contain basic english characters.");
                if (tree.SaplingDescription != null && tree.SaplingDescription.Contains('/'))
                    errors.Add($"Fruit tree sapling description for \"{tree.Name}\" must not contain slashes.");
                
                // todo - validate sapling purchase requirements
            }
            
            foreach (var hat in data.Hats)
            {
                if (!nameRegex.IsMatch(hat.Name))
                    errors.Add($"Hat name \"{hat.Name}\" must only contain basic english characters.");
                if (hat.Texture == null || string.IsNullOrEmpty(hat.Texture.Resource))
                    errors.Add($"Hat \"{hat.Name}\" must have a texture.");
                if (hat.Description != null && hat.Description.Contains('/'))
                    errors.Add($"Hat description for \"{hat.Name}\" must not contain slashes.");
            }

            foreach (var weapon in data.Weapons)
            {
                if (!nameRegex.IsMatch(weapon.Name))
                    errors.Add($"Weapon name \"{weapon.Name}\" must only contain basic english characters.");
                if (weapon.Texture == null || string.IsNullOrEmpty(weapon.Texture.Resource))
                    errors.Add($"Weapon \"{weapon.Name}\" must have a texture.");
                if (weapon.Description != null && weapon.Description.Contains('/'))
                    errors.Add($"Weapon description for \"{weapon.Name}\" must not contain slashes.");

                // todo - validate purchase requirements
            }

            foreach (var shirt in data.Shirts)
            {
                if (!nameRegex.IsMatch(shirt.Name))
                    errors.Add($"Shirt name \"{shirt.Name}\" must only contain basic english characters.");
                if (shirt.MaleTexture == null || string.IsNullOrEmpty(shirt.MaleTexture.Resource))
                    errors.Add($"Shirt \"{shirt.Name}\" must have a primary texture.");
                if (shirt.Description != null && shirt.Description.Contains('/'))
                    errors.Add($"Shirt description for \"{shirt.Name}\" must not contain slashes.");
            }
            
            foreach (var pants in data.Pantss)
            {
                if (!nameRegex.IsMatch(pants.Name))
                    errors.Add($"Pants name \"{pants.Name}\" must only contain basic english characters.");
                if (pants.Texture == null || string.IsNullOrEmpty(pants.Texture.Resource))
                    errors.Add($"Pants \"{pants.Name}\" must have a texture.");
                if (pants.Description != null && pants.Description.Contains('/'))
                    errors.Add($"Pants description for \"{pants.Name}\" must not contain slashes.");
            }

            foreach (var trecipe in data.TailoringRecipes)
            {
                if (!nameRegex.IsMatch(trecipe.Name))
                    errors.Add($"Tailoring recipe name \"{trecipe.Name}\" must only contain basic english characters.");
            }

            foreach (var boots in data.Bootss)
            {
                if (!nameRegex.IsMatch(boots.Name))
                    errors.Add($"Boots name \"{boots.Name}\" must only contain basic english characters.");
                if (boots.Texture == null || string.IsNullOrEmpty(boots.Texture.Resource))
                    errors.Add($"Boots \"{boots.Name}\" must have a texture.");
                if (boots.Description != null && boots.Description.Contains('/'))
                    errors.Add($"Boots description for \"{boots.Name}\" must not contain slashes.");

                // todo - validate purchase requirements
            }
        }

        public override void OnImport(UI ui, ModData data_, string importPath)
        {
            var data = data_ as JsonAssetsModData;

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ContractResolver = new JsonAssetsJsonContractResolver(),
            };

            string path = System.IO.Path.Combine(importPath, "Objects");
            if (System.IO.Directory.Exists(path))
            {
                foreach (var dir in System.IO.Directory.EnumerateDirectories(path))
                {
                    var obj = JsonConvert.DeserializeObject<ObjectData>(System.IO.File.ReadAllText(System.IO.Path.Combine(dir, "object.json")), settings);

                    string newFilename = "Object_" + obj.Name + ".png";
                    System.IO.File.Copy(System.IO.Path.Combine(dir, "object.png"), System.IO.Path.Combine(ui.ModProjectDir, newFilename));
                    obj.Texture = new ImageResourceReference() { Resource = newFilename, SubRect = new Rect2(0, 0, 16, 16) };

                    string colorFilename = System.IO.Path.Combine(dir, "color.png");
                    if (System.IO.File.Exists(colorFilename))
                    {
                        newFilename = "Object_" + obj.Name + "_Color.png";
                        System.IO.File.Copy(colorFilename, System.IO.Path.Combine(ui.ModProjectDir, newFilename));
                        obj.TextureColor = new ImageResourceReference() { Resource = newFilename, SubRect = new Rect2(0, 0, 16, 16) };
                    }

                    data.Objects.Add(obj);

                    var root = roots["Objects"];
                    var item = ui.ProjectTree.CreateItem(root);
                    item.SetText(0, obj.Name);
                    item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this object");
                    item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                    objects.Add(item, obj);
                }
            }

            path = System.IO.Path.Combine(importPath, "Crops");
            if (System.IO.Directory.Exists(path))
            {
                foreach (var dir in System.IO.Directory.EnumerateDirectories(path))
                {
                    var crop = JsonConvert.DeserializeObject<CropData>(System.IO.File.ReadAllText(System.IO.Path.Combine(dir, "crop.json")), settings);

                    string newFilename = "Crop_" + crop.Name + ".png";
                    System.IO.File.Copy(System.IO.Path.Combine(dir, "crop.png"), System.IO.Path.Combine(ui.ModProjectDir, newFilename));
                    crop.Texture = new ImageResourceReference() { Resource = newFilename, SubRect = new Rect2(0, 0, 128, 32) };

                    newFilename = "Crop_" + crop.Name + "_Seeds.png";
                    System.IO.File.Copy(System.IO.Path.Combine(dir, "seeds.png"), System.IO.Path.Combine(ui.ModProjectDir, newFilename));
                    crop.SeedTexture = new ImageResourceReference() { Resource = newFilename, SubRect = new Rect2(0, 0, 16, 16) };

                    string giantFilename = System.IO.Path.Combine(dir, "giant.png");
                    if (System.IO.File.Exists(giantFilename))
                    {
                        newFilename = "Object_" + crop.Name + "_Giant.png";
                        System.IO.File.Copy(giantFilename, System.IO.Path.Combine(ui.ModProjectDir, newFilename));
                        crop.GiantTexture = new ImageResourceReference() { Resource = newFilename, SubRect = new Rect2(0, 0, 48, 63) };
                    }

                    data.Crops.Add(crop);

                    var root = roots["Crops"];
                    var item = ui.ProjectTree.CreateItem(root);
                    item.SetText(0, crop.Name);
                    item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this crop");
                    item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                    crops.Add(item, crop);
                }
            }

            path = System.IO.Path.Combine(importPath, "FruitTrees");
            if (System.IO.Directory.Exists(path))
            {
                foreach (var dir in System.IO.Directory.EnumerateDirectories(path))
                {
                    var ftree = JsonConvert.DeserializeObject<FruitTreeData>(System.IO.File.ReadAllText(System.IO.Path.Combine(dir, "tree.json")), settings);

                    string newFilename = "FruitTree_" + ftree.Name + ".png";
                    System.IO.File.Copy(System.IO.Path.Combine(dir, "tree.png"), System.IO.Path.Combine(ui.ModProjectDir, newFilename));
                    ftree.Texture = new ImageResourceReference() { Resource = newFilename, SubRect = new Rect2(0, 0, 432, 80) };

                    newFilename = "FruitTree_" + ftree.Name + "_Sapling.png";
                    System.IO.File.Copy(System.IO.Path.Combine(dir, "sapling.png"), System.IO.Path.Combine(ui.ModProjectDir, newFilename));
                    ftree.SaplingTexture = new ImageResourceReference() { Resource = newFilename, SubRect = new Rect2(0, 0, 16, 16) };

                    data.FruitTrees.Add(ftree);

                    var root = roots["Fruit Trees"];
                    var item = ui.ProjectTree.CreateItem(root);
                    item.SetText(0, ftree.Name);
                    item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this fruit tree");
                    item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                    trees.Add(item, ftree);
                }
            }

            path = System.IO.Path.Combine(importPath, "BigCraftables");
            if (System.IO.Directory.Exists(path))
            {
                foreach (var dir in System.IO.Directory.EnumerateDirectories(path))
                {
                    var big = JsonConvert.DeserializeObject<BigCraftableData>(System.IO.File.ReadAllText(System.IO.Path.Combine(dir, "big-craftable.json")), settings);

                    string newFilename = "BigCraftable_" + big.Name + ".png";
                    System.IO.File.Copy(System.IO.Path.Combine(dir, "big-craftable.png"), System.IO.Path.Combine(ui.ModProjectDir, newFilename));
                    big.Texture = new ImageResourceReference() { Resource = newFilename, SubRect = new Rect2(0, 0, 16, 32) };

                    for (int i = 0; i < big.ReserveExtraIndices.Count; ++i)
                    {
                        string extraFilename = System.IO.Path.Combine(dir, "big-craftable-" + (i + 2) + ".png");
                        if (System.IO.File.Exists(extraFilename))
                        {
                            newFilename = "BigCraftalbe_" + big.Name + "_" + (i + 2) + ".png";
                            System.IO.File.Copy(extraFilename, System.IO.Path.Combine(ui.ModProjectDir, newFilename));
                            big.ReserveExtraIndices[i] = new ImageResourceReference() { Resource = newFilename, SubRect = new Rect2(0, 0, 16, 32) };
                        }
                    }

                    data.BigCraftables.Add(big);

                    var root = roots["Big Craftables"];
                    var item = ui.ProjectTree.CreateItem(root);
                    item.SetText(0, big.Name);
                    item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this big craftable");
                    item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                    bigs.Add(item, big);
                }
            }

            path = System.IO.Path.Combine(importPath, "Hats");
            if (System.IO.Directory.Exists(path))
            {
                foreach (var dir in System.IO.Directory.EnumerateDirectories(path))
                {
                    var hat = JsonConvert.DeserializeObject<HatData>(System.IO.File.ReadAllText(System.IO.Path.Combine(dir, "hat.json")), settings);

                    string newFilename = "Hat_" + hat.Name + ".png";
                    System.IO.File.Copy(System.IO.Path.Combine(dir, "hat.png"), System.IO.Path.Combine(ui.ModProjectDir, newFilename));
                    hat.Texture = new ImageResourceReference() { Resource = newFilename, SubRect = new Rect2(0, 0, 20, 80) };

                    data.Hats.Add(hat);

                    var root = roots["Hats"];
                    var item = ui.ProjectTree.CreateItem(root);
                    item.SetText(0, hat.Name);
                    item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this hat");
                    item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                    hats.Add(item, hat);
                }
            }

            path = System.IO.Path.Combine(importPath, "Weapons");
            if (System.IO.Directory.Exists(path))
            {
                foreach (var dir in System.IO.Directory.EnumerateDirectories(path))
                {
                    var weapon = JsonConvert.DeserializeObject<WeaponData>(System.IO.File.ReadAllText(System.IO.Path.Combine(dir, "weapon.json")), settings);

                    string newFilename = "Weapon_" + weapon.Name + ".png";
                    System.IO.File.Copy(System.IO.Path.Combine(dir, "weapon.png"), System.IO.Path.Combine(ui.ModProjectDir, newFilename));
                    weapon.Texture = new ImageResourceReference() { Resource = newFilename, SubRect = new Rect2(0, 0, 16, 16) };

                    data.Weapons.Add(weapon);

                    var root = roots["Weapons"];
                    var item = ui.ProjectTree.CreateItem(root);
                    item.SetText(0, weapon.Name);
                    item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this weapon");
                    item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                    weapons.Add(item, weapon);
                }
            }

            path = System.IO.Path.Combine(importPath, "Shirts");
            if (System.IO.Directory.Exists(path))
            {
                foreach (var dir in System.IO.Directory.EnumerateDirectories(path))
                {
                    var shirt = JsonConvert.DeserializeObject<ShirtData>(System.IO.File.ReadAllText(System.IO.Path.Combine(dir, "shirt.json")), settings);

                    string newFilename = "Shirt_" + shirt.Name + "_Male.png";
                    System.IO.File.Copy(System.IO.Path.Combine(dir, "male.png"), System.IO.Path.Combine(ui.ModProjectDir, newFilename));
                    shirt.MaleTexture = new ImageResourceReference() { Resource = newFilename, SubRect = new Rect2(0, 0, 8, 32) };

                    string colorFilename = System.IO.Path.Combine(dir, "male-color.png");
                    if (System.IO.File.Exists(colorFilename))
                    {
                        newFilename = "Shirt_" + shirt.Name + "_Male_Color.png";
                        System.IO.File.Copy(colorFilename, System.IO.Path.Combine(ui.ModProjectDir, newFilename));
                        shirt.MaleColorTexture = new ImageResourceReference() { Resource = newFilename, SubRect = new Rect2(0, 0, 8, 32) };
                    }

                    if (shirt.HasFemaleVariant)
                    {
                        newFilename = "Shirt_" + shirt.Name + "_Female.png";
                        System.IO.File.Copy(System.IO.Path.Combine(dir, "female.png"), System.IO.Path.Combine(ui.ModProjectDir, newFilename));
                        shirt.FemaleTexture = new ImageResourceReference() { Resource = newFilename, SubRect = new Rect2(0, 0, 8, 32) };

                        colorFilename = System.IO.Path.Combine(dir, "female-color.png");
                        if (System.IO.File.Exists(colorFilename))
                        {
                            newFilename = "Shirt_" + shirt.Name + "_Female_Color.png";
                            System.IO.File.Copy(colorFilename, System.IO.Path.Combine(ui.ModProjectDir, newFilename));
                            shirt.FemaleColorTexture = new ImageResourceReference() { Resource = newFilename, SubRect = new Rect2(0, 0, 8, 32) };
                        }
                    }

                    data.Shirts.Add(shirt);

                    var root = roots["Shirts"];
                    var item = ui.ProjectTree.CreateItem(root);
                    item.SetText(0, shirt.Name);
                    item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this shirt");
                    item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                    shirts.Add(item, shirt);
                }
            }

            path = System.IO.Path.Combine(importPath, "Pants");
            if (System.IO.Directory.Exists(path))
            {
                foreach (var dir in System.IO.Directory.EnumerateDirectories(path))
                {
                    var pants = JsonConvert.DeserializeObject<PantsData>(System.IO.File.ReadAllText(System.IO.Path.Combine(dir, "pants.json")), settings);

                    string newFilename = "Pants_" + pants.Name + ".png";
                    System.IO.File.Copy(System.IO.Path.Combine(dir, "pants.png"), System.IO.Path.Combine(ui.ModProjectDir, newFilename));
                    pants.Texture = new ImageResourceReference() { Resource = newFilename, SubRect = new Rect2(0, 0, 192, 688) };

                    data.Pantss.Add(pants);

                    var root = roots["Pants"];
                    var item = ui.ProjectTree.CreateItem(root);
                    item.SetText(0, pants.Name);
                    item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove these pants");
                    item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                    pantss.Add(item, pants);
                }
            }

            path = System.IO.Path.Combine(importPath, "Tailoring");
            if (System.IO.Directory.Exists(path))
            {
                foreach (var dir in System.IO.Directory.EnumerateDirectories(path))
                {
                    var recipe = JsonConvert.DeserializeObject<TailoringRecipeData>(System.IO.File.ReadAllText(System.IO.Path.Combine(dir, "recipe.json")), settings);
                    recipe.Name = System.IO.Path.GetFileName(dir);

                    data.TailoringRecipes.Add(recipe);

                    var root = roots["Tailoring Recipes"];
                    var item = ui.ProjectTree.CreateItem(root);
                    item.SetText(0, recipe.Name);
                    item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this tailoring recipe");
                    item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                    tailorings.Add(item, recipe);
                }
            }

            path = System.IO.Path.Combine(importPath, "Boots");
            if (System.IO.Directory.Exists(path))
            {
                foreach (var dir in System.IO.Directory.EnumerateDirectories(path))
                {
                    var boots = JsonConvert.DeserializeObject<BootsData>(System.IO.File.ReadAllText(System.IO.Path.Combine(dir, "boots.json")), settings);

                    string newFilename = "Boots_" + boots.Name + ".png";
                    System.IO.File.Copy(System.IO.Path.Combine(dir, "boots.png"), System.IO.Path.Combine(ui.ModProjectDir, newFilename));
                    boots.Texture = new ImageResourceReference() { Resource = newFilename, SubRect = new Rect2(0, 0, 16, 16) };

                    var colorImage = new Image();
                    colorImage.Load(System.IO.Path.Combine(dir, "color.png"));
                    colorImage.Lock();
                    for (int i = 0; i < boots.ColorPalette.Length; ++i)
                        boots.ColorPalette[i] = colorImage.GetPixel(i, 0);
                    colorImage.Unlock();
                    colorImage.Dispose();

                    data.Bootss.Add(boots);

                    var root = roots["Boots"];
                    var item = ui.ProjectTree.CreateItem(root);
                    item.SetText(0, boots.Name);
                    item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove these boots");
                    item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                    bootss.Add(item, boots);
                }
            }
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
                ContractResolver = new JsonAssetsJsonContractResolver(),
            };

            string objPath = System.IO.Path.Combine(path, "Objects");
            System.IO.Directory.CreateDirectory(objPath);
            foreach ( var obj in data.Objects )
            {
                if (obj.Recipe != null && obj.Recipe.ResultCount == 0)
                    obj.Recipe = null;
                if (obj.EdibleBuffs != null && obj.EdibleBuffs.Duration == 0)
                    obj.EdibleBuffs = null;
                if (obj.GiftTastes != null && obj.GiftTastes.Love.Count == 0 && obj.GiftTastes.Like.Count == 0 &&
                    obj.GiftTastes.Neutral.Count == 0 && obj.GiftTastes.Dislike.Count == 0 && obj.GiftTastes.Hate.Count == 0)
                    obj.GiftTastes = null;

                if (obj.CategoryTextOverride == "")
                    obj.CategoryTextOverride = null;
                if (obj.CategoryColorOverride?.a == 0)
                    obj.CategoryColorOverride = null;

                string objDir = System.IO.Path.Combine(objPath, obj.Name);
                System.IO.Directory.CreateDirectory(objDir);
                System.IO.File.WriteAllText(System.IO.Path.Combine(objDir, "object.json"), JsonConvert.SerializeObject(obj, settings));
                
                var image = obj.Texture.MakeImage(ui.ModProjectDir);
                image.SavePng(System.IO.Path.Combine(objDir, "object.png"));
                image.Dispose();

                var colImage = obj.TextureColor?.MakeImage(ui.ModProjectDir);
                colImage?.SavePng(System.IO.Path.Combine(objDir, "color.png"));
                colImage?.Dispose();

                if (obj.Recipe == null)
                    obj.Recipe = new RecipeData();
                if (obj.EdibleBuffs == null)
                    obj.EdibleBuffs = new ObjectData.FoodBuffs_();
                if (obj.GiftTastes == null)
                    obj.GiftTastes = new ObjectData.GiftTastes_();
            }
            
            string cropPath = System.IO.Path.Combine(path, "Crops");
            System.IO.Directory.CreateDirectory(cropPath);
            foreach (var crop in data.Crops)
            {
                if (crop.Bonus != null && crop.Bonus.MinimumPerHarvest == 0)
                    crop.Bonus = null;

                string cropDir = System.IO.Path.Combine(cropPath, crop.Name);
                System.IO.Directory.CreateDirectory(cropDir);
                System.IO.File.WriteAllText(System.IO.Path.Combine(cropDir, "crop.json"), JsonConvert.SerializeObject(crop, settings));

                var image = crop.Texture.MakeImage(ui.ModProjectDir);
                image.SavePng(System.IO.Path.Combine(cropDir, "crop.png"));
                image.Dispose();
                
                var seedsImage = crop.SeedTexture.MakeImage(ui.ModProjectDir);
                seedsImage.SavePng(System.IO.Path.Combine(cropDir, "seeds.png"));
                seedsImage.Dispose();

                var giantImage = crop.GiantTexture?.MakeImage(ui.ModProjectDir);
                giantImage?.SavePng(System.IO.Path.Combine(cropDir, "giant.png"));
                giantImage?.Dispose();

                if (crop.Bonus == null)
                    crop.Bonus = new CropData.Bonus_();
            }

            string treePath = System.IO.Path.Combine(path, "FruitTrees");
            System.IO.Directory.CreateDirectory(treePath);
            foreach (var tree in data.FruitTrees)
            {
                string treeDir = System.IO.Path.Combine(treePath, tree.Name);
                System.IO.Directory.CreateDirectory(treeDir);
                System.IO.File.WriteAllText(System.IO.Path.Combine(treeDir, "tree.json"), JsonConvert.SerializeObject(tree, settings));

                var image = tree.Texture.MakeImage(ui.ModProjectDir);
                image.SavePng(System.IO.Path.Combine(treeDir, "tree.png"));
                image.Dispose();

                var seedsImage = tree.SaplingTexture.MakeImage(ui.ModProjectDir);
                seedsImage.SavePng(System.IO.Path.Combine(treeDir, "sapling.png"));
                seedsImage.Dispose();
            }

            string bigPath = System.IO.Path.Combine(path, "BigCraftables");
            System.IO.Directory.CreateDirectory(bigPath);
            foreach (var big in data.BigCraftables)
            {
                if (big.Recipe == null && big.Recipe.ResultCount == 0)
                    big.Recipe = null;

                string bigDir = System.IO.Path.Combine(bigPath, big.Name);
                System.IO.Directory.CreateDirectory(bigDir);
                System.IO.File.WriteAllText(System.IO.Path.Combine(bigDir, "big-craftable.json"), JsonConvert.SerializeObject(big, settings));

                var image = big.Texture.MakeImage(ui.ModProjectDir);
                image.SavePng(System.IO.Path.Combine(bigDir, "big-craftable.png"));
                image.Dispose();

                int e = 2;
                foreach (var imageRef in big.ReserveExtraIndices)
                {
                    var extraImage = imageRef.MakeImage(ui.ModProjectDir);
                    extraImage.SavePng(System.IO.Path.Combine(bigDir, $"big-craftable-{e}.png"));
                    extraImage.Dispose();
                    ++e;
                }

                if (big.Recipe == null)
                    big.Recipe = new RecipeData();
            }

            string hatPath = System.IO.Path.Combine(path, "Hats");
            System.IO.Directory.CreateDirectory(hatPath);
            foreach (var hat in data.Hats)
            {
                string hatDir = System.IO.Path.Combine(hatPath, hat.Name);
                System.IO.Directory.CreateDirectory(hatDir);
                System.IO.File.WriteAllText(System.IO.Path.Combine(hatDir, "hat.json"), JsonConvert.SerializeObject(hat, settings));

                var image = hat.Texture.MakeImage(ui.ModProjectDir);
                image.SavePng(System.IO.Path.Combine(hatDir, "hat.png"));
                image.Dispose();
            }

            string weaponPath = System.IO.Path.Combine(path, "Weapons");
            System.IO.Directory.CreateDirectory(weaponPath);
            foreach (var weapon in data.Weapons)
            {
                string weaponDir = System.IO.Path.Combine(weaponPath, weapon.Name);
                System.IO.Directory.CreateDirectory(weaponDir);
                System.IO.File.WriteAllText(System.IO.Path.Combine(weaponDir, "weapon.json"), JsonConvert.SerializeObject(weapon, settings));

                var image = weapon.Texture.MakeImage(ui.ModProjectDir);
                image.SavePng(System.IO.Path.Combine(weaponDir, "weapon.png"));
                image.Dispose();
            }

            string shirtPath = System.IO.Path.Combine(path, "Shirts");
            System.IO.Directory.CreateDirectory(shirtPath);
            foreach (var shirt in data.Shirts)
            {
                string shirtDir = System.IO.Path.Combine(shirtPath, shirt.Name);
                System.IO.Directory.CreateDirectory(shirtDir);
                System.IO.File.WriteAllText(System.IO.Path.Combine(shirtDir, "shirt.json"), JsonConvert.SerializeObject(shirt, settings));

                var image = shirt.MaleTexture.MakeImage(ui.ModProjectDir);
                image.SavePng(System.IO.Path.Combine(shirtDir, "male.png"));
                image.Dispose();

                image = shirt.MaleColorTexture?.MakeImage(ui.ModProjectDir);
                image?.SavePng(System.IO.Path.Combine(shirtDir, "male-color.png"));
                image?.Dispose();

                image = shirt.FemaleTexture?.MakeImage(ui.ModProjectDir);
                image?.SavePng(System.IO.Path.Combine(shirtDir, "female.png"));
                image?.Dispose();

                image = shirt.FemaleColorTexture?.MakeImage(ui.ModProjectDir);
                image?.SavePng(System.IO.Path.Combine(shirtDir, "female-color.png"));
                image?.Dispose();
            }

            string pantsPath = System.IO.Path.Combine(path, "Pants");
            System.IO.Directory.CreateDirectory(pantsPath);
            foreach (var pants in data.Pantss)
            {
                string pantsDir = System.IO.Path.Combine(pantsPath, pants.Name);
                System.IO.Directory.CreateDirectory(pantsDir);
                System.IO.File.WriteAllText(System.IO.Path.Combine(pantsDir, "pants.json"), JsonConvert.SerializeObject(pants, settings));

                var image = pants.Texture.MakeImage(ui.ModProjectDir);
                image.SavePng(System.IO.Path.Combine(pantsDir, "pants.png"));
                image.Dispose();
            }

            string tailoringPath = System.IO.Path.Combine(path, "Tailoring");
            System.IO.Directory.CreateDirectory(tailoringPath);
            foreach (var trecipe in data.TailoringRecipes)
            {
                string tailoringDir = System.IO.Path.Combine(tailoringPath, trecipe.Name);
                System.IO.Directory.CreateDirectory(tailoringDir);
                System.IO.File.WriteAllText(System.IO.Path.Combine(tailoringDir, "recipe.json"), JsonConvert.SerializeObject(trecipe, settings));
            }

            string bootsPath = System.IO.Path.Combine(path, "Boots");
            System.IO.Directory.CreateDirectory(bootsPath);
            foreach (var boots in data.Bootss)
            {
                string bootsDir = System.IO.Path.Combine(bootsPath, boots.Name);
                System.IO.Directory.CreateDirectory(bootsDir);
                System.IO.File.WriteAllText(System.IO.Path.Combine(bootsDir, "boots.json"), JsonConvert.SerializeObject(boots, settings));

                var image = boots.Texture.MakeImage(ui.ModProjectDir);
                image.SavePng(System.IO.Path.Combine(bootsDir, "boots.png"));
                image.Dispose();

                image = new Image();
                image.Create(4, 1, false, Image.Format.Rgba8);
                image.Lock();
                for ( int i = 0; i < boots.ColorPalette.Length; ++i )
                    image.SetPixel(i, 0, boots.ColorPalette[i]);
                image.Unlock();
                image.SavePng(System.IO.Path.Combine(bootsDir, "color.png"));
                image.Dispose();
            }
        }

        public override void OnAdded(UI ui, ModData data_, TreeItem root)
        {
            var data = data_ as JsonAssetsModData;

            if ( root == roots[ "Objects" ] )
            {
                var objData = new ObjectData()
                {
                    Name = "Object",
                };
                data.Objects.Add(objData);

                var item = ui.ProjectTree.CreateItem(root);
                item.SetText(0, "Object");
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this object");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                objects.Add(item, objData);
            }
            else if (root == roots["Crops"])
            {
                var cropData = new CropData()
                {
                    Name = "Crop",
                };
                data.Crops.Add(cropData);

                var item = ui.ProjectTree.CreateItem(root);
                item.SetText(0, "Crop");
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this crop");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                crops.Add(item, cropData);
            }
            else if (root == roots["Fruit Trees"])
            {
                var treeData = new FruitTreeData()
                {
                    Name = "Fruit Tree",
                };
                data.FruitTrees.Add(treeData);

                var item = ui.ProjectTree.CreateItem(root);
                item.SetText(0, "Fruit Tree");
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this fruit tree");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                trees.Add(item, treeData);
            }
            else if (root == roots["Big Craftables"])
            {
                var bigData = new BigCraftableData()
                {
                    Name = "Big Craftable",
                };
                data.BigCraftables.Add(bigData);

                var item = ui.ProjectTree.CreateItem(root);
                item.SetText(0, "Big Craftable");
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this big craftable");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                bigs.Add(item, bigData);
            }
            else if (root == roots["Hats"])
            {
                var hatData = new HatData()
                {
                    Name = "Hat",
                };
                data.Hats.Add(hatData);

                var item = ui.ProjectTree.CreateItem(root);
                item.SetText(0, "Hat");
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this hat");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                hats.Add(item, hatData);
            }
            else if (root == roots["Weapons"])
            {
                var weaponData = new WeaponData()
                {
                    Name = "Weapon",
                };
                data.Weapons.Add(weaponData);

                var item = ui.ProjectTree.CreateItem(root);
                item.SetText(0, "Weapon");
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this weapon");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                weapons.Add(item, weaponData);
            }
            else if (root == roots["Shirts"])
            {
                var shirtData = new ShirtData()
                {
                    Name = "Shirt",
                };
                data.Shirts.Add(shirtData);

                var item = ui.ProjectTree.CreateItem(root);
                item.SetText(0, "Shirt");
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this shirt");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                shirts.Add(item, shirtData);
            }
            else if (root == roots["Pants"])
            {
                var pantsData = new PantsData()
                {
                    Name = "Pants",
                };
                data.Pantss.Add(pantsData);

                var item = ui.ProjectTree.CreateItem(root);
                item.SetText(0, "Pants");
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove these pants");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                pantss.Add(item, pantsData);
            }
            else if (root == roots["Tailoring Recipes"])
            {
                var trecipeData = new TailoringRecipeData()
                {
                    Name = "Tailoring Recipe",
                };
                data.TailoringRecipes.Add(trecipeData);

                var item = ui.ProjectTree.CreateItem(root);
                item.SetText(0, "Tailoring Recipe");
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this tailoring recipe");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                tailorings.Add(item, trecipeData);
            }
            else if (root == roots["Boots"])
            {
                var bootsData = new BootsData()
                {
                    Name = "Boots",
                };
                data.Bootss.Add(bootsData);

                var item = ui.ProjectTree.CreateItem(root);
                item.SetText(0, "Boots");
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove these boots");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                bootss.Add(item, bootsData);
            }
        }

        public override void OnRemoved(UI ui, ModData data_, TreeItem entry)
        {
            var data = data_ as JsonAssetsModData;
            if ( objects.ContainsKey( entry ) )
            {
                data.Objects.Remove(objects[entry]);
                objects.Remove(entry);
            }
            else if ( crops.ContainsKey(entry) )
            {
                data.Crops.Remove(crops[entry]);
                crops.Remove(entry);
            }
            else if ( trees.ContainsKey(entry) )
            {
                data.FruitTrees.Remove(trees[entry]);
                trees.Remove(entry);
            }
            else if (bigs.ContainsKey(entry))
            {
                data.BigCraftables.Remove(bigs[entry]);
                bigs.Remove(entry);
            }
            else if (hats.ContainsKey(entry))
            {
                data.Hats.Remove(hats[entry]);
                hats.Remove(entry);
            }
            else if (weapons.ContainsKey(entry))
            {
                data.Weapons.Remove(weapons[entry]);
                weapons.Remove(entry);
            }
            else if (shirts.ContainsKey(entry))
            {
                data.Shirts.Remove(shirts[entry]);
                shirts.Remove(entry);
            }
            else if (pantss.ContainsKey(entry))
            {
                data.Pantss.Remove(pantss[entry]);
                pantss.Remove(entry);
            }
            else if (tailorings.ContainsKey(entry))
            {
                data.TailoringRecipes.Remove(tailorings[entry]);
                tailorings.Remove(entry);
            }
            else if (bootss.ContainsKey(entry))
            {
                data.Bootss.Remove(bootss[entry]);
                bootss.Remove(entry);
            }
        }

        public override Node CreateMainEditingArea(UI ui, ModData data, TreeItem entry)
        {
            activeEntry = entry;
            if ( objects.ContainsKey(entry) )
            {
                activeEditor = ObjectEditor.Instance();
                DoObjectEditorConnections(activeEditor, entry);
            }
            else if (crops.ContainsKey(entry))
            {
                activeEditor = CropEditor.Instance();
                DoCropsEditorConnections(activeEditor, entry);
            }
            else if (trees.ContainsKey(entry))
            {
                activeEditor = FruitTreeEditor.Instance();
                DoFruitTreesEditorConnections(activeEditor, entry);
            }
            else if (bigs.ContainsKey(entry))
            {
                activeEditor = BigCraftableEditor.Instance();
                DoBigCraftableEditorConnections(activeEditor, entry);
            }
            else if (hats.ContainsKey(entry))
            {
                activeEditor = HatEditor.Instance();
                DoHatEditorConnections(activeEditor, entry);
            }
            else if (weapons.ContainsKey(entry))
            {
                activeEditor = WeaponEditor.Instance();
                DoWeaponEditorConnections(activeEditor, entry);
            }
            else if (shirts.ContainsKey(entry))
            {
                activeEditor = ShirtEditor.Instance();
                DoShirtEditorConnections(activeEditor, entry);
            }
            else if (pantss.ContainsKey(entry))
            {
                activeEditor = PantsEditor.Instance();
                DoPantsEditorConnections(activeEditor, entry);
            }
            else if (tailorings.ContainsKey(entry))
            {
                activeEditor = TailoringEditor.Instance();
                DoTailoringRecipeEditorConnections(activeEditor, entry);
            }
            else if (bootss.ContainsKey(entry))
            {
                activeEditor = BootsEditor.Instance();
                DoBootsEditorConnections(activeEditor, entry);
            }

            else if ( entry == roots["Translation Helper"] )
            {
                activeEditor = TranslationHelperEditor.Instance();
                DoTranslationHelperEditorConnections(activeEditor, entry, data as JsonAssetsModData);
            }

            activeEditor.SetMeta(Meta.CorrespondingController, ModUniqueId);
            return activeEditor;
        }

        public override void OnEditingAreaChanged(UI ui, ModData data, Node area)
        {
            activeEditor = null;
        }

        public override void OnResourceRenamed(UI ui, ModData data_, string oldFilename, string newFilename)
        {
            var data = data_ as JsonAssetsModData;

            foreach ( var item in data.Objects )
            {
                if (item.Texture.Resource == oldFilename)
                    item.Texture.Resource = newFilename;
                if (item.TextureColor.Resource == oldFilename)
                    item.TextureColor.Resource = newFilename;
            }

            foreach (var item in data.Crops)
            {
                if (item.Texture.Resource == oldFilename)
                    item.Texture.Resource = newFilename;
                if (item.SeedTexture.Resource == oldFilename)
                    item.SeedTexture.Resource = newFilename;
                if (item.GiantTexture.Resource == oldFilename)
                    item.GiantTexture.Resource = newFilename;
            }

            foreach (var item in data.FruitTrees)
            {
                if (item.Texture.Resource == oldFilename)
                    item.Texture.Resource = newFilename;
                if (item.SaplingTexture.Resource == oldFilename)
                    item.SaplingTexture.Resource = newFilename;
            }
            
            foreach (var item in data.BigCraftables)
            {
                if (item.Texture.Resource == oldFilename)
                    item.Texture.Resource = newFilename;
                foreach ( var res in item.ReserveExtraIndices )
                {
                    if (res.Resource == oldFilename)
                        res.Resource = newFilename;
                }
            }

            foreach (var item in data.Hats)
            {
                if (item.Texture.Resource == oldFilename)
                    item.Texture.Resource = newFilename;
            }

            foreach (var item in data.Weapons)
            {
                if (item.Texture.Resource == oldFilename)
                    item.Texture.Resource = newFilename;
            }

            foreach (var item in data.Shirts)
            {
                if (item.MaleTexture.Resource == oldFilename)
                    item.MaleTexture.Resource = newFilename;
                if (item.MaleColorTexture.Resource == oldFilename)
                    item.MaleColorTexture.Resource = newFilename;
                if (item.FemaleTexture.Resource == oldFilename)
                    item.FemaleTexture.Resource = newFilename;
                if (item.FemaleColorTexture.Resource == oldFilename)
                    item.FemaleColorTexture.Resource = newFilename;
            }

            foreach (var item in data.Pantss)
            {
                if (item.Texture.Resource == oldFilename)
                    item.Texture.Resource = newFilename;
            }

            foreach (var item in data.Bootss)
            {
                if (item.Texture.Resource == oldFilename)
                    item.Texture.Resource = newFilename;
            }
        }

        public override void OnResourceDeleted(UI ui, ModData data_, string oldFilename)
        {
            var data = data_ as JsonAssetsModData;

            foreach (var item in data.Objects)
            {
                if (item.Texture.Resource == oldFilename)
                    item.Texture.Resource = null;
                if (item.TextureColor.Resource == oldFilename)
                    item.TextureColor.Resource = null;
            }

            foreach (var item in data.Crops)
            {
                if (item.Texture.Resource == oldFilename)
                    item.Texture.Resource = null;
                if (item.SeedTexture.Resource == oldFilename)
                    item.SeedTexture.Resource = null;
                if (item.GiantTexture.Resource == oldFilename)
                    item.GiantTexture.Resource = null;
            }

            foreach (var item in data.FruitTrees)
            {
                if (item.Texture.Resource == oldFilename)
                    item.Texture.Resource = null;
                if (item.SaplingTexture.Resource == oldFilename)
                    item.SaplingTexture.Resource = null;
            }

            foreach (var item in data.BigCraftables)
            {
                if (item.Texture.Resource == oldFilename)
                    item.Texture.Resource = null;
                foreach (var res in item.ReserveExtraIndices)
                {
                    if (res.Resource == oldFilename)
                        res.Resource = null;
                }
            }

            foreach (var item in data.Hats)
            {
                if (item.Texture.Resource == oldFilename)
                    item.Texture.Resource = null;
            }

            foreach (var item in data.Weapons)
            {
                if (item.Texture.Resource == oldFilename)
                    item.Texture.Resource = null;
            }

            foreach (var item in data.Shirts)
            {
                if (item.MaleTexture.Resource == oldFilename)
                    item.MaleTexture.Resource = null;
                if (item.MaleColorTexture.Resource == oldFilename)
                    item.MaleColorTexture.Resource = null;
                if (item.FemaleTexture.Resource == oldFilename)
                    item.FemaleTexture.Resource = null;
                if (item.FemaleColorTexture.Resource == oldFilename)
                    item.FemaleColorTexture.Resource = null;
            }

            foreach (var item in data.Pantss)
            {
                if (item.Texture.Resource == oldFilename)
                    item.Texture.Resource = null;
            }

            foreach (var item in data.Bootss)
            {
                if (item.Texture.Resource == oldFilename)
                    item.Texture.Resource = null;
            }
        }

        private void AddSections(UI ui, TreeItem entry)
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
                "Translation Helper",
            };

            foreach (var section in sections)
            {
                var item = ui.ProjectTree.CreateItem(entry);
                item.SetText(0, section);
                if ( section != "Translation Helper" )
                    item.AddButton(0, ui.AddIcon, UI.ADD_BUTTON_INDEX, tooltip: "Add new entry");
                item.SetMeta(Meta.CorrespondingController, ModUniqueId);
                roots.Add(section, item);
            }
        }

        private void DoEditorConnections<T>(Node editor, T obj)
        {
            foreach (var prop in obj.GetType().GetProperties())
            {
                if (Attribute.IsDefined(prop, typeof(DoNotAutoConnectAttribute)))
                    continue;

                // Data-specific stuff
                if ( obj is BigCraftableData && prop.Name == "ReserveExtraIndices")
                {
                    string path = prop.Name + "/SubImageEditor";
                    var imageEditor = editor.GetNode<SubImageEditor>(path);

                    var refs = (List<ImageResourceReference>)prop.GetValue(obj);
                    ImageResourceReference baseRef = new ImageResourceReference();
                    if ( refs.Count > 0 )
                    {
                        baseRef.Resource = refs[0].Resource;
                        baseRef.SubRect = new Rect2(refs[0].SubRect.Value.Position, 16 * refs.Count, 32);
                    }

                    imageEditor.SetValues(baseRef);
                    var lambda = new LambdaWrapper<SubImageEditor>((ie) =>
                    {
                        refs.Clear();
                        var value = ie.GetImageRef();
                        if (string.IsNullOrEmpty(value.Resource) || !value.SubRect.HasValue)
                            return;

                        for (int i = 0; i < value.SubRect.Value.Size.x / 16; ++i)
                        {
                            ImageResourceReference iref = new ImageResourceReference()
                            {
                                Resource = value.Resource,
                                SubRect = new Rect2(value.SubRect.Value.Position.x + i * 16, value.SubRect.Value.Position.y, 16, 32),
                            };
                            refs.Add(iref);
                        }
                    });
                    lambda.SelfConnect(imageEditor, nameof(SubImageEditor.image_changed));
                }
                else if ( obj is CropData && prop.Name == "Product" )
                {
                    var cropData = obj as CropData;

                    var lineEdit = editor.GetNode<LineEdit>(prop.Name + "/LineEdit");
                    lineEdit.Text = cropData.Product?.ToString() ?? "";

                    var lambda = new LambdaWrapper<string>((str) =>
                    {
                        if (int.TryParse(str, out int i))
                            cropData.Product = i;
                        else
                            cropData.Product = str;
                    });
                    lambda.SelfConnect(lineEdit, "text_changed");
                }
                else if ( obj is CropData && prop.Name == "Seasons" )
                {
                    var cropData = obj as CropData;

                    string[] seasons = new string[] { "Spring", "Summer", "Fall", "Winter" };
                    foreach ( var season in seasons )
                    {
                        var checkbox = editor.GetNode<CheckBox>(prop.Name + "/" + season);
                        if (cropData.Seasons.Contains(season.ToLower()))
                            checkbox.Pressed = true;

                        var lambda = new LambdaWrapper<bool>((state) =>
                        {
                            if (state)
                                cropData.Seasons.Add(season.ToLower());
                            else
                                cropData.Seasons.Remove(season.ToLower());
                        });
                        lambda.SelfConnect(checkbox, "toggled");
                    }
                }
                else if (obj is FruitTreeData && prop.Name == "Product")
                {
                    var treeData = obj as FruitTreeData;

                    var lineEdit = editor.GetNode<LineEdit>(prop.Name + "/LineEdit");
                    lineEdit.Text = treeData.Product?.ToString() ?? "";

                    var lambda = new LambdaWrapper<string>((str) =>
                    {
                        if (int.TryParse(str, out int i))
                            treeData.Product = i;
                        else
                            treeData.Product = str;
                    });
                    lambda.SelfConnect(lineEdit, "text_changed");
                }
                else if ( obj is FruitTreeData && prop.Name == "Season" )
                {
                    string path = prop.Name + "/OptionButton";
                    var optionButton = editor.GetNode<OptionButton>(path);
                    int selInd = 0;
                    for (int i = 0; i < optionButton.GetItemCount(); ++i)
                    {
                        if (optionButton.GetItemText(i) == (string)prop.GetValue(obj))
                        {
                            selInd = i;
                            break;
                        }
                    }
                    optionButton.Selected = selInd;
                    var lambda = new LambdaWrapper<int>((idx) =>
                    {
                        var str = optionButton.GetItemText(idx);
                        prop.SetValue(obj, str);
                    });
                    lambda.SelfConnect(optionButton, "item_selected");
                }
                else if ( obj is BootsData && prop.Name == "ColorPalette" )
                {
                    var boots = obj as BootsData;
                    for (int i = 0; i < boots.ColorPalette.Length; ++i)
                    {
                        string path = prop.Name + "/Colors/Color" + i;
                        var colorPicker = editor.GetNode<ColorPickerButton>(path);
                        colorPicker.Color = boots.ColorPalette[i];
                        int tmp = i;
                        var lambda = new LambdaWrapper<Color>((color) => boots.ColorPalette[tmp] = color);
                        lambda.SelfConnect(colorPicker, "color_changed");
                    }
                }


                // Everything else
                else if (prop.Name == "SkillUnlockName")
                {
                    string path = prop.Name + "/OptionButton";
                    var optionButton = editor.GetNode<OptionButton>(path);
                    int selInd = 0;
                    for (int i = 0; i < optionButton.GetItemCount(); ++i)
                    {
                        if (optionButton.GetItemText(i) == (string)prop.GetValue(obj))
                        {
                            selInd = i;
                            break;
                        }
                    }
                    optionButton.Selected = selInd;
                    var lambda = new LambdaWrapper<int>((idx) =>
                    {
                        var str = optionButton.GetItemText(idx);
                        prop.SetValue(obj, str);
                    });
                    lambda.SelfConnect(optionButton, "item_selected");
                }

                else if (prop.PropertyType == typeof(string))
                {
                    string path = prop.Name + "/LineEdit";
                    var lineEdit = editor.GetNode<LineEdit>(path);
                    lineEdit.Text = (string)prop.GetValue(obj);
                    var lambda = new LambdaWrapper<string>((str) => prop.SetValue(obj, str));
                    if (prop.Name == "Name")
                    {
                        lambda = new LambdaWrapper<string>((str) =>
                        {
                            activeEntry.SetText(0, str);
                            prop.SetValue(obj, str);
                        });
                    }
                    lambda.SelfConnect(lineEdit, "text_changed");
                }
                else if (prop.PropertyType == typeof(int))
                {
                    string path = prop.Name + "/SpinBox";
                    var intEdit = editor.GetNode<SpinBox>(path);
                    intEdit.Value = (int)prop.GetValue(obj);
                    var lambda = new LambdaWrapper<float>((value) => prop.SetValue(obj, (int)value));
                    if (prop.Name == "Price")
                    {
                        lambda = new LambdaWrapper<float>((value) =>
                        {
                            var sellProp = obj.GetType().GetProperty("CanSell");
                            if (sellProp != null)
                                sellProp.SetValue(obj, value >= 0);
                            prop.SetValue(obj, (int)value);
                        });
                    }
                    else if (prop.Name == "PurchasePrice")
                    {
                        lambda = new LambdaWrapper<float>((value) =>
                        {
                            var canProp = obj.GetType().GetProperty("CanPurchase");
                            if (canProp != null)
                                canProp.SetValue(obj, value >= 0);
                            prop.SetValue(obj, (int)value);
                        });
                    }
                    lambda.SelfConnect(intEdit, "value_changed");
                }
                else if (prop.PropertyType == typeof(double))
                {
                    string path = prop.Name + "/SpinBox";
                    var doubleEdit = editor.GetNode<SpinBox>(path);
                    doubleEdit.Value = (double)prop.GetValue(obj) * (editor.HasNode(prop.Name + "/PercentLabel") ? 100 : 1);
                    var lambda = new LambdaWrapper<float>((value) => prop.SetValue(obj, ((double)value) / (editor.HasNode(prop.Name + "/PercentLabel") ? 100.0 : 1.0)));
                    lambda.SelfConnect(doubleEdit, "value_changed");
                }
                else if (prop.PropertyType == typeof(bool))
                {
                    string path = prop.Name + "/CheckBox";
                    if (prop.Name.StartsWith("Can"))
                        path = "CanFlags/" + prop.Name.Substring(3);
                    var checkBox = editor.GetNode<CheckBox>(path);
                    checkBox.Pressed = (bool)prop.GetValue(obj);
                    var lambda = new LambdaWrapper<bool>((value) => prop.SetValue(obj, value));
                    lambda.SelfConnect(checkBox, "toggled");
                }
                else if (prop.PropertyType.IsEnum)
                {
                    string path = prop.Name + "/OptionButton";
                    var optionButton = editor.GetNode<OptionButton>(path);
                    int selInd = 0;
                    for (int i = 0; i < optionButton.GetItemCount(); ++i)
                    {
                        if (optionButton.GetItemText(i) == prop.GetValue(obj).ToString())
                        {
                            selInd = i;
                            break;
                        }
                    }
                    optionButton.Selected = selInd;
                    var lambda = new LambdaWrapper<int>((idx) =>
                    {
                        var str = optionButton.GetItemText(idx);
                        prop.SetValue(obj, Enum.Parse(prop.PropertyType, str));
                    });
                    lambda.SelfConnect(optionButton, "item_selected");
                }
                else if (prop.PropertyType == typeof(Color))
                {
                    string path = prop.Name + "/ColorPickerButton";
                    var colorPicker = editor.GetNode<ColorPickerButton>(path);
                    colorPicker.Color = (Color)prop.GetValue(obj);
                    var lambda = new LambdaWrapper<Color>((color) => prop.SetValue(obj, color));
                    lambda.SelfConnect(colorPicker, "color_changed");
                }
                else if (prop.PropertyType == typeof(List<object>))
                {
                    string path = prop.Name + "/StringListEditor";
                    var stringsEditor = editor.GetNode<StringListEditor>(path);
                    var strings = (List<object>)prop.GetValue(obj);
                    foreach (var entry in strings)
                        stringsEditor.AddString((string)entry);
                    var lambdaAdd = new LambdaWrapper(() => strings.Add(""));
                    var lambdaDelete = new LambdaWrapper<int>((ind) => strings.RemoveAt(ind));
                    var lambdaEdit = new LambdaWrapper<int, string>((ind, str) => strings[ind] = str);
                    lambdaAdd.SelfConnect(stringsEditor, nameof(StringListEditor.entry_added));
                    lambdaDelete.SelfConnect(stringsEditor, nameof(StringListEditor.entry_deleted));
                    lambdaEdit.SelfConnect(stringsEditor, nameof(StringListEditor.entry_changed));
                }
                else if (prop.PropertyType == typeof(List<string>))
                {
                    string path = prop.Name + "/StringListEditor";
                    var stringsEditor = editor.GetNode<StringListEditor>(path);
                    var strings = (List<string>)prop.GetValue(obj);
                    foreach (var entry in strings)
                        stringsEditor.AddString(entry);
                    var lambdaAdd = new LambdaWrapper(() => strings.Add(""));
                    var lambdaDelete = new LambdaWrapper<int>((ind) => strings.RemoveAt(ind));
                    var lambdaEdit = new LambdaWrapper<int, string>((ind, str) => strings[ind] = str);
                    lambdaAdd.SelfConnect(stringsEditor, nameof(StringListEditor.entry_added));
                    lambdaDelete.SelfConnect(stringsEditor, nameof(StringListEditor.entry_deleted));
                    lambdaEdit.SelfConnect(stringsEditor, nameof(StringListEditor.entry_changed));
                }
                else if (prop.PropertyType == typeof(List<Color>))
                {
                    string path = prop.Name + "/ColorListEditor";
                    var colorsEditor = editor.GetNode<ColorListEditor>(path);
                    var colors = (List<Color>)prop.GetValue(obj);
                    foreach (var entry in colors)
                        colorsEditor.AddColor(entry);
                    var lambdaAdd = new LambdaWrapper(() => colors.Add(Colors.Black));
                    var lambdaDelete = new LambdaWrapper<int>((ind) => colors.RemoveAt(ind));
                    var lambdaEdit = new LambdaWrapper<int, Color>((ind, col) => colors[ind] = col);
                    lambdaAdd.SelfConnect(colorsEditor, nameof(StringListEditor.entry_added));
                    lambdaDelete.SelfConnect(colorsEditor, nameof(StringListEditor.entry_deleted));
                    lambdaEdit.SelfConnect(colorsEditor, nameof(StringListEditor.entry_changed));
                }
                else if (prop.PropertyType == typeof(ImageResourceReference))
                {
                    string path = prop.Name + "/SubImageEditor";
                    var imageEditor = editor.GetNode<SubImageEditor>(path);
                    imageEditor.SetValues((ImageResourceReference)prop.GetValue(obj));
                    var lambda = new LambdaWrapper<SubImageEditor>((ie) => prop.SetValue(obj, ie.GetImageRef()));
                    if (prop.Name == "TextureColor")
                    {
                        lambda = new LambdaWrapper<SubImageEditor>((ie) =>
                        {
                            var ir = ie.GetImageRef();
                            obj.GetType().GetProperty("IsColored").SetValue(obj, !string.IsNullOrEmpty(ir.Resource));
                            prop.SetValue(obj, ir);
                        });
                    }
                    else if (prop.Name == "FemaleTexture")
                    {
                        lambda = new LambdaWrapper<SubImageEditor>((ie) =>
                        {
                            var ir = ie.GetImageRef();
                            obj.GetType().GetProperty("HasFemaleVariant").SetValue(obj, !string.IsNullOrEmpty(ir.Resource));
                            prop.SetValue(obj, ir);
                        });
                    }
                    lambda.SelfConnect(imageEditor, nameof(SubImageEditor.image_changed));
                }
                else if (prop.PropertyType == typeof(RecipeData))
                {
                    string path = prop.Name + "/RecipeEditor";
                    var recipeEditor = editor.GetNode(path);
                    DoEditorConnections(recipeEditor, (RecipeData)prop.GetValue(obj));
                }
                else if (prop.PropertyType == typeof(List<RecipeData.Ingredient>))
                {
                    string path = prop.Name + "/IngredientListEditor";
                    var ingredEditor = editor.GetNode<IngredientListEditor>(path);
                    var ingreds = (List<RecipeData.Ingredient>)prop.GetValue(obj);
                    foreach (var ingred in ingreds)
                        ingredEditor.AddEntry(ingred.Object?.ToString(), ingred.Count);
                    var lambdaAdd = new LambdaWrapper(() => ingreds.Add(new RecipeData.Ingredient()));
                    var lambdaDelete = new LambdaWrapper<int>((ind) => ingreds.RemoveAt(ind));
                    var lambdaEdit = new LambdaWrapper<int, string, int>((idx, ingred, count) =>
                    {
                        if (int.TryParse(ingred, out int ingredId))
                            ingreds[idx].Object = ingredId;
                        else
                            ingreds[idx].Object = ingred;
                        ingreds[idx].Count = count;
                    });
                    lambdaAdd.SelfConnect(ingredEditor, nameof(IngredientListEditor.entry_added));
                    lambdaDelete.SelfConnect(ingredEditor, nameof(IngredientListEditor.entry_deleted));
                    lambdaEdit.SelfConnect(ingredEditor, nameof(IngredientListEditor.entry_changed));
                }
                else if (prop.PropertyType == typeof(ObjectData.FoodBuffs_))
                {
                    string path = prop.Name + "/Buffs";
                    var buffsContainer = editor.GetNode(path);
                    DoEditorConnections(buffsContainer, prop.GetValue(obj));
                }
                else if (prop.PropertyType == typeof(ObjectData.GiftTastes_))
                {
                    string path = prop.Name + "/GiftTasteEditor";
                    var giftsEditor = editor.GetNode<GiftTasteEditor>(path);
                    var gifts = (ObjectData.GiftTastes_)prop.GetValue(obj);
                    var entries = new List<Pair<string, string>>();
                    foreach (var giftProp in gifts.GetType().GetProperties())
                    {
                        var tasteEntries = (List<string>)giftProp.GetValue(gifts);
                        foreach (var taste in tasteEntries)
                        {
                            giftsEditor.AddEntry(taste, giftProp.Name);
                            entries.Add(new Pair<string, string>(taste, giftProp.Name));
                        }
                    }
                    var lambdaAdd = new LambdaWrapper(() => entries.Add(new Pair<string, string>("", "Neutral")));
                    var lambdaDelete = new LambdaWrapper<int>((idx) =>
                    {
                        var entry = entries[idx];
                        entries.RemoveAt(idx);
                        if (entry.First != "")
                            ((List<string>)gifts.GetType().GetProperty(entry.First).GetValue(gifts)).Remove(entry.Second);
                    });
                    var lambdaEdit = new LambdaWrapper<int, string, string>((idx, npc, likedness) =>
                    {
                        var entry = entries[idx];
                        if (entry.First != "")
                            ((List<string>)gifts.GetType().GetProperty(entry.First).GetValue(gifts)).Remove(entry.Second);
                        ((List<string>)gifts.GetType().GetProperty(likedness).GetValue(gifts)).Add(npc);
                        entries[idx].First = likedness;
                        entries[idx].Second = npc;

                    });
                    lambdaAdd.SelfConnect(giftsEditor, nameof(GiftTasteEditor.entry_added));
                    lambdaDelete.SelfConnect(giftsEditor, nameof(GiftTasteEditor.entry_deleted));
                    lambdaEdit.SelfConnect(giftsEditor, nameof(GiftTasteEditor.entry_changed));
                }
                else if (prop.PropertyType == typeof(CropData.Bonus_))
                {
                    string path = prop.Name + "/BonusEditor";
                    var bonusContainer = editor.GetNode(path);
                    DoEditorConnections(bonusContainer, prop.GetValue(obj));
                }
                else if (prop.PropertyType == typeof(Dictionary<string, string>))
                {
                    string path = prop.Name + "/LocalizationEditor";
                    var locEditor = editor.GetNode<LocalizationEditor>(path);
                    var locs = (Dictionary<string, string>)prop.GetValue(obj);
                    var entries = new List<Pair<string, string>>();
                    foreach (var loc in locs)
                    {
                        locEditor.AddEntry(Languages.CodeToLanguage(loc.Key), loc.Value);
                        entries.Add(new Pair<string, string>(loc.Key, loc.Value));
                    }
                    var lambdaAdd = new LambdaWrapper(() => entries.Add(new Pair<string, string>("", "")));
                    var lambdaDelete = new LambdaWrapper<int>((idx) =>
                    {
                        entries.RemoveAt(idx);
                        locs.Clear();
                        foreach (var entry in entries)
                        {
                            if (!locs.ContainsKey(entry.First))
                                locs.Add(entry.First, entry.Second);
                        }
                    });
                    var lambdaEdit = new LambdaWrapper<int, string, string>((idx, lang, str) =>
                    {
                        lang = Languages.LanguageToCode(lang);
                        entries[idx].First = lang;
                        entries[idx].Second = str;
                        locs.Clear();
                        foreach (var entry in entries)
                        {
                            if (!locs.ContainsKey(entry.First))
                                locs.Add(entry.First, entry.Second);
                        }
                    });
                    lambdaAdd.SelfConnect(locEditor, nameof(LocalizationEditor.entry_added));
                    lambdaDelete.SelfConnect(locEditor, nameof(LocalizationEditor.entry_deleted));
                    lambdaEdit.SelfConnect(locEditor, nameof(LocalizationEditor.entry_changed));
                }
            }
        }
    }
}
