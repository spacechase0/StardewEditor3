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
        public Rect2 SubRect { get; set; }

        public Image MakeImage(string resourceDir)
        {
            Image image = new Image();
            image.Load(System.IO.Path.Combine(resourceDir, Resource));
            if (SubRect == null)
                return image;

            var ret = new Image();
            ret.BlitRect(image, SubRect, new Vector2(0, 0));
            return ret;
        }
    }
}
