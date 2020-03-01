using System;
using System.Collections.Generic;
using System.Text;

namespace Tbin
{
    /// <summary>
    /// A single tile in a layer.
    /// </summary>
    public class TileBase
    {
        /// <summary>
        /// The properties for this tile.
        /// </summary>
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// A static, non-animated tile.
    /// </summary>
    public class StaticTile : TileBase
    {
        /// <summary>
        /// The tilesheet this tile pulls from.
        /// </summary>
        public string TileSheet { get; set; }

        /// <summary>
        /// The index in the tilesheet of this tile.
        /// </summary>
        public int TileIndex { get; set; }

        /// <summary>
        /// The blend mode of this tile.
        /// </summary>
        public byte BlendMode { get; set; }
    }

    /// <summary>
    /// An animated tile.
    /// </summary>
    public class AnimatedTile : TileBase
    {
        /// <summary>
        /// The interval of animation per frame, in milliseconds.
        /// </summary>
        public int FrameInterval { get; set; }

        /// <summary>
        /// The frames of this tile, as static tiles.
        /// </summary>
        public List<StaticTile> Frames { get; set; } = new List<StaticTile>();
    }
}
