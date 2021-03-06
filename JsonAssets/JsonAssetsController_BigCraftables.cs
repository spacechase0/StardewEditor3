﻿using Godot;
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
        private BigCraftableData activeBig;

        private void DoBigCraftableEditorConnections(Node editor, TreeItem entry)
        {
            activeBig = bigs[entry];
            if (activeBig.Recipe == null)
                activeBig.Recipe = new RecipeData();

            DoEditorConnections(editor, activeBig);
            var intEdit = editor.GetNode<SpinBox>("Texture/SubImageEditor/Values/SubRectWidth/SpinBox");
            intEdit.Value = 16;
            intEdit.Editable = false;
            intEdit = editor.GetNode<SpinBox>("Texture/SubImageEditor/Values/SubRectHeight/SpinBox");
            intEdit.Value = 32;
            intEdit.Editable = false;
            intEdit = editor.GetNode<SpinBox>("ReserveExtraIndices/SubImageEditor/Values/SubRectHeight/SpinBox");
            intEdit.Value = 32;
            intEdit.Editable = false;
        }
    }
}
