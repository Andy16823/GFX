using Genesis.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Genesis.Core
{
    public class AssetManager
    {
        public List<Texture> Textures { get; set; }
        public List<Graphics.Font> Fonts { get; set; }

        public AssetManager()
        {
            this.Textures = new List<Texture>();
            this.Fonts = new List<Graphics.Font>();
        }

        public Texture AddTexture(String name, Bitmap bitmap)
        {
            Texture texture = new Texture(name, bitmap);
            this.Textures.Add(texture);
            return texture;
        }

        public Texture GetTexture(String name)
        {
            foreach (var item in Textures)
            {
                if(item.Name.Equals(name))
                {
                    return item;
                }
            }
            return null;
        }

        public Graphics.Font GetFont(String name)
        {
            foreach (var item in Fonts)
            {
                if(item.Name.Equals(name))
                {
                    return item;
                }
            }
            return null;
        }

        public void Init(IRenderDevice renderDevice)
        {
            foreach (var item in Textures)
            {
                renderDevice.LoadTexture(item);
            }
            foreach (var item in Fonts)
            {
                renderDevice.LoadFont(item);
            }
        }

        public void DisposeTextures(Game game)
        {
            foreach (var item in Textures)
            {
                game.RenderDevice.DisposeTexture(item);
            }
            foreach (var item in Fonts)
            {
                game.RenderDevice.DisposeFont(item);
            }
        }

        /// <summary>
        /// Loads Textures from the ressource folder
        /// </summary>
        public void LoadTextures()
        {
            String ressources = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Resources";
            foreach(var file in Directory.GetFiles(ressources))
            {
                FileInfo info = new FileInfo(file);
                if(info.Extension.Equals(".png") || info.Extension.Equals(".jpg"))
                {
                    this.Textures.Add(new Texture(info.Name, new Bitmap(file)));
                    Console.WriteLine("Texture " + info.Name + " loaded!");
                }
            }
        }

        public void LoadFonts()
        {
            String ressources = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Resources";
            foreach (var file in Directory.GetFiles(ressources))
            {
                FileInfo info = new FileInfo(file);
                if (info.Extension.Equals(".gff"))
                {
                    Graphics.Font font = new Graphics.Font();
                    font.FromFile(file);
                    Fonts.Add(font);
                    Console.WriteLine("font " + font.Name + " loaded!");
                }
            }
        } 

        /// <summary>
        /// Packs the assets as an asset libary
        /// </summary>
        /// <param name="file"></param>
        public void PackAssets(String file)
        {
            XmlDocument xml = new XmlDocument();
            XmlDeclaration xmlDeclaration = xml.CreateXmlDeclaration("1.0", "UTF-8", null);
            
            var texturesNode = xml.CreateElement("Textures");
            foreach (var texture in this.Textures)
            {
                var textureNode = xml.CreateElement("Texture");
                XmlAttribute attribute = xml.CreateAttribute("Name");
                attribute.Value = texture.Name.ToString();
                textureNode.Attributes.Append(attribute);

                var textureBitmapNode = xml.CreateElement("Bitmap");
                textureBitmapNode.InnerText = Utils.ConvertBitmapToBase64(texture.Bitnmap);
                textureNode.AppendChild(textureBitmapNode);

                texturesNode.AppendChild(textureNode);
            }
            xml.AppendChild(texturesNode);
            xml.Save(file);
        }

        /// <summary>
        /// Import assets from a libary
        /// </summary>
        /// <param name="file"></param>
        public void ImportAssetLibary(String file)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(file);

            var texturesNode = xml.ChildNodes[0];
            foreach (XmlElement textureNode in texturesNode.ChildNodes)
            {
                var name = textureNode.Attributes["Name"].Value;
                var bitmap = Utils.ConvertBase64ToBitmap(textureNode.ChildNodes[0].InnerText);
                this.Textures.Add(new Texture(name, bitmap));
            }
        }

        public static String GetRessourcesDirectory()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Resources";
        }
    }
}
