using System;
using System.Collections.Generic;
using System.Text;

namespace Tbin
{
    /// <summary>
    /// A tilesheet in the map.
    /// </summary>
    public class TileSheet
    {
        /// <summary>
        /// The ID of this tilesheet.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// The description of this tilesheet.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The relative path to the image of this tilesheet.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// The size of the sheet, in tiles.
        /// </summary>
        public Vector2 SheetSize { get; set; }
        
        /// <summary>
        /// The size of each tile, in pixels.
        /// </summary>
        public Vector2 TileSize { get; set; }

        /// <summary>
        /// The margin around the tilesheet, in pixels.
        /// </summary>
        public Vector2 Margin { get; set; }

        /// <summary>
        /// The spacing between each tile, in pixels.
        /// </summary>
        public Vector2 Spacing { get; set; }

        /// <summary>
        /// The properties of this tilesheet.
        /// </summary>
        public Dictionary<string, object> Properties { get; set; } = new Dictionary<string, object>();
    }
}
