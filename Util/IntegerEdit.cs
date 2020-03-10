using Godot;
using System;

public class IntegerEdit : LineEdit
{
	[Signal]
	public delegate void int_edited(bool has, long value);

	public long? Value
	{
		get
		{
			return string.IsNullOrEmpty(Text) ? null : (long?)long.Parse(Text);
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
			if ( !char.IsDigit( text[ i ] ) && !(text[i] == '-' && i == 0) )
			{
				text = text.Remove(i, 1);
				--i;
			}
		}
		
		Text = text;
		CaretPosition = cursorPos;

		EmitSignal(nameof(int_edited), text != "", text == "" ? 0 : long.Parse(text));
	}
}
