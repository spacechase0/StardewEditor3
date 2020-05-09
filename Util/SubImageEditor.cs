using Godot;
using StardewEditor3.Util;
using System;
using System.Collections.Generic;

public class SubImageEditor : HBoxContainer
{
	[Signal]
	public delegate void image_changed(SubImageEditor editor);

	private Func<string, Texture> getTexture;

	private ImageResourceReference pendingStartValue;

	private OptionButton resourceSelect;
	private SpinBox subrectX;
	private SpinBox subrectY;
	private SpinBox subrectW;
	private SpinBox subrectH;
	private TextureRect texturePreview;

	public override void _Ready()
	{
		resourceSelect = GetNode<OptionButton>("Values/Resource/OptionButton");
		resourceSelect.Connect("item_selected", this, nameof(Signal_ResourceSelected));

		subrectX = GetNode<SpinBox>("Values/SubRectX/SpinBox");
		subrectX.Connect("value_changed", this, nameof(Signal_SubRectUpdated));
		subrectY = GetNode<SpinBox>("Values/SubRectY/SpinBox");
		subrectY.Connect("value_changed", this, nameof(Signal_SubRectUpdated));
		subrectW = GetNode<SpinBox>("Values/SubRectWidth/SpinBox");
		subrectW.Connect("value_changed", this, nameof(Signal_SubRectUpdated));
		subrectH = GetNode<SpinBox>("Values/SubRectHeight/SpinBox");
		subrectH.Connect("value_changed", this, nameof(Signal_SubRectUpdated));

		texturePreview = GetNode<TextureRect>("ImagePreview");

		var button = GetNode<Button>("Values/ResetSubRectButton");
		button.Connect("pressed", this, nameof(Signal_ResetSubRect));

		var ui = GetTree().Root.GetNode<UI>("UI");
		if ( ui != null )
		{
			InitResources(ui.GetImageList(), ui.GetTexture);
		}

		if (pendingStartValue != null)
			SetValues(pendingStartValue);
	}

	public void InitResources(List<string> resourceList, Func<string, Texture> getTextureFunc)
	{
		resourceSelect.Clear();
		resourceSelect.AddItem("");
		foreach ( var res in resourceList )
		{
			resourceSelect.AddItem(res);
		}

		getTexture = getTextureFunc;
	}

	public void SetValues(ImageResourceReference imageRef)
	{
		pendingStartValue = imageRef;

		if ( resourceSelect != null )
		{
			if (imageRef == null)
			{
				resourceSelect.Selected = -1;
				subrectX.Value = 0;
				subrectY.Value = 0;
				subrectW.Value = 0;
				subrectH.Value = 0;
				return;
			}

			for (int i = 0; i < resourceSelect.GetItemCount(); ++i)
			{
				if (resourceSelect.GetItemText(i) == imageRef.Resource)
				{
					resourceSelect.Selected = i;
					if (imageRef.SubRect.HasValue)
					{
						subrectX.Value = (int)imageRef.SubRect.Value.Position.x;
						subrectY.Value = (int)imageRef.SubRect.Value.Position.y;
						subrectW.Value = (int)imageRef.SubRect.Value.Size.x;
						subrectH.Value = (int)imageRef.SubRect.Value.Size.y;
					}
					ResetTexturePreview();
					return;
				}
			}
		}
	}

	public ImageResourceReference GetImageRef()
	{
		var imageRef = new ImageResourceReference()
		{
			Resource = resourceSelect.Selected == -1 ? null : resourceSelect.GetItemText(resourceSelect.Selected),
		};
		if (imageRef.Resource == "")
			imageRef.Resource = null;

		if (subrectW.Value >= 1 && subrectH.Value >= 1)
		{
			imageRef.SubRect = new Rect2((int)subrectX.Value, (int)subrectY.Value, (int)subrectW.Value, (int)subrectH.Value);
		}
		return imageRef;
	}

	private void Signal_SubRectUpdated(float _value)
	{
		EmitSignal(nameof(image_changed), this);
		ResetTexturePreview();
	}

	private void Signal_ResourceSelected(int _id)
	{
		EmitSignal(nameof(image_changed), this);
		ResetTexturePreview();
	}

	private void Signal_ResetSubRect()
	{
        if (subrectX.Editable)
    		subrectX.Value = 0;
        if (subrectY.Editable)
            subrectY.Value = 0;
        if (subrectW.Editable)
            subrectW.Value = 0;
        if (subrectH.Editable)
            subrectH.Value = 0;
		EmitSignal(nameof(image_changed), this);
		ResetTexturePreview();
	}

	private void ResetTexturePreview()
	{
		if ( resourceSelect.Selected == -1 )
		{
			texturePreview.Texture = null;
			return;
		}
		var res = resourceSelect.GetItemText(resourceSelect.Selected);
		if ( res == "" )
		{
			texturePreview.Texture = null;
			return;
		}
		var baseTex = getTexture(res);

		if (subrectW.Value < 1 || subrectH.Value < 1)
		{
			texturePreview.Texture = baseTex;
		}
		else
		{
			var tex = new AtlasTexture()
			{
				Atlas = baseTex,
				Region = new Rect2((int)subrectX.Value, (int)subrectY.Value, (int)subrectW.Value, (int)subrectH.Value),
			};
			texturePreview.Texture = tex;
		}
	}
}
