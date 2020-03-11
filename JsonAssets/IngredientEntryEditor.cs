using Godot;
using System;

public class IngredientEntryEditor : HBoxContainer
{
	[Signal]
	public delegate void deleted(IngredientEntryEditor node);
	[Signal]
	public delegate void changed(IngredientEntryEditor node, string ingred, int amt);

	public override void _Ready()
	{
		GetNode<LineEdit>("LineEdit").Connect("text_changed", this, nameof(Signal_OnIngredientChanged));
		GetNode<IntegerEdit>("IntegerEdit").Connect("int_edited", this, nameof(Signal_OnQuantityChanged));
		GetNode<Button>("DeleteButton").Connect("pressed", this, nameof(Signal_OnDelete));
	}

	private void Signal_OnDelete()
	{
		EmitSignal(nameof(deleted), this);
		QueueFree();
	}

	private void Signal_OnIngredientChanged(string str)
	{
		var other = GetNode<IntegerEdit>("IntegerEdit");
		EmitSignal(nameof(changed), this, str, other.Value.HasValue ? other.Value : 0 );
	}
	private void Signal_OnQuantityChanged(bool has, long val)
	{
		var other = GetNode<LineEdit>("LineEdit");
		EmitSignal(nameof(changed), this, other.Text, has ? val : 0);
	}
}
