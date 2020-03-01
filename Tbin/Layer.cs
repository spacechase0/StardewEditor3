using System;
using System.Collections.Generic;
using System.Text;

namespace Tbin
{
    /// <summary>
    /// A .tbin map layer.
    /// </summary>
    public class Layer
    {
        /// <summary>
        /// The layer ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// If the layer is visible or not.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// The layer description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The size of a layer, in tiles.
        /// </summary>
        public Vector2 LayerSize { get; set; }

        /// <summary>
        /// The size of a tile, in pixels.
        /// </summary>
        public Vector2 TileSize { get; set; }

        /// <summary>
        /// The layer properties.
        /// </summary>
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// The list of tiles.
        /// </summary>
        public List<TileBase> Tiles { get; set; } = new List<TileBase>();
    }
}
