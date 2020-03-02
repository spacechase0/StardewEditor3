using Godot;
using StardewEditor3.ContentPackControllers;
using System;

public class UI : MarginContainer
{
	public Tree ProjectTree { get; set; }
    public TreeItem ProjectRoot { get; set; }

    private MenuButton fileMenu;
    private PopupMenu newModMenu;

	public Texture EditIcon { get; set; }
	public Texture AddIcon { get; set; }
	public Texture RemoveIcon { get; set; }

    public const int EDIT_BUTTON_INDEX = 0;
    public const int ADD_BUTTON_INDEX = 1;
    public const int REMOVE_BUTTON_INDEX = 2;

	public override void _Ready()
	{
		EditIcon = GD.Load<Texture>("res://res/icons/edit.png");
		AddIcon = GD.Load<Texture>("res://res/icons/add.png");
		RemoveIcon = GD.Load<Texture>("res://res/icons/remove.png");

		fileMenu = GetNode<MenuButton>("MenuSeparator/MenuPanel/Menu/File");
		var filePopup = fileMenu.GetPopup();
        filePopup.Connect("index_pressed", this, nameof(Signal_FileMenuActivated));
        filePopup.AddItem("New project...");
		newModMenu = new PopupMenu() { Name = "NewMod" };
        newModMenu.Connect("index_pressed", this, nameof(Signal_NewModMenuActivated));
		foreach ( var controllerId in ContentPackController.GetRegisteredControllerTypes() )
		{
			var controller = ContentPackController.GetControllerForMod(controllerId);
            newModMenu.AddItem(controller.ModName);
		}
		filePopup.AddChild(newModMenu);
		filePopup.AddSubmenuItem("New content pack...", "NewMod");

		ProjectTree = GetNode<Tree>("MenuSeparator/Splitter/Left/ProjectTree");
		ProjectTree.Connect("button_pressed", this, nameof(Signal_ButtonPressed));
		ProjectTree.Connect("item_activated", this, nameof(Signal_CellActivated));
	}

    private void CreateNewProject()
    {
        ProjectRoot = ProjectTree.CreateItem();
        ProjectRoot.SetText(0, "My Project");
        ProjectRoot.DisableFolding = true;
    }

    private void Signal_FileMenuActivated(int index)
    {
        if (index == 0)
            CreateNewProject();
    }

    private void Signal_NewModMenuActivated(int index)
    {
        var controller = ContentPackController.GetControllerForMod(ContentPackController.GetRegisteredControllerTypes()[index]);

        var mod = ProjectTree.CreateItem(ProjectRoot);
        mod.SetText(0, $"[{controller.ModAbbreviation}] {ProjectRoot.GetText(0)}");
        mod.AddButton(0, RemoveIcon, REMOVE_BUTTON_INDEX, tooltip: "Remove this mod");
        controller.OnModCreated(this, mod);
    }

    private void Signal_ButtonPressed(TreeItem item, int column, int id)
    {
        if (id == REMOVE_BUTTON_INDEX)
            item.GetParent().RemoveChild(item);
    }
    
	private void Signal_CellActivated()
	{
	}
}
