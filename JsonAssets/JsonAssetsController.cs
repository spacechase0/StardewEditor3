using Godot;
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

        private TreeItem activeEntry;
        private Node activeEditor;

        private readonly PackedScene ObjectEditor = GD.Load<PackedScene>("res://JsonAssets/ObjectEditor.tscn");
        private readonly PackedScene BigCraftableEditor = GD.Load<PackedScene>("res://JsonAssets/BigCraftableEditor.tscn");
        private readonly PackedScene CropEditor = GD.Load<PackedScene>("res://JsonAssets/CropEditor.tscn");

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

            var bigRoot = roots["Big Craftables"];
            foreach (var big in data.BigCraftables)
            {
                var item = ui.ProjectTree.CreateItem(bigRoot);
                item.SetText(0, big.Name);
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this big craftable");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                bigs.Add(item, big);
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
                {
                    GD.Print("debug:texture for " + obj.Name + " is " + obj.Texture + " " + obj.Texture?.Resource);
                    errors.Add($"Object \"{obj.Name}\" must have a texture.");
                }
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

                string bigDir = System.IO.Path.Combine(bigPath, big.Name);
                System.IO.Directory.CreateDirectory(bigDir);
                System.IO.File.WriteAllText(System.IO.Path.Combine(bigDir, "big-craftable.json"), JsonConvert.SerializeObject(big, settings));

                var image = big.Texture.MakeImage(ui.ModProjectDir);
                image.SavePng(System.IO.Path.Combine(bigDir, "big-craftable.png"));
                image.Dispose();

                int e = 2;
                foreach ( var imageRef in big.ReserveExtraIndices )
                {
                    var extraImage = imageRef.MakeImage(ui.ModProjectDir);
                    extraImage.SavePng(System.IO.Path.Combine(bigDir, $"big-craftable-{e}.png"));
                    extraImage.Dispose();
                    ++e;
                }

                if (big.Recipe == null)
                    big.Recipe = new RecipeData();
            }
            
            string cropPath = System.IO.Path.Combine(path, "Crops");
            System.IO.Directory.CreateDirectory(cropPath);
            foreach (var crop in data.Crops)
            {
                if (crop.Bonus.MinimumPerHarvest == 0)
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
            else if ( crops.ContainsKey(entry) )
            {
                data.Crops.Remove(crops[entry]);
                crops.Remove(entry);
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
            else if (crops.ContainsKey(entry))
            {
                activeEditor = CropEditor.Instance();
                DoCropsEditorConnections(activeEditor, entry);
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
                else if (prop.PropertyType == typeof(double))
                {
                    if ( !editor.HasNode(prop.Name + "/PercentLabel") )
                    {
                        GD.PrintErr("Doubles not supported except in percents with IntegerEdit");
                        continue;
                    }
                    string path = prop.Name + "/IntegerEdit";
                    var intEdit = editor.GetNode<IntegerEdit>(path);
                    intEdit.Value = (long?)(int)((double)prop.GetValue(obj) * 100);
                    var lambda = new LambdaWrapper<bool, long>((has, value) => prop.SetValue(obj, ((int)value) / 100.0));
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
            }
        }
    }
}
