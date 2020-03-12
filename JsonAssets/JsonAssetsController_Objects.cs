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
        private ObjectData activeObj;

        private void DoObjectEditorConnections(Node editor, TreeItem entry)
        {
            activeObj = objects[entry];
            if (activeObj.Recipe == null)
                activeObj.Recipe = new RecipeData();
            if (activeObj.EdibleBuffs == null)
                activeObj.EdibleBuffs = new ObjectData.FoodBuffs_();

            DoObjectConnections(editor, activeObj);
            var intEdit = editor.GetNode<IntegerEdit>("Texture/SubImageEditor/Values/SubRectWidth/IntegerEdit");
            intEdit.Value = 16;
            intEdit.Editable = false;
            intEdit = editor.GetNode<IntegerEdit>("Texture/SubImageEditor/Values/SubRectHeight/IntegerEdit");
            intEdit.Value = 16;
            intEdit.Editable = false;
            intEdit = editor.GetNode<IntegerEdit>("TextureColor/SubImageEditor/Values/SubRectWidth/IntegerEdit");
            intEdit.Value = 16;
            intEdit.Editable = false;
            intEdit = editor.GetNode<IntegerEdit>("TextureColor/SubImageEditor/Values/SubRectHeight/IntegerEdit");
            intEdit.Value = 16;
            intEdit.Editable = false;
        }

        private void DoObjectConnections<T>(Node editor, T obj)
        {
            foreach (var prop in obj.GetType().GetProperties())
            {
                if (Attribute.IsDefined(prop, typeof(DoNotAutoConnectAttribute)))
                    continue;

                if ( prop.Name == "SkillUnlockName" )
                {
                    string path = prop.Name + "/OptionButton";
                    var optionButton = editor.GetNode<OptionButton>(path);
                    int selInd = 0;
                    for (int i = 0; i < optionButton.GetItemCount(); ++i)
                    {
                        if (optionButton.GetItemText(i) == (string) prop.GetValue(obj))
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
                    else if ( prop.Name == "PurchasePrice" )
                    {
                        lambda = new LambdaWrapper<bool, long>((has, value) =>
                        {
                            obj.GetType().GetProperty("CanPurchase").SetValue(obj, has);
                            prop.SetValue(obj, (int)value);
                        });
                    }
                    lambda.SelfConnect(intEdit, nameof(IntegerEdit.int_edited));
                }
                else if ( prop.PropertyType == typeof(bool))
                {
                    string path = prop.Name + "/CheckBox";
                    if (prop.Name.StartsWith("Can"))
                        path = "CanFlags/" + prop.Name.Substring(3);
                    var checkBox = editor.GetNode<CheckBox>(path);
                    checkBox.Pressed = (bool) prop.GetValue(obj);
                    var lambda = new LambdaWrapper<bool>((value) => prop.SetValue(obj, value));
                    lambda.SelfConnect(checkBox, "toggled");
                }
                else if ( prop.PropertyType.IsEnum )
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
                else if ( prop.PropertyType == typeof(List<string>) )
                {
                    string path = prop.Name + "/StringListEditor";
                    var stringsEditor = editor.GetNode<StringListEditor>(path);
                    var strings = (List<string>)prop.GetValue(obj);
                    foreach (var entry in strings)
                        stringsEditor.AddString(entry);
                    var lambdaAdd = new LambdaWrapper(() => strings.Add(""));
                    var lambdaDelete = new LambdaWrapper<int> ((ind) => strings.RemoveAt(ind));
                    var lambdaEdit = new LambdaWrapper<int, string>((ind, str) => strings[ind] = str);
                    lambdaAdd.SelfConnect(stringsEditor, nameof(StringListEditor.entry_added));
                    lambdaDelete.SelfConnect(stringsEditor, nameof(StringListEditor.entry_deleted));
                    lambdaEdit.SelfConnect(stringsEditor, nameof(StringListEditor.entry_changed));
                }
                else if ( prop.PropertyType == typeof(ImageResourceReference) )
                {
                    string path = prop.Name + "/SubImageEditor";
                    var imageEditor = editor.GetNode<SubImageEditor>(path);
                    imageEditor.SetValues((ImageResourceReference)prop.GetValue(obj));
                    var lambda = new LambdaWrapper<SubImageEditor>((ie) => prop.SetValue(obj, ie.GetImageRef()));
                    if ( prop.Name == "TextureColor" )
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
                else if ( prop.PropertyType == typeof(RecipeData) )
                {
                    string path = prop.Name + "/RecipeEditor";
                    var recipeEditor = editor.GetNode(path);
                    DoObjectConnections(recipeEditor, (RecipeData) prop.GetValue(obj));
                }
                else if ( prop.PropertyType == typeof(List<RecipeData.Ingredient>) )
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
                else if ( prop.PropertyType == typeof(ObjectData.FoodBuffs_) )
                {
                    string path = prop.Name + "/Buffs";
                    var buffsCountainer = editor.GetNode(path);
                    DoObjectConnections(buffsCountainer, prop.GetValue(obj));
                }
            }
        }
    }
}
