using Godot;
using System;

public class IntListEditor : VBoxContainer
{
	[Signal]
	public delegate void entry_added();
	[Signal]
	public delegate void entry_deleted(int entry);
	[Signal]
	public delegate void entry_changed(int entry, bool newHas, int newValue);

	private readonly PackedScene IntListEntryEditorScene = GD.Load<PackedScene>("res://Util/IntListEntryEditor.tscn");

	public override void _Ready()
	{
		GetNode<Button>("AddButton").Connect("pressed", this, nameof(Signal_AddValue));
	}

	public void AddInt(int? value)
	{
		var strEditor = IntListEntryEditorScene.Instance();
		strEditor.GetNode<IntegerEdit>("IntegerEdit").Value = value;
		strEditor.Connect(nameof(IntListEntryEditor.changed), this, nameof(Signal_ChangedValue));
		strEditor.Connect(nameof(IntListEntryEditor.deleted), this, nameof(Signal_DeleteValue));
		AddChild(strEditor);
	}

	private void Signal_AddValue()
	{
		AddInt(0);
		EmitSignal(nameof(entry_added));
	}

	private void Signal_DeleteValue(StringListEntryEditor node)
	{
		var ind = GetChildren().IndexOf(node);
		EmitSignal(nameof(entry_deleted), ind - 1);
	}

	private void Signal_ChangedValue(StringListEntryEditor node, bool newHas, bool newValue)
	{
		var ind = GetChildren().IndexOf(node);
		EmitSignal(nameof(entry_changed), ind - 1, newHas, newValue);
	}
}
