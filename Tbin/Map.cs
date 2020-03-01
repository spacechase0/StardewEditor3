using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Tbin
{
    /// <summary>
    /// A .tbin format map.
    /// </summary>
    public class Map
    {
        /// <summary>
        /// The map ID
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The map description
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// The map properties
        /// </summary>
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// The tilesheets.
        /// </summary>
        public List<TileSheet> TileSheets { get; set; } = new List<TileSheet>();

        /// <summary>
        /// The layers.
        /// </summary>
        public List<Layer> Layers { get; set; } = new List<Layer>();
        
        public void Load(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                if (new string(reader.ReadChars(6)) != "tBIN10")
                    throw new Exception("File is not a tbin file.");

                Id = ReadStringAndLength(reader);
                Description = ReadStringAndLength(reader);
                Properties = ReadProperties(reader);

                int count = reader.ReadInt32();
                for ( int i = 0; i < count; ++i )
                {
                    TileSheets.Add(ReadTileSheet(reader));
                }

                count = reader.ReadInt32();
                for ( int i = 0; i < count; ++i )
                {
                    Layers.Add(ReadLayer(reader));
                }
            }
        }

        private static Dictionary<string, object> ReadProperties(BinaryReader reader)
        {
            var ret = new Dictionary<string, object>();

            int count = reader.ReadInt32();
            for ( int i = 0; i < count; ++i )
            {
                string key = ReadStringAndLength(reader);

                byte type = reader.ReadByte();
                object value = null;
                switch ( type )
                {
                    case 0: value = reader.ReadByte() > 0; break;
                    case 1: value = reader.ReadInt32(); break;
                    case 2: value = reader.ReadSingle(); break;
                    case 3: value = ReadStringAndLength(reader); break;
                    case 4: throw new Exception("Bad property type");
                }

                ret.Add(key, value);
            }

            return ret;
        }

        private static TileSheet ReadTileSheet(BinaryReader reader)
        {
            TileSheet ret = new TileSheet();

            ret.Id = ReadStringAndLength(reader);
            ret.Description = ReadStringAndLength(reader);
            ret.Image = ReadStringAndLength(reader);
            ret.SheetSize = new Vector2() { X = reader.ReadInt32(), Y = reader.ReadInt32() };
            ret.TileSize = new Vector2() { X = reader.ReadInt32(), Y = reader.ReadInt32() };
            ret.Margin = new Vector2() { X = reader.ReadInt32(), Y = reader.ReadInt32() };
            ret.Spacing = new Vector2() { X = reader.ReadInt32(), Y = reader.ReadInt32() };
            ret.Properties = ReadProperties(reader);

            return ret;
        }

        private static Layer ReadLayer(BinaryReader reader)
        {
            Layer ret = new Layer();

            ret.Id = ReadStringAndLength(reader);
            ret.Visible = reader.ReadByte() > 0;
            ret.Description = ReadStringAndLength(reader);
            ret.LayerSize = new Vector2() { X = reader.ReadInt32(), Y = reader.ReadInt32() };
            ret.TileSize = new Vector2() { X = reader.ReadInt32(), Y = reader.ReadInt32() };
            ret.Properties = ReadProperties(reader);

            string currTilesheet = "";
            ret.Tiles = new List<TileBase>(ret.LayerSize.X * ret.LayerSize.Y);
            for ( int iy = 0; iy < ret.LayerSize.Y; ++iy )
            {
                int ix = 0;
                while ( ix < ret.LayerSize.X )
                {
                    byte c = reader.ReadByte();
                    switch ( c )
                    {
                        case (byte) 'N':
                            {
                                int amt = reader.ReadInt32();
                                for ( int n = 0; n < amt; ++n )
                                {
                                    ret.Tiles.Add(null);
                                }
                                ix += amt;
                            }
                            break;
                        case (byte)'S':
                            ret.Tiles.Add(ReadStaticTile(reader, currTilesheet));
                            ++ix;
                            break;
                        case (byte)'A':
                            ret.Tiles.Add(ReadAnimatedTile(reader));
                            ++ix;
                            break;
                        case (byte)'T':
                            currTilesheet = ReadStringAndLength(reader);
                            break;
                        default:
                            throw new Exception("Bad tile data");
                    }
                }
            }

            return ret;
        }

        private static StaticTile ReadStaticTile(BinaryReader reader, string currTilesheet)
        {
            StaticTile ret = new StaticTile();
            ret.TileSheet = currTilesheet;
            ret.TileIndex = reader.ReadInt32();
            ret.BlendMode = reader.ReadByte();
            ret.Properties = ReadProperties(reader);
            return ret;
        }

        private static AnimatedTile ReadAnimatedTile(BinaryReader reader)
        {
            AnimatedTile ret = new AnimatedTile();

            ret.FrameInterval = reader.ReadInt32();

            int frameCount = reader.ReadInt32();
            ret.Frames.Capacity = frameCount;

            string currTilesheet = "";
            for ( int i = 0; i < frameCount; )
            {
                byte b = reader.ReadByte();
                switch ( b )
                {
                    case (byte)'T':
                        currTilesheet = ReadStringAndLength(reader);
                        break;
                    case (byte)'S':
                        ret.Frames.Add(ReadStaticTile(reader, currTilesheet));
                        ++i;
                        break;
                    default:
                        throw new Exception("Bad animated tile data");
                }
            }

            ret.Properties = ReadProperties(reader);

            return ret;
        }

        private static string ReadStringAndLength(BinaryReader reader)
        {
            int len = reader.ReadInt32();
            return new string(reader.ReadChars(len));
        }
    }
}
