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
            if (activeObj.GiftTastes == null)
                activeObj.GiftTastes = new ObjectData.GiftTastes_();

            DoEditorConnections(editor, activeObj);
            var intEdit = editor.GetNode<SpinBox>("Texture/SubImageEditor/Values/SubRectWidth/SpinBox");
            intEdit.Value = 16;
            intEdit.Editable = false;
            intEdit = editor.GetNode<SpinBox>("Texture/SubImageEditor/Values/SubRectHeight/SpinBox");
            intEdit.Value = 16;
            intEdit.Editable = false;
            intEdit = editor.GetNode<SpinBox>("TextureColor/SubImageEditor/Values/SubRectWidth/SpinBox");
            intEdit.Value = 16;
            intEdit.Editable = false;
            intEdit = editor.GetNode<SpinBox>("TextureColor/SubImageEditor/Values/SubRectHeight/SpinBox");
            intEdit.Value = 16;
            intEdit.Editable = false;
        }
    }
}
