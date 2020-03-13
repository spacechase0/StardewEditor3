using Godot;
using System;

public class LocalizationEntryEditor : HBoxContainer
{
	[Signal]
	public delegate void deleted(LocalizationEntryEditor node);
	[Signal]
	public delegate void changed(LocalizationEntryEditor node, string lang, string value);

	public override void _Ready()
	{
		GetNode<OptionButton>("OptionButton").Connect("item_selected", this, nameof(Signal_OnLanguageChanged));
		GetNode<LineEdit>("LineEdit").Connect("text_changed", this, nameof(Signal_OnStringChanged));
		GetNode<Button>("DeleteButton").Connect("pressed", this, nameof(Signal_OnDelete));
	}

	private void Signal_OnDelete()
	{
		EmitSignal(nameof(deleted), this);
		QueueFree();
	}

	private void Signal_OnLanguageChanged(int idx)
	{
		string lang = GetNode<OptionButton>("OptionButton").GetItemText(idx);
		var other = GetNode<LineEdit>("LineEdit");
		EmitSignal(nameof(changed), this, lang, other.Text);
	}

	private void Signal_OnStringChanged(string value)
	{
		var other = GetNode<OptionButton>("OptionButton");
		EmitSignal(nameof(changed), this, other.GetItemText(other.Selected), value);
	}
}
