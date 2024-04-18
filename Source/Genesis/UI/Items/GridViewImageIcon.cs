using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genesis.UI.Items
{
    public class GridViewImageIcon : IItem
    {
        public Texture Texture { get; set; }

        public GridViewImageIcon(String name, Texture texture)
        {
            this.Texture = texture;
            this.Name = name;
        }

        public override void OnRender(IRenderDevice renderer, Widget parent, Rect screenBounds)
        {
            var grid = (GridView)parent;

            var x = screenBounds.X;
            var y = screenBounds.Y;
            var width = screenBounds.Width - (grid.Padding * 2);
            var height = screenBounds.Height - (grid.Padding * 2);

            //renderer.FillRect(new Rect(x, y, width, height), Color.Blue);
            renderer.DrawSprite(new Vec3(x,y), new Vec3(width, height), Texture);

            if(!String.IsNullOrEmpty(this.Text) && grid.Font != null) {
                
            }

        }
    }
}
