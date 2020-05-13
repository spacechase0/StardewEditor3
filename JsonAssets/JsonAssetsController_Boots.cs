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
        private BootsData activeBoots;

        private void DoBootsEditorConnections(Node editor, TreeItem entry)
        {
            activeBoots = bootss[entry];

            DoEditorConnections(editor, activeBoots);
            var intEdit = editor.GetNode<SpinBox>("Texture/SubImageEditor/Values/SubRectWidth/SpinBox");
            intEdit.Value = 16;
            intEdit.Editable = false;
            intEdit = editor.GetNode<SpinBox>("Texture/SubImageEditor/Values/SubRectHeight/SpinBox");
            intEdit.Value = 16;
            intEdit.Editable = false;
        }
    }
}
