using Godot;
using System;

public class UI : MarginContainer
{
	private Tree tree;
	private Texture editIcon;
	
	public override void _Ready()
	{
		editIcon = GD.Load<Texture>("res://res/icons/edit.png");

		tree = GetNode<Tree>("MenuSeparator/Splitter/Left/ProjectTree");
		tree.Connect("item_rmb_edited", this, nameof(Signal_ItemRmbEdited));
		tree.Connect("item_rmb_selected", this, nameof(Signal_ItemRmbEdited2));

		var root = tree.CreateItem();
		root.SetText(0, "My Project");
		root.DisableFolding = true;

		var cp = tree.CreateItem(root);
		cp.SetText(0, "[CP] My Project");
		{
			var editImages = tree.CreateItem(cp);
			editImages.SetText(0, "Patches");
			{
				var a = tree.CreateItem(editImages);
				a.SetText(0, "Test Patch");
				a.AddButton(0, editIcon);
			}
		}

		var ja = tree.CreateItem(root);
		ja.SetText(0, "[JA] My Project");
		{
			var objs = tree.CreateItem(ja);
			objs.SetText(0, "Objects");
			{
				var a = tree.CreateItem(objs);
				a.SetText(0, "Test Item");
			}
		}
	}

	private void Signal_ItemRmbEdited()
	{
	}

	private void Signal_ItemRmbEdited2(Vector2 pos)
	{
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
