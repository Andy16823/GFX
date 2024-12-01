using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Genesis.Graphics
{
    /// <summary>
    /// Represents a font with information about glyph size, spacing, and glyph atlas.
    /// </summary>
    public class Font
    {
        /// <summary>
        /// Gets or sets the name of the font.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Gets or sets the size of each glyph in the font.
        /// </summary>
        public float GlyphSize { get; set; }

        /// <summary>
        /// Gets or sets the spacing between glyphs.
        /// </summary>
        public float Spacing { get; set; }

        /// <summary>
        /// Gets or sets the number of rows in the glyph atlas.
        /// </summary>
        public int Rows { get; set; }

        /// <summary>
        /// Gets or sets the number of columns in the glyph atlas.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Gets or sets the bitmap containing the font atlas.
        /// </summary>
        public Bitmap FontAtlas { get; set; }

        /// <summary>
        /// Gets or sets the render ID associated with the font.
        /// </summary>
        public int RenderID { get; set; }

        /// <summary>
        /// Gets or sets the list of glyphs in the font.
        /// </summary>
        public List<Glyphe> Glyphes { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Font"/> class.
        /// </summary>
        public Font()
        {
            this.Glyphes = new List<Glyphe>();
        }

        /// <summary>
        /// Loads font information from an XML file.
        /// </summary>
        /// <param name="file">The path to the XML file containing font information.</param>
        public void FromFile(String file)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(file);

            XmlNode root = xml.ChildNodes[1];
            XmlNode glyphesNode = root.ChildNodes[0];
            XmlNode atlasNode = root.ChildNodes[1];
            this.GlyphSize = float.Parse(root.Attributes["GlypheWidth"].Value);
            this.Rows = int.Parse(root.Attributes["Rows"].Value);
            this.Column = int.Parse(root.Attributes["Columns"].Value);
            this.Spacing = float.Parse(root.Attributes["LetterSpacing"].Value);
            this.Name = root.Attributes["Name"].Value;

            this.Glyphes.Clear();
            foreach (XmlNode item in glyphesNode.ChildNodes)
            {
                Char character = Char.Parse(item.Attributes["Char"].Value);
                int Row = int.Parse(item.Attributes["Row"].Value);
                int Column = int.Parse(item.Attributes["Column"].Value);
                Glyphe glyphe = new Glyphe(character, Row, Column);
                Console.WriteLine("Glyphe " + character + " loaded");
                this.Glyphes.Add(glyphe);
            }

            FontAtlas = Base64ToImage(atlasNode.InnerText);
        }

        /// <summary>
        /// Converts a base64-encoded image string to a Bitmap.
        /// </summary>
        /// <param name="base64">The base64-encoded image string.</param>
        /// <returns>The decoded Bitmap image.</returns>
        public Bitmap Base64ToImage(string base64)
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            Bitmap image = (Bitmap)Bitmap.FromStream(ms, true);
            ms.Close();
            return image;
        }

        /// <summary>
        /// Gets the glyph associated with a specific character.
        /// </summary>
        /// <param name="character">The character for which to retrieve the glyph.</param>
        /// <returns>The glyph associated with the specified character.</returns>
        public Glyphe GetGlyphe(Char character)
        {
            foreach (var item in Glyphes)
            {
                if(item.Character.Equals(character))
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Loads a system font with a default set of glyphs.
        /// </summary>
        /// <param name="fontName">The name of the system font to load.</param>
        /// <returns>The loaded system font.</returns>
        public static Font LoadSystemFont(string fontName)
        {
            String glyphes = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.:-+!§$%&/()=?,\"'";
            int glypheSize = 256;
            int lines = glyphes.Length / 10;
            int imageWidth = 10 * glypheSize;
            int imageHeight = (lines + 1) * glypheSize;
            Bitmap fontAtlas = new Bitmap(imageWidth, imageHeight);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(fontAtlas);

            Font font = new Font();
            font.Spacing = 0;
            font.GlyphSize = 256;
            font.Name = fontName;
            font.Column = 9;
            font.Rows = lines;

            int i = 0;
            int line = 0;
            foreach (var c in glyphes)
            {
                int x = i * glypheSize;
                int y = line * glypheSize;

                g.DrawImage(RenderGlyphe(c.ToString(), glypheSize, Color.White, fontName, 160), new Point(x, y));
                font.Glyphes.Add(new Glyphe(c, line, i));

                if (i == 9)
                {
                    line++;
                    i = 0;
                }
                else
                {
                    i++;
                }
            }

            font.FontAtlas = fontAtlas;
            return font;
        }

        /// <summary>
        /// Renders a glyph image with the specified parameters.
        /// </summary>
        /// <param name="glypheValue">The value of the glyph to render.</param>
        /// <param name="glypeSize">The size of the glyph.</param>
        /// <param name="color">The color of the glyph.</param>
        /// <param name="fontName">The name of the font to use for rendering.</param>
        /// <param name="fontSize">The size of the font.</param>
        /// <returns>The rendered glyph as a Bitmap image.</returns>
        private static Bitmap RenderGlyphe(String glypheValue, int glypeSize, Color color, string fontName, int fontSize)
        {
            System.Drawing.Font font = new System.Drawing.Font(fontName, fontSize);
            Bitmap glyphe = new Bitmap(glypeSize, glypeSize);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(glyphe);

            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            //g.FillRectangle(new SolidBrush(Color.Red), new RectangleF(0, 0, 256, 256));
            g.DrawString(glypheValue, font, new SolidBrush(color), new RectangleF(0, 0, glypeSize, glypeSize), sf);

            return glyphe;
        }

    }
}
