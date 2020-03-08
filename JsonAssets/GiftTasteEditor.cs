using Godot;
using System;

public class GiftTasteEditor : VBoxContainer
{
	private PackedScene GiftTasteEntryEditor;

	public override void _Ready()
	{
		GiftTasteEntryEditor = GD.Load<PackedScene>("res://JsonAssets/GiftTasteEntryEditor.tscn");

		GetNode<Button>("AddButton").Connect("pressed", this, nameof(Signal_AddEntry));
	}

	private void Signal_AddEntry()
	{
		var editor = GiftTasteEntryEditor.Instance();
		AddChild(editor);
	}
}
