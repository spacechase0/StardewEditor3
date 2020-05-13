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
        private ShirtData activeShirt;

        private void DoShirtEditorConnections(Node editor, TreeItem entry)
        {
            activeShirt = shirts[entry];

            DoEditorConnections(editor, activeShirt);
            var intEdit = editor.GetNode<SpinBox>("MaleTexture/SubImageEditor/Values/SubRectWidth/SpinBox");
            intEdit.Value = 8;
            intEdit.Editable = false;
            intEdit = editor.GetNode<SpinBox>("MaleTexture/SubImageEditor/Values/SubRectHeight/SpinBox");
            intEdit.Value = 32;
            intEdit.Editable = false;

            intEdit = editor.GetNode<SpinBox>("MaleColorTexture/SubImageEditor/Values/SubRectWidth/SpinBox");
            intEdit.Value = 8;
            intEdit.Editable = false;
            intEdit = editor.GetNode<SpinBox>("MaleColorTexture/SubImageEditor/Values/SubRectHeight/SpinBox");
            intEdit.Value = 32;
            intEdit.Editable = false;

            intEdit = editor.GetNode<SpinBox>("FemaleTexture/SubImageEditor/Values/SubRectWidth/SpinBox");
            intEdit.Value = 8;
            intEdit.Editable = false;
            intEdit = editor.GetNode<SpinBox>("FemaleTexture/SubImageEditor/Values/SubRectHeight/SpinBox");
            intEdit.Value = 32;
            intEdit.Editable = false;

            intEdit = editor.GetNode<SpinBox>("FemaleColorTexture/SubImageEditor/Values/SubRectWidth/SpinBox");
            intEdit.Value = 8;
            intEdit.Editable = false;
            intEdit = editor.GetNode<SpinBox>("FemaleColorTexture/SubImageEditor/Values/SubRectHeight/SpinBox");
            intEdit.Value = 32;
            intEdit.Editable = false;
        }
    }
}
