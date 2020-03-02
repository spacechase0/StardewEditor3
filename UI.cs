using Godot;
using StardewEditor3.ContentPackControllers;
using System;

public class UI : MarginContainer
{
	public Tree ProjectTree { get; set; }

	public Texture EditIcon { get; set; }
	public Texture AddIcon { get; set; }
	public Texture RemoveIcon { get; set; }
	
	public override void _Ready()
	{
		EditIcon = GD.Load<Texture>("res://res/icons/edit.png");
		AddIcon = GD.Load<Texture>("res://res/icons/add.png");
		RemoveIcon = GD.Load<Texture>("res://res/icons/remove.png");

		var fileMenu = GetNode<MenuButton>("MenuSeparator/MenuPanel/Menu/File");
		var filePopup = fileMenu.GetPopup();
		filePopup.AddItem("New project...");
		filePopup.AddSubmenuItem("New content pack...", "NewMod");
		var newModPopup = fileMenu.GetNode<PopupMenu>("NewMod");
		foreach ( var controllerId in ContentPackController.GetRegisteredControllerTypes() )
		{
			var controller = ContentPackController.GetControllerForMod(controllerId);
			newModPopup.AddItem(controller.ModName);
		}

		ProjectTree = GetNode<Tree>("MenuSeparator/Splitter/Left/ProjectTree");
		ProjectTree.Connect("item_activated", this, nameof(Signal_CellActivated));

		var root = ProjectTree.CreateItem();
		root.SetText(0, "My Project");
		root.DisableFolding = true;

		var ja = ProjectTree.CreateItem(root);
		ja.SetText(0, "[JA] My Project");
		ContentPackController.GetControllerForMod(JsonAssetsController.MOD_UNIQUE_ID).OnModCreated(this, ja);
	}

	private void Signal_CellActivated()
	{
	}
}
