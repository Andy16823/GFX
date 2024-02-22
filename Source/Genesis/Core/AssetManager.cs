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
    /// <summary>
    /// Manages game assets such as textures and fonts.
    /// </summary>
    public class AssetManager
    {
        /// <summary>
        /// List of loaded textures.
        /// </summary>
        public List<Texture> Textures { get; set; }

        /// <summary>
        /// List of loaded fonts.
        /// </summary>
        public List<Graphics.Font> Fonts { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssetManager"/> class.
        /// </summary>
        public AssetManager()
        {
            this.Textures = new List<Texture>();
            this.Fonts = new List<Graphics.Font>();
        }

        /// <summary>
        /// Adds a texture to the asset manager.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <param name="bitmap">The bitmap representing the texture.</param>
        /// <returns>The added texture.</returns>
        public Texture AddTexture(String name, Bitmap bitmap)
        {
            Texture texture = new Texture(name, bitmap);
            this.Textures.Add(texture);
            return texture;
        }

        /// <summary>
        /// Gets a texture by name.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <returns>The texture with the specified name, or null if not found.</returns>
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

        /// <summary>
        /// Gets a font by name.
        /// </summary>
        /// <param name="name">The name of the font.</param>
        /// <returns>The font with the specified name, or null if not found.</returns>
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

        /// <summary>
        /// Initializes the assets in the rendering device.
        /// </summary>
        /// <param name="renderDevice">The rendering device to load the assets into.</param>
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

        /// <summary>
        /// Disposes of the loaded textures and fonts.
        /// </summary>
        /// <param name="game">The game object associated with the assets.</param>
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
        /// Loads textures from the resource folder.
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

        /// <summary>
        /// Loads fonts from the resource folder.
        /// </summary>
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
        /// Adds an font
        /// </summary>
        /// <param name="font">the font to add</param>
        public void AddFont(Genesis.Graphics.Font font)
        {
            this.Fonts.Add(font);
        }

        /// <summary>
        /// Loads a system font
        /// </summary>
        /// <param name="font"></param>
        public void LoadSystemFont(String font)
        {
            this.Fonts.Add(Genesis.Graphics.Font.LoadSystemFont(font));
        }

        /// <summary>
        /// Packs the assets into an asset library.
        /// </summary>
        /// <param name="file">The file path to save the asset library.</param>
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
        /// Imports assets from an asset library.
        /// </summary>
        /// <param name="file">The file path of the asset library to import.</param>
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

        /// <summary>
        /// Gets the resource directory path.
        /// </summary>
        /// <returns>The path to the resource directory.</returns>
        public static String GetRessourcesDirectory()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "/Resources";
        }
    }
}
