using Godot;
using System;

public class IntListEntryEditor : HBoxContainer
{
	[Signal]
	public delegate void deleted(IntListEntryEditor node);
	[Signal]
	public delegate void changed(IntListEntryEditor node, float value);

	public override void _Ready()
	{
		GetNode<SpinBox>("SpinBox").Connect("value_changed", this, nameof(Signal_OnChanged));
		GetNode<Button>("DeleteButton").Connect("pressed", this, nameof(Signal_OnDelete));
	}

	private void Signal_OnDelete()
	{
		EmitSignal(nameof(deleted), this);
		QueueFree();
	}

	private void Signal_OnChanged(float value)
	{
		EmitSignal(nameof(changed), this, value);
	}
}
