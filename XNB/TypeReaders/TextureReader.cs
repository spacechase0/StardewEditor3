using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tbin;

namespace XNB.TypeReaders
{
    public class TextureReader : TypeReader
    {
        public object Read(BinaryReader reader, string fullDecl)
        {
            int format = reader.ReadInt32();
            if (format != 0) // Color
                throw new Exception("Texture format not supported");

            uint width = reader.ReadUInt32();
            uint height = reader.ReadUInt32();
            uint mips = reader.ReadUInt32();

            if (mips != 1)
                throw new Exception("More than one mips not implemented");

            uint size = reader.ReadUInt32();
            byte[] bytes = reader.ReadBytes((int)size);

            var image = new Image();
            for ( uint iy = 0; iy < height; ++iy )
            {
                for (uint ix = 0; ix < width; ++ix)
                {
                    uint ind = (ix + (iy * width)) * 4;
                    byte r = bytes[ind + 0];
                    byte g = bytes[ind + 1];
                    byte b = bytes[ind + 2];
                    byte a = bytes[ind + 3];
                    image.SetPixel((int)ix, (int)iy, new Color(r / 255f, g / 255f, b / 255f, a / 255f));
                }
            }
            return image;
        }

        public Type GetReadType(string fullDecl)
        {
            return typeof(Image);
        }
    }
}
