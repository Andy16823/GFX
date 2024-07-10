using Genesis.Graphics.RenderDevice;
using Genesis.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyTutorialGame
{
    internal static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Create an window Handle
            Genesis.Core.Window window = new Genesis.Core.Window();
            Viewport viewport = new Viewport(1280, 720);
            var handle = window.CreateWindowHandle("Hello Window", viewport);

            // Create Render Settings for the renderer
            var renderSettings = new RenderSettings()
            {
                gamma = 0.5f
            };

            // Create the game
            var game = new MyGame(new BetaRenderer(handle, renderSettings), viewport);

            // Show and Run the Game
            window.RunGame(game);
        }
    }
}
