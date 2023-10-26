using Genesis.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }
}
