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
        private PantsData activePants;

        private void DoPantsEditorConnections(Node editor, TreeItem entry)
        {
            activePants = pantss[entry];

            DoEditorConnections(editor, activePants);
            var intEdit = editor.GetNode<SpinBox>("Texture/SubImageEditor/Values/SubRectWidth/SpinBox");
            intEdit.Value = 192;
            intEdit.Editable = false;
            intEdit = editor.GetNode<SpinBox>("Texture/SubImageEditor/Values/SubRectHeight/SpinBox");
            intEdit.Value = 688;
            intEdit.Editable = false;
        }
    }
}
