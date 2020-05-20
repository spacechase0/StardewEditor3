using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.ContentPatcher
{
    public partial class ContentPatcherController
    {
        private ConfigToken activeConfig;
        
        public void DoConfigTokenEditorConnections(Node editor, TreeItem entry)
        {
            activeConfig = configs[entry];

            DoEditorConnections(editor, activeConfig);
        }
    }
}
