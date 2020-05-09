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
        private HatData activeHat;

        private void DoHatEditorConnections(Node editor, TreeItem entry)
        {
            activeHat = hats[entry];

            DoEditorConnections(editor, activeHat);
            var intEdit = editor.GetNode<SpinBox>("Texture/SubImageEditor/Values/SubRectWidth/SpinBox");
            intEdit.Value = 20;
            intEdit.Editable = false;
            intEdit = editor.GetNode<SpinBox>("Texture/SubImageEditor/Values/SubRectHeight/SpinBox");
            intEdit.Value = 80;
            intEdit.Editable = false;
        }
    }
}
