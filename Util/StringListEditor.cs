using Godot;
using System;

public class StringListEditor : VBoxContainer
{
	private PackedScene StringListEntryEditor;

	public override void _Ready()
	{
		StringListEntryEditor = GD.Load<PackedScene>("res://Util/StringListEntryEditor.tscn");

		GetNode<Button>("AddButton").Connect("pressed", this, nameof(Signal_AddString));
	}

	private void Signal_AddString()
	{
		var strEditor = StringListEntryEditor.Instance();
		AddChild(strEditor);
	}
}
