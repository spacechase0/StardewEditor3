using Godot;
using System;

public class IngredientListEditor : VBoxContainer
{
	[Signal]
	public delegate void entry_added();
	[Signal]
	public delegate void entry_deleted(int entry);
	[Signal]
	public delegate void entry_changed(int entry, string newIngred, int newAmt);

	private readonly PackedScene IngredientEntryEditorScene = GD.Load<PackedScene>("res://JsonAssets/IngredientEntryEditor.tscn");


	public override void _Ready()
	{
		GetNode<Button>("AddButton").Connect("pressed", this, nameof(Signal_AddEntry));
	}

	public void AddEntry(string ingred, int amt)
	{
		var ingredEditor = IngredientEntryEditorScene.Instance();
		ingredEditor.GetNode<LineEdit>("LineEdit").Text = ingred;
		ingredEditor.GetNode<SpinBox>("SpinBox").Value = amt;
		ingredEditor.Connect(nameof(IngredientEntryEditor.changed), this, nameof(Signal_ChangedEntry));
		ingredEditor.Connect(nameof(IngredientEntryEditor.deleted), this, nameof(Signal_DeleteEntry));
		AddChild(ingredEditor);
	}

	private void Signal_AddEntry()
	{
		AddEntry("", 1);
		EmitSignal(nameof(entry_added));
	}

	private void Signal_DeleteEntry(StringListEntryEditor node)
	{
		var ind = GetChildren().IndexOf(node);
		EmitSignal(nameof(entry_deleted), ind - 1);
	}

	private void Signal_ChangedEntry(StringListEntryEditor node, string ingred, int amt)
	{
		var ind = GetChildren().IndexOf(node);
		EmitSignal(nameof(entry_changed), ind - 1, ingred, amt);
	}
}
