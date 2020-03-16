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

        private TreeItem activeEntry;
        private Node activeEditor;

        private readonly PackedScene ObjectEditor = GD.Load<PackedScene>("res://JsonAssets/ObjectEditor.tscn");
        private readonly PackedScene BigCraftableEditor = GD.Load<PackedScene>("res://JsonAssets/BigCraftableEditor.tscn");

        public JsonAssetsController()
        :   base(MOD_NAME, MOD_UNIQUE_ID, MOD_ABBREVIATION)
        {
        }

        public override ModData OnModCreated(UI ui, TreeItem mod)
        {
            AddSections(ui, mod);
            return new JsonAssetsModData();
        }

        public override void OnSave(UI ui, ModData data)
        {
            // todo
        }

        public override void OnLoad(UI ui, ModData data, TreeItem entry)
        {
            AddSections(ui, entry);
        }

        private Regex nameRegex = new Regex(@"^[a-zA-Z0-9_.,\- ]+$", RegexOptions.Compiled);
        private Regex tagRegex = new Regex(@"^[a-zA-Z_]+$", RegexOptions.Compiled);
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
                if (obj.Recipe.ResultCount == 0)
                    obj.Recipe = null;
                if (obj.EdibleBuffs.Duration == 0)
                    obj.EdibleBuffs = null;
                if (obj.GiftTastes.Love.Count == 0 && obj.GiftTastes.Like.Count == 0 && obj.GiftTastes.Neutral.Count == 0 &&
                    obj.GiftTastes.Dislike.Count == 0 && obj.GiftTastes.Hate.Count == 0)
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

            string bigPath = System.IO.Path.Combine(path, "BigCraftables");
            System.IO.Directory.CreateDirectory(bigPath);
            foreach (var big in data.BigCraftables)
            {
                if (big.Recipe.ResultCount == 0)
                    big.Recipe = null;

                string objDir = System.IO.Path.Combine(bigPath, big.Name);
                System.IO.Directory.CreateDirectory(bigPath);
                System.IO.File.WriteAllText(System.IO.Path.Combine(bigPath, "big-craftable.json"), JsonConvert.SerializeObject(big, settings));

                var image = big.Texture.MakeImage(ui.ModProjectDir);
                image.SavePng(System.IO.Path.Combine(bigPath, "big-craftable.png"));
                image.Dispose();

                int e = 2;
                foreach ( var imageRef in big.ReserveExtraIndices )
                {
                    var extraImage = big.Texture.MakeImage(ui.ModProjectDir);
                    extraImage.SavePng(System.IO.Path.Combine(bigPath, $"big-craftable-{e}.png"));
                    extraImage.Dispose();
                    ++e;
                }

                if (big.Recipe == null)
                    big.Recipe = new RecipeData();
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
            else if ( root == roots[ "Big Craftables" ] )
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
        }

        public override void OnRemoved(UI ui, ModData data_, TreeItem entry)
        {
            var data = data_ as JsonAssetsModData;
            if ( objects.ContainsKey( entry ) )
            {
                data.Objects.Remove(objects[entry]);
                objects.Remove(entry);
            }
            else if (bigs.ContainsKey(entry))
            {
                data.BigCraftables.Remove(bigs[entry]);
                bigs.Remove(entry);
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
            else if (bigs.ContainsKey(entry))
            {
                activeEditor = BigCraftableEditor.Instance();
                DoBigCraftableEditorConnections(activeEditor, entry);
            }

            return activeEditor;
        }

        public override void OnEditingAreaChanged(UI ui, ModData data, Node area)
        {
            activeEditor = null;
        }

        public override void OnResourceRenamed(UI ui, ModData data, string oldFilename, string newFilename)
        {
            // todo
        }

        public override void OnResourceDeleted(UI ui, ModData data, string filename)
        {
            // todo
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
            };

            foreach (var section in sections)
            {
                var item = ui.ProjectTree.CreateItem(entry);
                item.SetText(0, section);
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

                if (prop.Name == "SkillUnlockName")
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
                            GD.Print("edited name");
                            activeEntry.SetText(0, str);
                            prop.SetValue(obj, str);
                        });
                    }
                    lambda.SelfConnect(lineEdit, "text_changed");
                }
                else if (prop.PropertyType == typeof(int))
                {
                    string path = prop.Name + "/IntegerEdit";
                    var intEdit = editor.GetNode<IntegerEdit>(path);
                    intEdit.Value = (long?)(int)prop.GetValue(obj);
                    var lambda = new LambdaWrapper<bool, long>((has, value) => prop.SetValue(obj, (int)value));
                    if (prop.Name == "Price")
                    {
                        lambda = new LambdaWrapper<bool, long>((has, value) =>
                        {
                            obj.GetType().GetProperty("CanSell").SetValue(obj, has);
                            prop.SetValue(obj, (int)value);
                        });
                    }
                    else if (prop.Name == "Edibility")
                    {
                        var val = intEdit.Value;
                        intEdit.Value = val == -300 ? null : (long?)val;
                        lambda = new LambdaWrapper<bool, long>((has, value) =>
                        {
                            prop.SetValue(obj, has ? (int)value : -300);
                        });
                    }
                    else if (prop.Name == "PurchasePrice")
                    {
                        lambda = new LambdaWrapper<bool, long>((has, value) =>
                        {
                            obj.GetType().GetProperty("CanPurchase").SetValue(obj, has);
                            prop.SetValue(obj, (int)value);
                        });
                    }
                    lambda.SelfConnect(intEdit, nameof(IntegerEdit.int_edited));
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
                    lambda.SelfConnect(imageEditor, nameof(SubImageEditor.image_changed));
                }
                else if (prop.PropertyType == typeof(RecipeData))
                {
                    string path = prop.Name + "/RecipeEditor";
                    var recipeEditor = editor.GetNode(path);
                    DoObjectConnections(recipeEditor, (RecipeData)prop.GetValue(obj));
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
                    DoObjectConnections(buffsContainer, prop.GetValue(obj));
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
                else if (prop.PropertyType == typeof(Dictionary<string, string>))
                {
                    string path = prop.Name + "/LocalizationEditor";
                    var locEditor = editor.GetNode<LocalizationEditor>(path);
                    var locs = (Dictionary<string, string>)prop.GetValue(obj);
                    var entries = new List<Pair<string, string>>();
                    foreach (var loc in locs)
                    {
                        locEditor.AddEntry(loc.Key, loc.Value);
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
                else if (prop.PropertyType == typeof(List<ImageResourceReference>))
                {
                    // todo
                }
            }
        }
    }
}
