using Godot;
using Newtonsoft.Json;
using StardewEditor3;
using StardewEditor3.Util;
using System;
using System.Collections.Generic;
using System.IO;

public class UI : MarginContainer
{
	public string ModProjectDir { get; set; }
	public Project ModProject { get; set; }

	private FileSystemWatcher projectDirWatcher;

	public Tree ProjectTree { get; set; }
	public TreeItem ProjectRoot { get; set; }
	public PanelContainer MainEditingArea { get; set; }

	private TreeItem depsRoot;
	private readonly Dictionary<TreeItem, Dependency> deps = new Dictionary<TreeItem, Dependency>();

	private TreeItem updateKeysRoot;
	private readonly Dictionary<TreeItem, UpdateKey> updateKeys = new Dictionary<TreeItem, UpdateKey>();

	private TreeItem resourcesRoot;
	private readonly Dictionary<TreeItem, string> resourcesNames = new Dictionary<TreeItem, string>();
    private readonly Dictionary<string, ImageTexture> textureResources = new Dictionary<string, ImageTexture>();

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
		filePopup.AddSeparator();
		filePopup.AddItem("Save");
		filePopup.AddItem("Open");
		filePopup.AddSeparator();
		filePopup.AddItem("Export");

		ProjectTree = GetNode<Tree>("MenuSeparator/Splitter/Left/ProjectTree");
		ProjectTree.Connect("button_pressed", this, nameof(Signal_TreeButtonPressed));
		ProjectTree.Connect("item_activated", this, nameof(Signal_TreeCellActivated));
		ProjectTree.Connect("item_edited", this, nameof(Signal_TreeItemEdited));

		MainEditingArea = GetNode<PanelContainer>("MenuSeparator/Splitter/Main");

		var confirm = GetNode<ConfirmationDialog>("ConfirmNewProjectDialog");
		confirm.Connect("confirmed", this, nameof(Signal_CreateNewProject_Pre));

		var confirm2 = GetNode<ConfirmationDialog>("ConfirmOpenProjectDialog");
		confirm2.Connect("confirmed", this, nameof(Signal_OpenProject_Pre));

		var removal = GetNode<ConfirmationDialog>("PendingRemovalDialog");
		removal.Connect("confirmed", this, nameof(Signal_PendingRemovalConfirm));

		var createDir = GetNode<FileDialog>("NewProjectDirectoryDialog");
		createDir.Connect("dir_selected", this, nameof(Signal_CreateNewProject_SelectedDirectory));

		var open = GetNode<FileDialog>("OpenProjectDialog");
		open.Connect("file_selected", this, nameof(Signal_OpenProject));

		var import = GetNode<FileDialog>("ResourceImportDialog");
		import.Connect("files_selected", this, nameof(Signal_ResourceImport_Multiple));

