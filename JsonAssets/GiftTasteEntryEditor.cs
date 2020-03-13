using Godot;
using System;

public class GiftTasteEntryEditor : HBoxContainer
{
    [Signal]
    public delegate void deleted(IngredientEntryEditor node);
    [Signal]
    public delegate void changed(IngredientEntryEditor node, string person, string likedness);

    public override void _Ready()
    {
        GetNode<LineEdit>("LineEdit").Connect("text_changed", this, nameof(Signal_OnNpcChanged));
        GetNode<OptionButton>("OptionButton").Connect("item_selected", this, nameof(Signal_OnLikednessChanged));
        GetNode<Button>("DeleteButton").Connect("pressed", this, nameof(Signal_OnDelete));
	}

	private void Signal_OnDelete()
	{
        EmitSignal(nameof(deleted), this);
		QueueFree();
    }

    private void Signal_OnNpcChanged(string value)
    {
        var other = GetNode<OptionButton>("OptionButton");
        EmitSignal(nameof(changed), this, value, other.GetItemText(other.Selected));
    }

    private void Signal_OnLikednessChanged(int idx)
    {
        string likedness = GetNode<OptionButton>("OptionButton").GetItemText(idx);
        var other = GetNode<LineEdit>("LineEdit");
        EmitSignal(nameof(changed), this, other.Text, likedness);
    }
}
