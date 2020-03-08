using Godot;
using System;

public class LocalizationEditor : VBoxContainer
{
	private PackedScene LocalizationEntryEditor;

	public override void _Ready()
	{
		LocalizationEntryEditor = GD.Load<PackedScene>("res://JsonAssets/LocalizationEntryEditor.tscn");

		GetNode<Button>("AddButton").Connect("pressed", this, nameof(Signal_AddEntry));
	}

	private void Signal_AddEntry()
	{
		var editor = LocalizationEntryEditor.Instance();
		AddChild(editor);
	}
}
