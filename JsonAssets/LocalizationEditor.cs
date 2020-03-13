using Godot;
using System;

public class LocalizationEditor : VBoxContainer
{
	[Signal]
	public delegate void entry_added();
	[Signal]
	public delegate void entry_deleted(int entry);
	[Signal]
	public delegate void entry_changed(int entry, string lang, string str);

	private readonly PackedScene LocalizationEntryEditorScene = GD.Load<PackedScene>("res://JsonAssets/LocalizationEntryEditor.tscn");

	public override void _Ready()
	{
		GetNode<Button>("AddButton").Connect("pressed", this, nameof(Signal_AddEntry));
	}

	public void AddEntry(string lang, string str)
	{
		var entry = (LocalizationEntryEditor)LocalizationEntryEditorScene.Instance();
		var optionButton = entry.GetNode<OptionButton>("OptionButton");
		int selInd = 0;
		for (int i = 0; i < optionButton.GetItemCount(); ++i)
		{
			if (optionButton.GetItemText(i) == lang)
			{
				selInd = i;
				break;
			}
		}
		optionButton.Selected = selInd;
		entry.GetNode<LineEdit>("LineEdit").Text = str;
		entry.Connect(nameof(LocalizationEntryEditor.deleted), this, nameof(Signal_DeleteEntry));
		entry.Connect(nameof(LocalizationEntryEditor.changed), this, nameof(Signal_ChangedEntry));
		AddChild(entry);
	}

	private void Signal_AddEntry()
	{
		AddEntry("", "");
		EmitSignal(nameof(entry_added));
	}

	private void Signal_DeleteEntry(LocalizationEntryEditor node)
	{
		var ind = GetChildren().IndexOf(node);
		EmitSignal(nameof(entry_deleted), ind - 1);
	}

	private void Signal_ChangedEntry(LocalizationEntryEditor node, string lang, string str)
	{
		var ind = GetChildren().IndexOf(node);
		EmitSignal(nameof(entry_changed), ind - 1, lang, str);
	}
}
