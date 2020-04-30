using Godot;
using System;

public class ColorListEditor : VBoxContainer
{
    [Signal]
    public delegate void entry_added();
    [Signal]
    public delegate void entry_deleted(int entry);
    [Signal]
    public delegate void entry_changed(int entry, Color newColor);

    private readonly PackedScene ColorListEntryEditorScene = GD.Load<PackedScene>("res://Util/ColorListEntryEditor.tscn");

    public override void _Ready()
    {
        GetNode<Button>("AddButton").Connect("pressed", this, nameof(Signal_AddValue));
    }

    public void AddColor(Color color)
    {
        var colEditor = ColorListEntryEditorScene.Instance();
        colEditor.GetNode<ColorPickerButton>("ColorPickerButton").Color = color;
        colEditor.Connect(nameof(ColorListEntryEditor.changed), this, nameof(Signal_ChangedValue));
        colEditor.Connect(nameof(ColorListEntryEditor.deleted), this, nameof(Signal_DeleteValue));
        AddChild(colEditor);
    }

    private void Signal_AddValue()
    {
        AddColor(Color.Color8(0, 0, 0));
        EmitSignal(nameof(entry_added));
    }

    private void Signal_DeleteValue(StringListEntryEditor node)
    {
        var ind = GetChildren().IndexOf(node);
        EmitSignal(nameof(entry_deleted), ind - 1);
    }

    private void Signal_ChangedValue(StringListEntryEditor node, Color newColor)
    {
        var ind = GetChildren().IndexOf(node);
        EmitSignal(nameof(entry_changed), ind - 1, newColor);
    }
}
