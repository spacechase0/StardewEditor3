using Godot;
using StardewEditor3;
using System;
using System.Collections.Generic;

public class UI : MarginContainer
{
	public Project ModProject { get; set; }

	public Tree ProjectTree { get; set; }
	public TreeItem ProjectRoot { get; set; }
	public PanelContainer MainEditingArea { get; set; }

	private TreeItem depsRoot;
	private Dictionary<TreeItem, Dependency> deps = new Dictionary<TreeItem, Dependency>();

	private TreeItem updateKeysRoot;
	private Dictionary<TreeItem, UpdateKey> updateKeys = new Dictionary<TreeItem, UpdateKey>();

	private MenuButton fileMenu;
	private PopupMenu newModMenu;

	private PackedScene ProjectEditor;
	private PackedScene DependencyEditor;
	private PackedScene UpdateKeyEditor;

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

		ProjectEditor = GD.Load<PackedScene>("res://ProjectEditor.tscn");
		DependencyEditor = GD.Load<PackedScene>("res://DependencyEditor.tscn");
		UpdateKeyEditor = GD.Load<PackedScene>("res://UpdateKeyEditor.tscn");

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
		filePopup.SetItemDisabled(1, true);

		ProjectTree = GetNode<Tree>("MenuSeparator/Splitter/Left/ProjectTree");
		ProjectTree.Connect("button_pressed", this, nameof(Signal_TreeButtonPressed));
		ProjectTree.Connect("item_activated", this, nameof(Signal_TreeCellActivated));

		MainEditingArea = GetNode<PanelContainer>("MenuSeparator/Splitter/Main");

		var confirm = GetNode<ConfirmationDialog>("ConfirmNewProjectDialog");
		confirm.Connect("confirmed", this, nameof(CreateNewProject));

