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
        private TailoringRecipeData activeTailoring;

        private void DoTailoringRecipeEditorConnections(Node editor, TreeItem entry)
        {
            activeTailoring = tailorings[entry];

            DoEditorConnections(editor, activeTailoring);
        }
    }
}
