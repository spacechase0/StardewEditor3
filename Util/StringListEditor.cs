using Godot;
using System;

public class StringListEditor : VBoxContainer
{
	private PackedScene StringEditor;

	public override void _Ready()
	{
		StringEditor = GD.Load<PackedScene>("res://Util/StringListEntryEditor.tscn");

		GetNode<Button>("AddButton").Connect("pressed", this, nameof(Signal_AddString));
	}

	private void Signal_AddString()
	{
		var strEditor = StringEditor.Instance();
		AddChild(strEditor);
	}
}