		var export = GetNode<FileDialog>("ExportProjectDialog");
		export.Connect("dir_selected", this, nameof(Signal_ExportProject));
	}

    public List<string> GetImageList()
    {
        var ret = new List<string>();
        foreach ( var item in resourcesNames )
        {
            ret.Add(item.Value);
        }
        return ret;
    }

    public Texture GetTexture(string res)
    {
        if ( !textureResources.ContainsKey(res) )
        {
            var image = new Image();
            image.Load(System.IO.Path.Combine(ModProjectDir, res));
            var tex = new ImageTexture();
            tex.CreateFromImage(image);
            textureResources.Add(res, tex);
        }

        return textureResources[res];
    }

	private void Signal_CreateNewProject_Pre()
	{
		var createDir = GetNode<FileDialog>("NewProjectDirectoryDialog");
		createDir.PopupCenteredClamped();
	}

	private void Signal_CreateNewProject_SelectedDirectory(string dir)
	{
		if ( System.IO.Directory.GetFiles(dir).Length != 0 || System.IO.Directory.GetDirectories(dir).Length != 0 )
		{
			GetNode<AcceptDialog>("BadProjectDirectoryDialog").PopupCenteredClamped();
			return;
		}

		ModProjectDir = dir;
		CreateNewProject();
	}

	private void CreateNewProject(bool loading = false)
	{
		if ( ProjectRoot != null )
		{
			ProjectRoot.GetParent().RemoveChild(ProjectRoot);
		}

		if (projectDirWatcher != null)
		{
			projectDirWatcher.EnableRaisingEvents = false;
			projectDirWatcher = null; 
		}

		if ( !loading )
			ModProject = new Project();

		ProjectRoot = ProjectTree.CreateItem();
		ProjectRoot.SetText(0, "My Project 1.0.0");
		ProjectRoot.DisableFolding = true;

		depsRoot = ProjectTree.CreateItem(ProjectRoot);
		depsRoot.SetText(0, "Dependencies");
		depsRoot.AddButton(0, AddIcon, ADD_BUTTON_INDEX, tooltip: "Add a dependency");

		updateKeysRoot = ProjectTree.CreateItem(ProjectRoot);
		updateKeysRoot.SetText(0, "Update Keys");
		updateKeysRoot.AddButton(0, AddIcon, ADD_BUTTON_INDEX, tooltip: "Add an update key");

		resourcesRoot = ProjectTree.CreateItem(ProjectRoot);
		resourcesRoot.SetText(0, "Resources");
		resourcesRoot.AddButton(0, AddIcon, ADD_BUTTON_INDEX, tooltip: "Import a resource");

		fileMenu.GetPopup().SetItemDisabled(1, false);

		if (!loading)
		{
			SaveProject();
			InitFileSystemWatcher();
		}
	}

	private void SaveProject()
	{
		string path = System.IO.Path.Combine(ModProjectDir, "project.stardeweditor");
		GD.Print("Saving to " + path);

		JsonSerializerSettings settings = new JsonSerializerSettings()
		{
			Formatting = Formatting.Indented,
			TypeNameHandling = TypeNameHandling.Objects,
		};

		System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(ModProject, settings));
		foreach ( var mod in ModProject.Mods )
		{
			var controller = ContentPackController.GetControllerForMod(mod.ContentPackFor);
			controller.OnSave(this, mod);
		}
	}

	private void Signal_OpenProject_Pre()
	{
		var openDialog = GetNode<FileDialog>("OpenProjectDialog");
		openDialog.PopupCenteredClamped();
	}

	private void Signal_OpenProject(string path)
	{
		string json = System.IO.File.ReadAllText(path);
		var verCheck = JsonConvert.DeserializeObject<VersionCheck>(json);
		if ( verCheck.FormatVersion != Project.LATEST_VERSION )
		{
			GD.Print($"Bad version {verCheck.FormatVersion}, expected {Project.LATEST_VERSION}.");
			return;
		}
		GD.Print("Opening " + path);

		JsonSerializerSettings settings = new JsonSerializerSettings()
		{
			TypeNameHandling = TypeNameHandling.Objects,
		};

		ModProjectDir = System.IO.Path.GetDirectoryName(path);
		ModProject = JsonConvert.DeserializeObject<Project>(json, settings);

		CreateNewProject(loading: true);
		ProjectRoot.SetText(0, $"{ModProject.Name} {ModProject.Version}");

		foreach ( var dep in ModProject.Dependencies )
		{
			GD.Print($"Dependency: {dep.UniqueID}");
			var depItem = ProjectTree.CreateItem(depsRoot);
			depItem.SetText(0, $"{dep.UniqueID} {dep.MinimumVersion}");
			depItem.AddButton(0, RemoveIcon, REMOVE_BUTTON_INDEX, tooltip: "Remove this dependency");
			deps.Add(depItem, dep);
		}

		foreach ( var updateKey in ModProject.UpdateKeys )
		{
			GD.Print($"Update key: {updateKey.Platform}");
			var updateKeyItem = ProjectTree.CreateItem(updateKeysRoot);
			updateKeyItem.SetText(0, $"{updateKey.Platform}:{updateKey.Id}");
			updateKeyItem.AddButton(0, RemoveIcon, REMOVE_BUTTON_INDEX, tooltip: "Remove this update key");
			updateKeys.Add(updateKeyItem, updateKey);
		}

		foreach ( var mod in ModProject.Mods )
		{
			GD.Print($"Mod: {mod.ContentPackFor}");
			var controller = ContentPackController.GetControllerForMod(mod.ContentPackFor);

			var modItem = ProjectTree.CreateItem(ProjectRoot);
			modItem.SetText(0, $"[{controller.ModAbbreviation}] {ModProject.Name}");
			modItem.AddButton(0, RemoveIcon, REMOVE_BUTTON_INDEX, tooltip: "Remove this mod");
			modItem.SetMeta(Meta.CorrespondingController, controller.ModUniqueId);
			controller.OnLoad(this, mod, modItem);
		}

		foreach ( var filename_ in System.IO.Directory.GetFiles(ModProjectDir) )
		{
			var filename = System.IO.Path.GetFileName(filename_);
			var ext = System.IO.Path.GetExtension(filename);
			if ( ext == ".png" || ext == ".tbin" || ext == ".tmx" || ext == ".json" || ext == ".xnb" )
			{
				var resItem = ProjectTree.CreateItem(resourcesRoot);
				resItem.SetText(0, filename);
				resItem.SetEditable(0, true);
				resItem.AddButton(0, RemoveIcon, REMOVE_BUTTON_INDEX, tooltip: "Delete this resource");
				resourcesNames.Add(resItem, filename);
			}
		}

		InitFileSystemWatcher();
	}

	private void Signal_ResourceImport_Multiple(string[] files)
	{
		foreach ( var file in files )
		{
			var origFilename = System.IO.Path.GetFileName(file);
			var filename = origFilename;
			for (int i = 2; System.IO.File.Exists(System.IO.Path.Combine(ModProjectDir, filename)); ++i)
				filename = System.IO.Path.GetFileNameWithoutExtension(origFilename) + i + System.IO.Path.GetExtension(origFilename);

			System.IO.File.Copy(file, System.IO.Path.Combine(ModProjectDir, filename));
			// The file system watcher will handle UI stuff
		}
	}

	private void Signal_ExportProject(string dir)
	{
        GD.Print("Exporting...");
		string path = System.IO.Path.Combine(dir, ModProject.Name);
		if ( System.IO.Directory.Exists( path ) )
			System.IO.Directory.Delete(path, true);
		System.IO.Directory.CreateDirectory(path);

		foreach ( var mod in ModProject.Mods )
		{
            GD.Print("Exporting for " + mod.ContentPackFor);
			var controller = ContentPackController.GetControllerForMod(mod.ContentPackFor);
			controller.OnExport(this, mod, path);
		}
	}

	private void Signal_FileMenuActivated(int index)
	{
		if (index == 0)
		{
			if (ModProject != null)
			{
				var confirm = GetNode<ConfirmationDialog>("ConfirmNewProjectDialog");
				confirm.PopupCenteredClamped();
			}
			else
				Signal_CreateNewProject_Pre();
		}
		else if ( index == 3 )
		{
			SaveProject();
		}
		else if ( index == 4 )
		{
			if (ModProject != null)
			{
				var confirm = GetNode<ConfirmationDialog>("ConfirmOpenProjectDialog");
				confirm.PopupCenteredClamped();
			}
			else
				Signal_OpenProject_Pre();
		}
		else if ( index == 6 )
		{
			var export = GetNode<FileDialog>("ExportProjectDialog");
			export.PopupCenteredClamped();
		}
	}

	private void Signal_NewModMenuActivated(int index)
	{
		var controller = ContentPackController.GetControllerForMod(ContentPackController.GetRegisteredControllerTypes()[index]);

		var mod = ProjectTree.CreateItem(ProjectRoot);
		mod.SetText(0, $"[{controller.ModAbbreviation}] {ModProject.Name}");
		mod.AddButton(0, RemoveIcon, REMOVE_BUTTON_INDEX, tooltip: "Remove this mod");
		mod.SetMeta(Meta.CorrespondingController, controller.ModUniqueId);
		ModProject.Mods.Add(controller.OnModCreated(this, mod));
	}

	private void Signal_TreeButtonPressed(TreeItem item, int column, int id)
	{
		if (id == REMOVE_BUTTON_INDEX)
		{
			pendingRemoval = item;
			var confirm = GetNode<ConfirmationDialog>("PendingRemovalDialog");
			confirm.PopupCenteredClamped();
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
			else if ( item == resourcesRoot )
			{
				var import = GetNode<FileDialog>("ResourceImportDialog");
				import.PopupCenteredClamped();
			}
            else if ( item.GetMeta(Meta.CorrespondingController) != null )
            {
                var controller = ContentPackController.GetControllerForMod((string)item.GetParent().GetMeta(Meta.CorrespondingController));
                var data = ModProject.Mods.Find(md => md.ContentPackFor == controller.ModUniqueId);
                controller.OnAdded(this, data, item);
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
		else if ( pendingRemoval.GetParent() == resourcesRoot )
		{
			string filename = resourcesNames[pendingRemoval];
			System.IO.File.Delete(System.IO.Path.Combine(ModProjectDir, filename));
			foreach (var mod in ModProject.Mods)
			{
				var controller = ContentPackController.GetControllerForMod(mod.ContentPackFor);
				controller.OnResourceDeleted(this, mod, filename);
			}
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
			int selInd = 0;
			for ( int i = 0; i < optionButton.GetItemCount(); ++i )
			{
				if (optionButton.GetItemText(i) == updateKey.Platform)
				{
					selInd = i;
					break;
				}
			}
			optionButton.Selected = selInd;
			optionButton.Connect("item_selected", this, nameof(Signal_UpdateKeyPlatformEdited));
			var lineEdit = editor.GetNode<LineEdit>("Id/LineEdit");
			lineEdit.Text = updateKey.Id;
			lineEdit.Connect("text_changed", this, nameof(Signal_UpdateKeyIdEdited));
			newArea = editor;
		}
        else if ( sel.GetMeta(Meta.CorrespondingController) != null )
        {
            var controller = ContentPackController.GetControllerForMod((string)sel.GetMeta(Meta.CorrespondingController));
            var data = ModProject.Mods.Find(md => md.ContentPackFor == controller.ModUniqueId);
            newArea = controller.CreateMainEditingArea(this, data, sel);
        }

		if ( newArea != null )
		{
			ClearMainEditingArea();
			MainEditingArea.AddChild(newArea);
		}
	}

	private void Signal_TreeItemEdited()
	{
		var edited = ProjectTree.GetSelected();
		if ( edited.GetParent() == resourcesRoot )
		{
			var oldFilename = resourcesNames[edited];
			string newFilename = edited.GetText(0);

			justRenamedInUi = newFilename;
			System.IO.File.Move(System.IO.Path.Combine(ModProjectDir, oldFilename), System.IO.Path.Combine(ModProjectDir, newFilename));
			resourcesNames[edited] = newFilename;
			foreach ( var mod in ModProject.Mods )
			{
				var controller = ContentPackController.GetControllerForMod(mod.ContentPackFor);
				controller.OnResourceRenamed(this, mod, oldFilename, newFilename);
			}
		}
	}

	private void Signal_ProjectNameEdited(string str)
	{
		ModProject.Name = str;
		ProjectRoot.SetText(0, $"{str} {ModProject.Version}");
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
		ProjectRoot.SetText(0, $"{ModProject.Name} {str}");
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

	private void InitFileSystemWatcher()
	{
		projectDirWatcher = new FileSystemWatcher(ModProjectDir)
		{
			NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
		};
		projectDirWatcher.Created += WatchProjectDir_Create;
		projectDirWatcher.Renamed += WatchProjectDir_Rename;
		projectDirWatcher.Deleted += WatchProjectDir_Delete;
        projectDirWatcher.Changed += WatchProjectDir_Change;
        projectDirWatcher.EnableRaisingEvents = true;
	}

    string justRenamedInUi = null;
	private void WatchProjectDir_Create(object sender, FileSystemEventArgs e)
	{
		var filename = System.IO.Path.GetFileName(e.Name);
		if (filename == justRenamedInUi)
		{
			justRenamedInUi = null;
			return;
		}
		var ext = System.IO.Path.GetExtension(filename);
		if (ext == ".png" || ext == ".tbin" || ext == ".tmx" || ext == ".json" || ext == ".xnb")
		{
			var resItem = ProjectTree.CreateItem(resourcesRoot);
			resItem.SetText(0, filename);
			resItem.SetEditable(0, true);
			resItem.AddButton(0, RemoveIcon, REMOVE_BUTTON_INDEX, tooltip: "Delete this resource");
			resourcesNames.Add(resItem, filename);
		}
	}

	private void WatchProjectDir_Rename(object sender, RenamedEventArgs e)
	{
		var oldFilename = System.IO.Path.GetFileName(e.OldName);
		var filename = System.IO.Path.GetFileName(e.Name);

		for (var child = resourcesRoot.GetChildren(); child != null; child = child.GetNext())
		{
			if (child.GetText(0) == oldFilename)
			{
				child.SetText(0, filename);
				resourcesNames[child] = filename;
				return;
			}
		}
	}

	private void WatchProjectDir_Delete(object sender, FileSystemEventArgs e)
	{
		var filename = System.IO.Path.GetFileName(e.Name);
		for ( var child = resourcesRoot.GetChildren(); child != null; child = child.GetNext() )
		{
			if ( child.GetText(0) == filename)
			{
				resourcesRoot.RemoveChild(child);
				resourcesNames.Remove(child);
				return;
			}
		}
    }

    private void WatchProjectDir_Change(object sender, FileSystemEventArgs e)
    {
        var filename = System.IO.Path.GetFileName(e.Name);
        if ( System.IO.Path.GetExtension(filename) == ".png" )
        {
            if ( textureResources.ContainsKey(filename) )
            {
                var image = new Image();
                image.Load(e.Name);

                var tex = textureResources[filename];
                tex.CreateFromImage(image);
            }
        }
    }
}
