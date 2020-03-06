using Godot;
using System;

public class IntegerEdit : LineEdit
{
	[Signal]
	public delegate void int_edited(int value);

	public int? Value
	{
		get
		{
			return string.IsNullOrEmpty(Text) ? null : (int?)int.Parse(Text);
		}
		set
		{
			if (value.HasValue)
				Text = value.Value.ToString();
			else
				Text = "";
		}
	}

	public override void _Ready()
	{
		Connect("text_changed", this, nameof(Signal_TextChanged));
	}

	public void Signal_TextChanged(string text)
	{
		int cursorPos = CaretPosition;

		for ( int i = 0; i < text.Length; ++i )
		{
			if ( !char.IsDigit( text[ i ] ) )
			{
				text = text.Remove(i, 1);
				--i;
			}
		}
		
		Text = text;
		CaretPosition = cursorPos;

		EmitSignal(nameof(int_edited), int.Parse(text));
	}
}
