using Godot;
using JsonAssets.Data;
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
                activeObj.Recipe = new ObjectData.Recipe_();
            if (activeObj.EdibleBuffs == null)
                activeObj.EdibleBuffs = new ObjectData.FoodBuffs_();

            var lineEdit = editor.GetNode<LineEdit>("Name/LineEdit");
            lineEdit.Text = activeObj.Name;
            lineEdit.Connect("text_changed", this, nameof(Signal_ObjectNameEdited));

            lineEdit = editor.GetNode<LineEdit>("EnableWithMod/LineEdit");
            lineEdit.Text = activeObj.EnableWithMod;
            lineEdit.Connect("text_changed", this, nameof(Signal_ObjectEnableWithModEdited));

            lineEdit = editor.GetNode<LineEdit>("DisableWithMod/LineEdit");
            lineEdit.Text = activeObj.DisableWithMod;
            lineEdit.Connect("text_changed", this, nameof(Signal_ObjectDisableWithModEdited));

            var imageEditor = editor.GetNode<SubImageEditor>("Texture/SubImageEditor");
            var intEdit = imageEditor.GetNode<IntegerEdit>("Values/SubRectWidth/IntegerEdit");
            intEdit.Value = 16;
            intEdit.Editable = false;
            intEdit = imageEditor.GetNode<IntegerEdit>("Values/SubRectHeight/IntegerEdit");
            intEdit.Value = 16;
            intEdit.Editable = false;
            imageEditor.SetValues(activeObj.Texture);
            imageEditor.Connect(nameof(SubImageEditor.image_changed), this, nameof(Signal_ObjectTextureEdited));

            imageEditor = editor.GetNode<SubImageEditor>("ColorTexture/SubImageEditor");
            intEdit = imageEditor.GetNode<IntegerEdit>("Values/SubRectWidth/IntegerEdit");
            intEdit.Value = 16;
            intEdit.Editable = false;
            intEdit = imageEditor.GetNode<IntegerEdit>("Values/SubRectHeight/IntegerEdit");
            intEdit.Value = 16;
            intEdit.Editable = false;
            imageEditor.SetValues(activeObj.TextureColor);
            imageEditor.Connect(nameof(SubImageEditor.image_changed), this, nameof(Signal_ObjectColorTextureEdited));

            lineEdit = editor.GetNode<LineEdit>("Description/LineEdit");
            lineEdit.Text = activeObj.Description;
            lineEdit.Connect("text_changed", this, nameof(Signal_ObjectDescriptionEdited));

            var optionButton = editor.GetNode<OptionButton>("Category/OptionButton");
            int selInd = 0;
            for (int i = 0; i < optionButton.GetItemCount(); ++i)
            {
                if (optionButton.GetItemText(i) == activeObj.Category.ToString())
                {
                    selInd = i;
                    break;
                }
            }
            optionButton.Selected = selInd;
            optionButton.Connect("item_selected", this, nameof(Signal_ObjectCategoryEdited));

            lineEdit = editor.GetNode<LineEdit>("CategoryTextOverride/LineEdit");
            lineEdit.Text = activeObj.CategoryTextOverride;
            lineEdit.Connect("text_changed", this, nameof(Signal_ObjectCategoryTextOverrideEdited));

            var colorPicker = editor.GetNode<ColorPickerButton>("CategoryColorOverride/ColorPickerButton");
            colorPicker.Color = activeObj.CategoryColorOverride;
            colorPicker.Connect("color_changed", this, nameof(Signal_ObjectCategoryColorOverrideEdited));

            intEdit = editor.GetNode<IntegerEdit>("Price/IntegerEdit");
            intEdit.Value = activeObj.CanSell ? (long?) activeObj.Price : null;
            intEdit.Connect("int_edited", this, nameof(Signal_ObjectSellPriceEdited));

            var checkBox = editor.GetNode<CheckBox>("CanFlags/Trash");
            checkBox.Pressed = activeObj.CanTrash;
            checkBox.Connect("toggled", this, nameof(Signal_ObjectCanTrashEdited));

            checkBox = editor.GetNode<CheckBox>("CanFlags/Gift");
            checkBox.Pressed = activeObj.CanBeGifted;
            checkBox.Connect("toggled", this, nameof(Signal_ObjectCanGiftEdited));

            optionButton = editor.GetNode<OptionButton>("Recipe/RecipeEditor/SkillUnlockName/OptionButton");
            selInd = -1;
            for (int i = 0; i < optionButton.GetItemCount(); ++i)
            {
                if (optionButton.GetItemText(i) == activeObj.Recipe.SkillUnlockName?.ToString())
                {
                    selInd = i;
                    break;
                }
            }
            optionButton.Selected = selInd;
            optionButton.Connect("item_selected", this, nameof(Signal_ObjectRecipeSkillUnlockNameEdited));

            intEdit = editor.GetNode<IntegerEdit>("Recipe/RecipeEditor/SkillUnlockLevel/IntegerEdit");
            intEdit.Value = activeObj.Recipe.SkillUnlockLevel != -1 ? (long?) activeObj.Recipe.SkillUnlockLevel : null;
            intEdit.Connect("int_edited", this, nameof(Signal_ObjectRecipeSkillUnlockLevelEdited));
            
            intEdit = editor.GetNode<IntegerEdit>("Recipe/RecipeEditor/ResultCount/IntegerEdit");
            intEdit.Value = activeObj.Recipe.ResultCount;
            intEdit.Connect("int_edited", this, nameof(Signal_ObjectRecipeResultCountEdited));

            var ingredEditor = editor.GetNode<IngredientListEditor>("Recipe/RecipeEditor/Ingredients/IngredientListEditor");
            foreach ( var ingred in activeObj.Recipe.Ingredients )
                ingredEditor.AddEntry(ingred.Object?.ToString(), ingred.Count);
            ingredEditor.Connect(nameof(IngredientListEditor.entry_added), this, nameof(Signal_ObjectRecipeIngredientAdded));
            ingredEditor.Connect(nameof(IngredientListEditor.entry_deleted), this, nameof(Signal_ObjectRecipeIngredientDeleted));
            ingredEditor.Connect(nameof(IngredientListEditor.entry_changed), this, nameof(Signal_ObjectRecipeIngredientEdited));

            checkBox = editor.GetNode<CheckBox>("Recipe/RecipeEditor/IsDefault/CheckBox");
            checkBox.Pressed = activeObj.Recipe.IsDefault;
            checkBox.Connect("toggled", this, nameof(Signal_ObjectRecipeIsDefaultEdited));

            intEdit = editor.GetNode<IntegerEdit>("Recipe/RecipeEditor/PurchasePrice/IntegerEdit");
            intEdit.Value = activeObj.CanPurchase ? (long?) activeObj.PurchasePrice : null;
            intEdit.Connect("int_edited", this, nameof(Signal_ObjectRecipePurchasePriceEdited));

            lineEdit = editor.GetNode<LineEdit>("Recipe/RecipeEditor/PurchaseFrom/LineEdit");
            lineEdit.Text = activeObj.PurchaseFrom;
            lineEdit.Connect("text_changed", this, nameof(Signal_ObjectRecipePurchaseFromEdited));

            var stringsEditor = editor.GetNode<StringListEditor>("Recipe/RecipeEditor/PurchaseRequirements/StringListEditor");
            foreach (var req in activeObj.Recipe.PurchaseRequirements)
                stringsEditor.AddString(req);
            stringsEditor.Connect(nameof(StringListEditor.entry_added), this, nameof(Signal_ObjectRecipePurchaseRequirementAdded));
            stringsEditor.Connect(nameof(StringListEditor.entry_deleted), this, nameof(Signal_ObjectRecipePurchaseRequirementDeleted));
            stringsEditor.Connect(nameof(StringListEditor.entry_changed), this, nameof(Signal_ObjectRecipePurchaseRequirementEdited));
        }

        private void Signal_ObjectNameEdited(string str)
        {
            activeObj.Name = str;
            activeEntry.SetText(0, str);
        }

        private void Signal_ObjectEnableWithModEdited(string str)
        {
            activeObj.EnableWithMod = str;
        }

        private void Signal_ObjectDisableWithModEdited(string str)
        {
            activeObj.DisableWithMod = str;
        }

        private void Signal_ObjectTextureEdited(SubImageEditor imageEditor)
        {
            activeObj.Texture = imageEditor.GetImageRef();
        }

        private void Signal_ObjectColorTextureEdited(SubImageEditor imageEditor)
        {
            activeObj.TextureColor = imageEditor.GetImageRef();
        }

        private void Signal_ObjectDescriptionEdited(string str)
        {
            activeObj.Description = str;
        }

        private void Signal_ObjectCategoryEdited(int idx)
        {
            var optionButton = activeEditor.GetNode<OptionButton>("Category/OptionButton");
            var str = optionButton.GetItemText(idx);

            activeObj.Category = ( ObjectData.Category_ ) Enum.Parse(typeof(ObjectData.Category_), str);
        }

        private void Signal_ObjectCategoryTextOverrideEdited(string str)
        {
            activeObj.CategoryTextOverride = str;
        }

        private void Signal_ObjectCategoryColorOverrideEdited(Color col)
        {
            activeObj.CategoryColorOverride = col;
        }

        private void Signal_ObjectSellPriceEdited(bool has, long value)
        {
            if ( has )
            {
                activeObj.CanSell = true;
                activeObj.Price = (int) value;
            }
            else
            {
                activeObj.CanSell = false;
            }
        }

        private void Signal_ObjectCanTrashEdited(bool value)
        {
            activeObj.CanTrash = value;
        }

        private void Signal_ObjectCanGiftEdited(bool value)
        {
            activeObj.CanBeGifted = value;
        }

        private void Signal_ObjectRecipeSkillUnlockNameEdited(int idx)
        {
            var optionButton = activeEditor.GetNode<OptionButton>("Recipe/RecipeEditor/SkillUnlockName/OptionButton");
            var str = optionButton.GetItemText(idx);

            activeObj.Recipe.SkillUnlockName = str;
        }

        private void Signal_ObjectRecipeSkillUnlockLevelEdited(bool _has, long value)
        {
            activeObj.Recipe.SkillUnlockLevel = (int) value;
        }

        private void Signal_ObjectRecipeResultCountEdited(bool _has, long value)
        {
            activeObj.Recipe.ResultCount = (int) value;
        }

        private void Signal_ObjectRecipeIngredientAdded()
        {
            activeObj.Recipe.Ingredients.Add(new ObjectData.Recipe_.Ingredient());
        }

        private void Signal_ObjectRecipeIngredientDeleted(int ind)
        {
            activeObj.Recipe.Ingredients.RemoveAt(ind);
        }

        private void Signal_ObjectRecipeIngredientEdited(int ind, string ingred, int amt)
        {
            if (int.TryParse(ingred, out int ingredId))
                activeObj.Recipe.Ingredients[ind].Object = ingredId;
            else
                activeObj.Recipe.Ingredients[ind].Object = ingred;
            activeObj.Recipe.Ingredients[ind].Count = amt;
        }

        private void Signal_ObjectRecipeIsDefaultEdited(bool value)
        {
            activeObj.Recipe.IsDefault = value;
        }

        private void Signal_ObjectRecipePurchasePriceEdited(bool has, long value)
        {
            activeObj.CanPurchase = has;
            activeObj.PurchasePrice = (int) value;
        }

        private void Signal_ObjectRecipePurchaseFromEdited(string str)
        {
            activeObj.PurchaseFrom = str;
        }

        private void Signal_ObjectRecipePurchaseRequirementAdded()
        {
            activeObj.Recipe.PurchaseRequirements.Add("");
        }

        private void Signal_ObjectRecipePurchaseRequirementDeleted(int ind)
        {
            activeObj.Recipe.PurchaseRequirements.RemoveAt(ind);
        }

        private void Signal_ObjectRecipePurchaseRequirementEdited(int ind, string str)
        {
            activeObj.Recipe.PurchaseRequirements[ind] = str;
        }
    }
}
