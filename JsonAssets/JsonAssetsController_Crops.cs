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

            var intEdit = editor.GetNode<SpinBox>("Texture/SubImageEditor/Values/SubRectWidth/SpinBox");
            intEdit.Value = 128;
            intEdit.Editable = false;
            intEdit = editor.GetNode<SpinBox>("Texture/SubImageEditor/Values/SubRectHeight/SpinBox");
            intEdit.Value = 32;
            intEdit.Editable = false;

            intEdit = editor.GetNode<SpinBox>("SeedTexture/SubImageEditor/Values/SubRectWidth/SpinBox");
            intEdit.Value = 16;
            intEdit.Editable = false;
            intEdit = editor.GetNode<SpinBox>("SeedTexture/SubImageEditor/Values/SubRectHeight/SpinBox");
            intEdit.Value = 16;
            intEdit.Editable = false;

            intEdit = editor.GetNode<SpinBox>("GiantTexture/SubImageEditor/Values/SubRectWidth/SpinBox");
            intEdit.Value = 48;
            intEdit.Editable = false;
            intEdit = editor.GetNode<SpinBox>("GiantTexture/SubImageEditor/Values/SubRectHeight/SpinBox");
            intEdit.Value = 63;
            intEdit.Editable = false;
        }
    }
}
