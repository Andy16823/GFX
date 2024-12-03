using Genesis.Core;
using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.UI
{
    public class Button : Widget
    {
        public Graphics.Font Font { get; set; }
        public float FontSize { get; set; } = 16f;
        public float FontSpacing { get; set; } = 0.5f;
        public Color ButtonColor { get; set; } = Color.Black;
        public Color ForeColor { get; set; } = Color.White;
        public Color BorderColor { get; set; } = Color.White;
        public Color HoverColor { get; set; } = Color.FromArgb(0, 143, 255);
        public String Text { get; set; }

        public Button(String name, Vec3 location, Vec3 size, String text, Graphics.Font font, WidgetAnchor anchor)
        {
            this.Name = name;
            this.Location = location;
            this.Size = size;   
            this.Text = text;
            this.Anchor = anchor;
            this.Font = font;
        }

        public override void OnRender(Game game, IRenderDevice renderDevice, Scene scene, Canvas canvas)
        {
            base.OnRender(game, renderDevice, scene, canvas);
            // Get the bounds
            var bounds = TransformBounds(new Rect(GetRelativePos(canvas), this.Size), this.Anchor);

            // Render the background
            var bgColor = ButtonColor;
            if (this.Hovered())
            {
                bgColor = HoverColor;
            }
            renderDevice.FillRect(bounds, bgColor);

            // Render the text
            var textWidth = Utils.GetStringWidth(Text, FontSize, FontSpacing);
            var textHeight = Utils.GetStringHeight(Text, FontSize, FontSpacing);

            var textX = bounds.X;
            var textY = bounds.Y;

            renderDevice.DrawString(Text, new Vec3(textX, textY), FontSize, FontSpacing, Font, ForeColor);

            // Render the border
            renderDevice.DrawRect(bounds, BorderColor, 2f);
        }

    }
}
