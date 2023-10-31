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
    public class Font
    {
        public String Name { get; set; }
        public float GlyphSize { get; set; }
        public float Spacing { get; set; }
        public int Rows { get; set; }
        public int Column { get; set; }
        public Bitmap FontAtlas { get; set; }
        public int RenderID { get; set; }
        public List<Glyphe> Glyphes { get; set; }

        public Font()
        {
            this.Glyphes = new List<Glyphe>();
        }

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

        public Bitmap Base64ToImage(string base64)
        {
            byte[] imageBytes = Convert.FromBase64String(base64);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            Bitmap image = (Bitmap)Bitmap.FromStream(ms, true);
            ms.Close();
            return image;
        }

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

    }
}
