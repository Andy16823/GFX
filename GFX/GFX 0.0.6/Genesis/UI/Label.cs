﻿using Genesis.Core;
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
    public class Label : Entity
    {
        public Graphics.Font Font { get; set; }
        public String Text { get; set; }
        public Color TextColor { get; set; }
        public float FontSize { get; set; } = 18.0f;

        /// <summary>
        /// Create a new instance of the label
        /// </summary>
        /// <param name="name"></param>
        /// <param name="location"></param>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <param name="color"></param>
        public Label(String name, Vec3 location, String text, Graphics.Font font, Color color)
        {
            Name = name;
            Location = location;
            Text = text;   
            Font = font;
            TextColor = color;
            Size = new Vec3(GetStringWidht(), FontSize);
        }

        /// <summary>
        /// Renders the label
        /// </summary>
        /// <param name="game"></param>
        /// <param name="renderDevice"></param>
        /// <param name="scene"></param>
        /// <param name="canvas"></param>
        public override void OnRender(Game game, IRenderDevice renderDevice, Scene scene, Canvas canvas)
        {
            base.OnRender(game, renderDevice, scene, canvas);
            Vec3 loc = GetRelativePos(canvas);
            renderDevice.DrawString(Text, loc, FontSize, 0.5f,Font, TextColor);
        }

        /// <summary>
        /// Returns the width from the string in float
        /// </summary>
        /// <returns></returns>
        private float GetStringWidht()
        {
            int chars = Text.Length;
            float baseWidth = chars * FontSize;
            float spaceWidth = (float)(FontSize * 0.5);
            float spacingWidth = spaceWidth * (chars -1);
            return baseWidth - spacingWidth;
        }

        /// <summary>
        /// Returns the display bounds from the string
        /// </summary>
        /// <param name="location">Location from the string</param>
        /// <returns></returns>
        private Rect GetStringBounds(Vec3 location)
        {
            Rect rect = new Rect();
            rect.X = location.X;
            rect.Y = location.Y;
            rect.Width = GetStringWidht();
            rect.Height = FontSize;
            return rect;
        }

    }
}