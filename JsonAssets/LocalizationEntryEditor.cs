using Godot;
using System;

public class LocalizationEntryEditor : HBoxContainer
{
    public override void _Ready()
    {
        GetNode<Button>("DeleteButton").Connect("pressed", this, nameof(Signal_OnDelete));
    }

    private void Signal_OnDelete()
    {
        QueueFree();
    }
}