		var removal = GetNode<ConfirmationDialog>("PendingRemovalDialog");
		removal.Connect("confirmed", this, nameof(Signal_PendingRemovalConfirm));
	}

	private void CreateNewProject()
	{
		if ( ProjectRoot != null )
		{
			ProjectRoot.GetParent().RemoveChild(ProjectRoot);
		}

		ModProject = new Project();

		ProjectRoot = ProjectTree.CreateItem();
		ProjectRoot.SetText(0, "My Project");
		ProjectRoot.DisableFolding = true;

		depsRoot = ProjectTree.CreateItem(ProjectRoot);
		depsRoot.SetText(0, "Dependencies");
		depsRoot.AddButton(0, AddIcon, ADD_BUTTON_INDEX, tooltip: "Add a dependency");

		updateKeysRoot = ProjectTree.CreateItem(ProjectRoot);
		updateKeysRoot.SetText(0, "Update Keys");
		updateKeysRoot.AddButton(0, AddIcon, ADD_BUTTON_INDEX, tooltip: "Add an update key");

		fileMenu.GetPopup().SetItemDisabled(1, false);
	}

	private void Signal_FileMenuActivated(int index)
	{
		if (index == 0)
		{
			if (ModProject != null)
			{
				var confirm = GetNode<ConfirmationDialog>("ConfirmNewProjectDialog");
				confirm.PopupCentered();
			}
			else
				CreateNewProject();
		}
	}

	private void Signal_NewModMenuActivated(int index)
	{
		var controller = ContentPackController.GetControllerForMod(ContentPackController.GetRegisteredControllerTypes()[index]);

		var mod = ProjectTree.CreateItem(ProjectRoot);
		mod.SetText(0, $"[{controller.ModAbbreviation}] {ProjectRoot.GetText(0)}");
		mod.AddButton(0, RemoveIcon, REMOVE_BUTTON_INDEX, tooltip: "Remove this mod");
		mod.SetMeta(Meta.CorrespondingController, controller.ModUniqueId);
		controller.OnModCreated(this, mod);
	}

	private void Signal_TreeButtonPressed(TreeItem item, int column, int id)
	{
		if (id == REMOVE_BUTTON_INDEX)
		{
			pendingRemoval = item;
			var confirm = GetNode<ConfirmationDialog>("PendingRemovalDialog");
			confirm.PopupCentered();
		}
		else if ( id == ADD_BUTTON_INDEX )
		{
			if ( item == depsRoot )
			{
				Dependency dep;
				ModProject.Dependencies.Add(dep = new Dependency() { UniqueID = "mod.id" });

				var depItem = ProjectTree.CreateItem(depsRoot);
				depItem.SetText(0, dep.UniqueID);
				depItem.AddButton(0, RemoveIcon, REMOVE_BUTTON_INDEX, tooltip: "Remove this dependency");
				deps.Add(depItem, dep);
			}
			else if ( item == updateKeysRoot )
			{
				UpdateKey updateKey;
				ModProject.UpdateKeys.Add(updateKey = new UpdateKey() { Platform = "Nexus" } );

				var updateKeyItem = ProjectTree.CreateItem(updateKeysRoot);
				updateKeyItem.SetText(0, "Nexus:");
				updateKeyItem.AddButton(0, RemoveIcon, REMOVE_BUTTON_INDEX, tooltip: "Remove this update key");
				updateKeys.Add(updateKeyItem, updateKey);
			}
		}
	}

	private TreeItem pendingRemoval;
	private void Signal_PendingRemovalConfirm()
	{
		if ( pendingRemoval.GetParent() == depsRoot )
		{
			ModProject.Dependencies.Remove(deps[pendingRemoval]);
			deps.Remove(pendingRemoval);
		}
		else if ( pendingRemoval.GetParent() == updateKeysRoot )
		{
			ModProject.UpdateKeys.Remove(updateKeys[pendingRemoval]);
			updateKeys.Remove(pendingRemoval);
		}
		else if (pendingRemoval.GetMeta(Meta.CorrespondingController) != null)
		{
			var controller = ContentPackController.GetControllerForMod((string) pendingRemoval.GetMeta(Meta.CorrespondingController));
			var data = ModProject.Mods.Find(md => md.ContentPackFor == controller.ModUniqueId);
			if (pendingRemoval.GetParent() == ProjectRoot)
				ModProject.Mods.Remove(data);
			else
				controller.OnRemoved(this, data, pendingRemoval);
		}
		pendingRemoval.GetParent().RemoveChild(pendingRemoval);
	}
	
	private void Signal_TreeCellActivated()
	{
		var sel = ProjectTree.GetSelected();
		Node newArea = null;

		if ( sel == ProjectRoot )
		{
			var editor = ProjectEditor.Instance();
			var lineEdit = editor.GetNode<LineEdit>("Name/LineEdit");
			lineEdit.Text = ModProject.Name;
			lineEdit.Connect("text_changed", this, nameof(Signal_ProjectNameEdited));
			lineEdit = editor.GetNode<LineEdit>("Description/LineEdit");
			lineEdit.Text = ModProject.Description;
			lineEdit.Connect("text_changed", this, nameof(Signal_ProjectDescriptionEdited));
			lineEdit = editor.GetNode<LineEdit>("UniqueId/LineEdit");
			lineEdit.Text = ModProject.UniqueId;
			lineEdit.Connect("text_changed", this, nameof(Signal_ProjectUniqueIdEdited));
			lineEdit = editor.GetNode<LineEdit>("Version/LineEdit");
			lineEdit.Text = ModProject.Version;
			lineEdit.Connect("text_changed", this, nameof(Signal_ProjectVersionEdited));
			lineEdit = editor.GetNode<LineEdit>("Author/LineEdit");
			lineEdit.Text = ModProject.Author;
			lineEdit.Connect("text_changed", this, nameof(Signal_ProjectAuthorEdited));
			newArea = editor;
		}
		else if ( sel.GetParent() == depsRoot )
		{
			activeDep = sel;
			var dep = deps[sel];

			var editor = DependencyEditor.Instance();
			var lineEdit = editor.GetNode<LineEdit>("Id/LineEdit");
			lineEdit.Text = dep.UniqueID;
			lineEdit.Connect("text_changed", this, nameof(Signal_DependencyIdEdited));
			var checkBox = editor.GetNode<CheckBox>("Required/CheckBox");
			checkBox.Pressed = dep.IsRequired;
			checkBox.Connect("toggled", this, nameof(Signal_DependencyRequiredToggled));
			lineEdit = editor.GetNode<LineEdit>("MinimumVersion/LineEdit");
			lineEdit.Text = dep.MinimumVersion;
			lineEdit.Connect("text_changed", this, nameof(Signal_DependencyMinimumVersionEdited));
			newArea = editor;
		}
		else if ( sel.GetParent() == updateKeysRoot )
		{
			activeUpdateKey = sel;
			var updateKey = updateKeys[sel];

			var editor = UpdateKeyEditor.Instance();
			var optionButton = editor.GetNode<OptionButton>("Platform/OptionButton");
			optionButton.Selected = optionButton.Items.IndexOf(updateKey.Platform);
			optionButton.Connect("item_selected", this, nameof(Signal_UpdateKeyPlatformEdited));
			var lineEdit = editor.GetNode<LineEdit>("Id/LineEdit");
			lineEdit.Text = updateKey.Id;
			lineEdit.Connect("text_changed", this, nameof(Signal_UpdateKeyIdEdited));
			newArea = editor;
		}

		if ( newArea != null )
		{
			ClearMainEditingArea();
			MainEditingArea.AddChild(newArea);
		}
	}

	private void Signal_ProjectNameEdited(string str)
	{
		ModProject.Name = str;
		ProjectRoot.SetText(0, str);
		for ( var child = ProjectRoot.GetChildren(); child != null; child = child.GetNext() )
		{
			if ( child.GetText(0).BeginsWith("[") )
			{
				var controller = ContentPackController.GetControllerForMod((string)child.GetMeta(Meta.CorrespondingController));
				child.SetText(0, $"[{controller.ModAbbreviation}] {str}");
			}
		}
	}
	private void Signal_ProjectDescriptionEdited(string str)
	{
		ModProject.Description = str;
	}
	private void Signal_ProjectUniqueIdEdited(string str)
	{
		ModProject.UniqueId = str;
	}
	private void Signal_ProjectVersionEdited(string str)
	{
		ModProject.Version = str;
	}
	private void Signal_ProjectAuthorEdited(string str)
	{
		ModProject.Author = str;
	}

	private TreeItem activeDep = null;
	private void Signal_DependencyIdEdited(string str)
	{
		var dep = deps[activeDep];
		activeDep.SetText(0, $"{str} {dep.MinimumVersion}");
		dep.UniqueID = str;
	}
	private void Signal_DependencyRequiredToggled(bool state)
	{
		var dep = deps[activeDep];
		dep.IsRequired = state;
	}
	private void Signal_DependencyMinimumVersionEdited(string str)
	{
		var dep = deps[activeDep];
        activeDep.SetText(0, $"{dep.UniqueID} {str}");
        dep.MinimumVersion = str;
	}

	private TreeItem activeUpdateKey = null;
	private void Signal_UpdateKeyPlatformEdited(int idx)
	{
		var optionButton = MainEditingArea.GetChild(0).GetNode<OptionButton>("Platform/OptionButton");
        var str = optionButton.GetItemText(idx);

		var updateKey = updateKeys[activeUpdateKey];
		activeUpdateKey.SetText(0, $"{str}:{updateKey.Id}");
		updateKey.Platform = str;
	}
	private void Signal_UpdateKeyIdEdited(string str)
	{
		var updateKey = updateKeys[activeUpdateKey];
		activeUpdateKey.SetText(0, $"{updateKey.Platform}:{str}");
		updateKey.Id = str;
	}

	private void ClearMainEditingArea()
	{
		if (MainEditingArea.GetChildCount() <= 0)
			return;

		var child = MainEditingArea.GetChild(0);
		if (child.GetMeta(Meta.CorrespondingController) != null)
		{
			var controller = ContentPackController.GetControllerForMod((string)pendingRemoval.GetMeta(Meta.CorrespondingController));
			var data = ModProject.Mods.Find(md => md.ContentPackFor == controller.ModUniqueId);
			controller.OnEditingAreaChanged(this, data, child);
		}
		child.QueueFree();
	}
}
