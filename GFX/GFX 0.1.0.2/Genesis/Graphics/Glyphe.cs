using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics
{
    /// <summary>
    /// Represents a glyph in a font texture atlas.
    /// </summary>
    public class Glyphe
    {
        /// <summary>
        /// Gets or sets the character associated with the glyph.
        /// </summary>
        public Char Character { get; set; }

        /// <summary>
        /// Gets or sets the row index of the glyph in the texture atlas.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// Gets or sets the column index of the glyph in the texture atlas.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Glyphe"/> class.
        /// </summary>
        /// <param name="character">The character associated with the glyph.</param>
        /// <param name="row">The row index of the glyph in the texture atlas.</param>
        /// <param name="column">The column index of the glyph in the texture atlas.</param>
        public Glyphe(char character, int row, int column)
        {
            Character = character;
            Row = row;
            Column = column;
        }
    }
}
