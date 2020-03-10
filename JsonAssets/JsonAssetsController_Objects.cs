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
    }
}
