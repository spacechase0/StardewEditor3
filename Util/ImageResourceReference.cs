using Godot;
using StardewEditor3.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewEditor3.Util
{
    public class ImageResourceReference
    {
        public string Resource { get; set; }
        public Rect2? SubRect { get; set; }

        public Image MakeImage(string resourceDir)
        {
            Image image = new Image();
            image.Load(System.IO.Path.Combine(resourceDir, Resource));
            if (!SubRect.HasValue)
                return image;

            GD.Print("meow:" + SubRect);
            var ret = new Image();
            if (SubRect.HasValue)
                ret.Create((int)SubRect.Value.Size.x, (int)SubRect.Value.Size.y, false, Image.Format.Rgba8);
            else
                ret.Create(image.GetWidth(), image.GetHeight(), false, Image.Format.Rgba8);
            ret.BlitRect(image, SubRect.Value, new Vector2(0, 0));
            return ret;
        }
    }
}
