using Godot;
using System;

public class LocalizationEditor : VBoxContainer
{
	private readonly PackedScene LocalizationEntryEditor = GD.Load<PackedScene>("res://JsonAssets/LocalizationEntryEditor.tscn");

	public override void _Ready()
	{
		GetNode<Button>("AddButton").Connect("pressed", this, nameof(Signal_AddEntry));
	}

	private void Signal_AddEntry()
	{
		var editor = LocalizationEntryEditor.Instance();
		AddChild(editor);
	}
}
