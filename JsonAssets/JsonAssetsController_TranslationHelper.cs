using Godot;
using StardewEditor3.JsonAssets.Data;
using StardewEditor3.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.JsonAssets
{
    public partial class JsonAssetsController
    {
        private void DoTranslationHelperEditorConnections(Node editor, TreeItem entry, JsonAssetsModData data)
        {
            var keys = editor.GetNode<VBoxContainer>("PanelContainer/Keys");
            var grid = editor.GetNode<GridContainer>("HSplitContainer/Values/VBoxContainer/Grid");
            var english = editor.GetNode<VBoxContainer>("HSplitContainer/PanelContainer/English");

            foreach ( var item in data.Objects )
            {
                AddTranslation(keys, grid, english, "Object.Name." + item.Name, item.Name, item.NameLocalization);
                AddTranslation(keys, grid, english, "Object.Description." + item.Name, item.Description, item.DescriptionLocalization);
            }
            foreach (var item in data.Crops)
            {
                AddTranslation(keys, grid, english, "Crop.SeedName." + item.Name, item.SeedName, item.SeedNameLocalization);
                AddTranslation(keys, grid, english, "Crop.SeedDescription." + item.Name, item.SeedDescription, item.SeedDescriptionLocalization);
            }
            foreach (var item in data.FruitTrees)
            {
                AddTranslation(keys, grid, english, "FruitTree.SaplingName." + item.Name, item.SaplingName, item.SaplingNameLocalization);
                AddTranslation(keys, grid, english, "FruitTree.SaplingDescription." + item.Name, item.SaplingDescription, item.SaplingDescriptionLocalization);
            }
            foreach (var item in data.Hats)
            {
                AddTranslation(keys, grid, english, "Hat.Name." + item.Name, item.Name, item.NameLocalization);
                AddTranslation(keys, grid, english, "Hat.Description." + item.Name, item.Description, item.DescriptionLocalization);
            }
            foreach (var item in data.Weapons)
            {
                AddTranslation(keys, grid, english, "Weapon.Name." + item.Name, item.Name, item.NameLocalization);
                AddTranslation(keys, grid, english, "Weapon.Description." + item.Name, item.Description, item.DescriptionLocalization);
            }
            foreach (var item in data.Shirts)
            {
                AddTranslation(keys, grid, english, "Shirt.Name." + item.Name, item.Name, item.NameLocalization);
                AddTranslation(keys, grid, english, "Shirt.Description." + item.Name, item.Description, item.DescriptionLocalization);
            }
            foreach (var item in data.Pantss)
            {
                AddTranslation(keys, grid, english, "Pants.Name." + item.Name, item.Name, item.NameLocalization);
                AddTranslation(keys, grid, english, "Pants.Description." + item.Name, item.Description, item.DescriptionLocalization);
            }
            foreach (var item in data.Bootss)
            {
                AddTranslation(keys, grid, english, "Boots.Name." + item.Name, item.Name, item.NameLocalization);
                AddTranslation(keys, grid, english, "Boots.Description." + item.Name, item.Description, item.DescriptionLocalization);
            }
        }

        private void AddTranslation(VBoxContainer keys, GridContainer values, VBoxContainer englishContainer, string key, string english, Dictionary<string, string> locales)
        {
            var label = new Label() { Text = key, RectMinSize = new Vector2(0, 32) };
            keys.AddChild(label);

            var lineEdit = new LineEdit() { Text = english, Editable = false };
            lineEdit.AddFontOverride("font", UI.InternationalFont);
            englishContainer.AddChild(lineEdit);

            foreach ( var lang in Languages.GetList() )
            {
                string code = Languages.LanguageToCode(lang);
                if (!locales.ContainsKey(code))
                    locales.Add(code, "");

                lineEdit = new LineEdit() { Text = locales[code] };
                lineEdit.AddFontOverride("font", UI.InternationalFont);
                values.AddChild(lineEdit);

                var lambda = new LambdaWrapper<string>((str) => locales[code] = str);
                lambda.SelfConnect(lineEdit, "text_changed");
            }
        }
    }
}
