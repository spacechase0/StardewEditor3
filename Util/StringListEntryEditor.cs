using Godot;
using System;

public class StringListEntryEditor : HBoxContainer
{
	[Signal]
	public delegate void deleted(StringListEntryEditor node);
	[Signal]
	public delegate void changed(StringListEntryEditor node, string str);

	public override void _Ready()
	{
		GetNode<LineEdit>("LineEdit").Connect("text_changed", this, nameof(Signal_OnChanged));
		GetNode<Button>("DeleteButton").Connect("pressed", this, nameof(Signal_OnDelete));
	}

	private void Signal_OnDelete()
	{
		EmitSignal(nameof(deleted), this);
		QueueFree();
	}

	private void Signal_OnChanged(string str)
	{
		EmitSignal(nameof(changed), this, str);
	}
}
