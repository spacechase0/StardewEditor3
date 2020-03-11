using Godot;
using System;

public class GiftTasteEditor : VBoxContainer
{
	private readonly PackedScene GiftTasteEntryEditor = GD.Load<PackedScene>("res://JsonAssets/GiftTasteEntryEditor.tscn");

	public override void _Ready()
	{
		GetNode<Button>("AddButton").Connect("pressed", this, nameof(Signal_AddEntry));
	}

	private void Signal_AddEntry()
	{
		var editor = GiftTasteEntryEditor.Instance();
		AddChild(editor);
	}
}
