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
        private FruitTreeData activeTree;

        private void DoFruitTreesEditorConnections(Node editor, TreeItem entry)
        {
            activeTree = trees[entry];

            DoEditorConnections(editor, activeTree);

            var intEdit = editor.GetNode<SpinBox>("Texture/SubImageEditor/Values/SubRectWidth/SpinBox");
            intEdit.Value = 432;
            intEdit.Editable = false;
            intEdit = editor.GetNode<SpinBox>("Texture/SubImageEditor/Values/SubRectHeight/SpinBox");
            intEdit.Value = 80;
            intEdit.Editable = false;

            intEdit = editor.GetNode<SpinBox>("SaplingTexture/SubImageEditor/Values/SubRectWidth/SpinBox");
            intEdit.Value = 16;
            intEdit.Editable = false;
            intEdit = editor.GetNode<SpinBox>("SaplingTexture/SubImageEditor/Values/SubRectHeight/SpinBox");
            intEdit.Value = 16;
            intEdit.Editable = false;
        }
    }
}
