using Genesis.Core;
using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.UI
{
    /// <summary>
    /// Represents an image button widget in the UI.
    /// </summary>
    public class ImageButton : Widget
    {
        /// <summary>
        /// Gets or sets the normal texture displayed when the button is not hovered.
        /// </summary>
        public Texture NormalTexture { get; set; }

        /// <summary>
        /// Gets or sets the hover texture displayed when the button is hovered.
        /// </summary>
        public Texture HoverTexture { get; set; }

        /// <summary>
        /// Creates a new instance of the ImageButton class.
        /// </summary>
        /// <param name="name">The name of the image button.</param>
        /// <param name="location">The location of the image button.</param>
        /// <param name="size">The size of the image button.</param>
        /// <param name="normalTexture">The normal texture displayed when the button is not hovered.</param>
        /// <param name="hoverTexture">The hover texture displayed when the button is hovered.</param>
        public ImageButton(String name, Vec3 location, Vec3 size, Texture normalTexture, Texture hoverTexture)
        {
            this.Name= name;
            this.Location= location;    
            this.Size= size;
            this.NormalTexture= normalTexture;
            this.HoverTexture= hoverTexture;
        }

        /// <summary>
        /// Renders the image button on the screen.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <param name="renderDevice">The render device.</param>
        /// <param name="scene">The scene instance.</param>
        /// <param name="canvas">The canvas containing the image button.</param>
        public override void OnRender(Game game, IRenderDevice renderDevice, Scene scene, Canvas canvas)
        {
            base.OnRender(game, renderDevice, scene, canvas);
            Vec3 loc = GetRelativePos(canvas);
            if (IsHover(game,scene,canvas))
            {
                renderDevice.DrawSprite(loc, this.Size, this.HoverTexture);
            }
            else
            {
                renderDevice.DrawSprite(loc, this.Size, this.NormalTexture);
            }
        }

    }
}
