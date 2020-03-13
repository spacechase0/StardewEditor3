using Godot;
using System;

public class GiftTasteEditor : VBoxContainer
{
    [Signal]
    public delegate void entry_added();
    [Signal]
    public delegate void entry_deleted(int entry);
    [Signal]
    public delegate void entry_changed(int entry, string npc, string likedness);

    private readonly PackedScene GiftTasteEntryEditorScene = GD.Load<PackedScene>("res://JsonAssets/GiftTasteEntryEditor.tscn");

	public override void _Ready()
	{
		GetNode<Button>("AddButton").Connect("pressed", this, nameof(Signal_AddEntry));
    }
    public void AddEntry(string npc, string likedness)
    {
        var entry = (GiftTasteEntryEditor)GiftTasteEntryEditorScene.Instance();
        entry.GetNode<LineEdit>("LineEdit").Text = npc;
        var optionButton = entry.GetNode<OptionButton>("OptionButton");
        int selInd = 0;
        for (int i = 0; i < optionButton.GetItemCount(); ++i)
        {
            if (optionButton.GetItemText(i) == likedness)
            {
                selInd = i;
                break;
            }
        }
        optionButton.Selected = selInd;
        entry.Connect(nameof(GiftTasteEntryEditor.deleted), this, nameof(Signal_DeleteEntry));
        entry.Connect(nameof(GiftTasteEntryEditor.changed), this, nameof(Signal_ChangedEntry));
        AddChild(entry);
    }

    private void Signal_AddEntry()
	{
        AddEntry("", "Neutral");
        EmitSignal(nameof(entry_added));
    }

    private void Signal_DeleteEntry(LocalizationEntryEditor node)
    {
        var ind = GetChildren().IndexOf(node);
        EmitSignal(nameof(entry_deleted), ind - 1);
    }

    private void Signal_ChangedEntry(LocalizationEntryEditor node, string npc, string likedness)
    {
        var ind = GetChildren().IndexOf(node);
        EmitSignal(nameof(entry_changed), ind - 1, npc, likedness);
    }
}
