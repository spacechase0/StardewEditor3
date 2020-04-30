using Godot;
using System;

public class ColorListEntryEditor : HBoxContainer
{
    [Signal]
    public delegate void deleted(ColorListEntryEditor node);
    [Signal]
    public delegate void changed(ColorListEntryEditor node, Color color);

    public override void _Ready()
    {
        GetNode<ColorPickerButton>("ColorPickerButton").Connect("color_changed", this, nameof(Signal_OnChanged));
        GetNode<Button>("DeleteButton").Connect("pressed", this, nameof(Signal_OnDelete));
    }

    private void Signal_OnDelete()
    {
        EmitSignal(nameof(deleted), this);
        QueueFree();
    }

    private void Signal_OnChanged(Color color)
    {
        EmitSignal(nameof(changed), this, color);
    }
}
