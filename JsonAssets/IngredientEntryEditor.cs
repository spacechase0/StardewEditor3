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
		GetNode<SpinBox>("SpinBox").Connect("value_changed", this, nameof(Signal_OnQuantityChanged));
		GetNode<Button>("DeleteButton").Connect("pressed", this, nameof(Signal_OnDelete));
	}

	private void Signal_OnDelete()
	{
		EmitSignal(nameof(deleted), this);
		QueueFree();
	}

	private void Signal_OnIngredientChanged(string str)
	{
		var other = GetNode<SpinBox>("SpinBox");
		EmitSignal(nameof(changed), this, str, (int)other.Value );
	}
	private void Signal_OnQuantityChanged(float value)
	{
		var other = GetNode<LineEdit>("LineEdit");
		EmitSignal(nameof(changed), this, other.Text, (int)value);
	}
}
