using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using StardewEditor3.Util;

namespace StardewEditor3.ContentPatcher
{
    public partial class ContentPatcherController : ContentPackController
    {
        public const string MOD_NAME = "Content Patcher";
        public const string MOD_UNIQUE_ID = "Pathoschild.ContentPatcher";
        public const string MOD_ABBREVIATION = "CP";

        private readonly Dictionary<string, TreeItem> roots = new Dictionary<string, TreeItem>();
        private readonly Dictionary<TreeItem, ConfigToken> configs = new Dictionary<TreeItem, ConfigToken>();

        private TreeItem activeEntry;
        private Node activeEditor;

        private readonly PackedScene ConfigTokenEditor = GD.Load<PackedScene>("res://ContentPatcher/ConfigTokenEditor.tscn");

        public ContentPatcherController()
        :   base(MOD_NAME, MOD_UNIQUE_ID, MOD_ABBREVIATION)
        {
        }

        public override ModData OnModCreated(UI ui, TreeItem entry)
        {
            AddSections(ui, entry);
            return new ContentPatcherModData();
        }

        public override void OnSave(UI ui, ModData data)
        {
            // todo
        }

        public override void OnLoad(UI ui, ModData data_, TreeItem entry)
        {
            var data = data_ as ContentPatcherModData;

            AddSections(ui, entry);

            var configRoot = roots["Configuration Entries"];
            foreach ( var config in data.ConfigTokens )
            {
                var item = ui.ProjectTree.CreateItem(configRoot);
                item.SetText(0, config.Name);
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this configuration entry");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                configs.Add(item, config);
            }
        }

        public override void OnValidate(UI ui, ModData data_, List<string> errors)
        {
            var data = data_ as ContentPatcherModData;

            // todo
        }

        public override void OnImport(UI ui, ModData data_, string importPath)
        {
            var data = data_ as ContentPatcherModData;

            // todo
        }

        public override void OnExport(UI ui, ModData data_, string exportPath)
        {
            var data = data_ as ContentPatcherModData;
            
            // todo
        }

        public override void OnAdded(UI ui, ModData data_, TreeItem root)
        {
            var data = data_ as ContentPatcherModData;

            if ( root == roots[ "Configuration Entries" ] )
            {
                var configToken = new ConfigToken()
                {
                    Name = "ConfigToken",
                };
                data.ConfigTokens.Add(configToken);

                var item = ui.ProjectTree.CreateItem(root);
                item.SetText(0, "ConfigToken");
                item.AddButton(0, ui.RemoveIcon, UI.REMOVE_BUTTON_INDEX, tooltip: "Remove this configuration entry");
                item.SetMeta(Meta.CorrespondingController, MOD_UNIQUE_ID);
                configs.Add(item, configToken);
            }
        }

        public override void OnRemoved(UI ui, ModData data_, TreeItem entry)
        {
            var data = data_ as ContentPatcherModData;

            if ( configs.ContainsKey( entry ) )
            {
                data.ConfigTokens.Remove(configs[entry]);
                configs.Remove(entry);
            }
        }

        public override Node CreateMainEditingArea(UI ui, ModData data_, TreeItem entry)
        {
            var data = data_ as ContentPatcherModData;

            activeEntry = entry;
            if ( configs.ContainsKey( entry ) )
            {
                activeEditor = ConfigTokenEditor.Instance();
                DoConfigTokenEditorConnections(activeEditor, entry);
            }

            activeEditor.SetMeta(Meta.CorrespondingController, ModUniqueId);
            return activeEditor;
        }

        public override void OnEditingAreaChanged(UI ui, ModData data_, Node area)
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
                "Configuration Entries",
                "Dynamic Tokens",
                "Patches",
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
                
                if (prop.PropertyType == typeof(string))
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
                    lambda.SelfConnect(imageEditor, nameof(SubImageEditor.image_changed));
                }
            }
        }
    }
}
