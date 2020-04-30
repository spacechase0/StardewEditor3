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
        private CropData activeCrop;

        private void DoCropsEditorConnections(Node editor, TreeItem entry)
        {
            activeCrop = crops[entry];
            if (activeCrop.Bonus == null)
                activeCrop.Bonus = new CropData.Bonus_();

            DoEditorConnections(editor, activeCrop);

            var intEdit = editor.GetNode<IntegerEdit>("Texture/SubImageEditor/Values/SubRectWidth/IntegerEdit");
            intEdit.Value = 128;
            intEdit.Editable = false;
            intEdit = editor.GetNode<IntegerEdit>("Texture/SubImageEditor/Values/SubRectHeight/IntegerEdit");
            intEdit.Value = 32;
            intEdit.Editable = false;

            intEdit = editor.GetNode<IntegerEdit>("SeedTexture/SubImageEditor/Values/SubRectWidth/IntegerEdit");
            intEdit.Value = 16;
            intEdit.Editable = false;
            intEdit = editor.GetNode<IntegerEdit>("SeedTexture/SubImageEditor/Values/SubRectHeight/IntegerEdit");
            intEdit.Value = 16;
            intEdit.Editable = false;

            intEdit = editor.GetNode<IntegerEdit>("GiantTexture/SubImageEditor/Values/SubRectWidth/IntegerEdit");
            intEdit.Value = 48;
            intEdit.Editable = false;
            intEdit = editor.GetNode<IntegerEdit>("GiantTexture/SubImageEditor/Values/SubRectHeight/IntegerEdit");
            intEdit.Value = 63;
            intEdit.Editable = false;
        }
    }
}
