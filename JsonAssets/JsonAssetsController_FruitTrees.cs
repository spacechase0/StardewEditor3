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

            var intEdit = editor.GetNode<IntegerEdit>("Texture/SubImageEditor/Values/SubRectWidth/IntegerEdit");
            intEdit.Value = 432;
            intEdit.Editable = false;
            intEdit = editor.GetNode<IntegerEdit>("Texture/SubImageEditor/Values/SubRectHeight/IntegerEdit");
            intEdit.Value = 80;
            intEdit.Editable = false;

            intEdit = editor.GetNode<IntegerEdit>("SaplingTexture/SubImageEditor/Values/SubRectWidth/IntegerEdit");
            intEdit.Value = 16;
            intEdit.Editable = false;
            intEdit = editor.GetNode<IntegerEdit>("SaplingTexture/SubImageEditor/Values/SubRectHeight/IntegerEdit");
            intEdit.Value = 16;
            intEdit.Editable = false;
        }
    }
}
