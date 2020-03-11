using Godot;
using System;

public class StringListEditor : VBoxContainer
{
    [Signal]
    public delegate void entry_added();
    [Signal]
    public delegate void entry_deleted(int entry);
    [Signal]
    public delegate void entry_changed(int entry, string newVal);

	private readonly PackedScene StringListEntryEditorScene = GD.Load<PackedScene>("res://Util/StringListEntryEditor.tscn");

	public override void _Ready()
	{
		GetNode<Button>("AddButton").Connect("pressed", this, nameof(Signal_AddString));
	}

    public void AddString(string str)
    {
        var strEditor = StringListEntryEditorScene.Instance();
        strEditor.GetNode<LineEdit>("LineEdit").Text = str;
        strEditor.Connect(nameof(StringListEntryEditor.changed), this, nameof(Signal_ChangedString));
        strEditor.Connect(nameof(StringListEntryEditor.deleted), this, nameof(Signal_DeleteString));
        AddChild(strEditor);
    }

	private void Signal_AddString()
	{
        AddString("");
        EmitSignal(nameof(entry_added));
	}

    private void Signal_DeleteString(StringListEntryEditor node)
    {
        var ind = GetChildren().IndexOf(node);
        EmitSignal(nameof(entry_deleted), ind - 1);
    }

    private void Signal_ChangedString(StringListEntryEditor node, string str)
    {
        var ind = GetChildren().IndexOf(node);
        EmitSignal(nameof(entry_changed), ind - 1, str);
    }
}
