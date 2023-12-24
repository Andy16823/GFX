using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core
{
    public class Light : GameElement
    {
        public Light(String name, Vec3 location)
        {
            this.Name = name;
            this.Location = location;
        }

        public Color LightColor { get; set; } = Color.FromArgb(255, 255, 255);
        public float Intensity { get; set; }
        
        /// <summary>
        /// Returns the sun position relative to the camera
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        public Vec3 GetLightDirection(Camera camera)
        {
            Vec3 camPos = camera.Location;
            Vec3 lightDirection = Vec3.Normalized(this.Location -  camPos);
            return lightDirection;
        }

        /// <summary>
        /// Returns the light color in rgb
        /// </summary>
        /// <returns></returns>
        public Vec3 GetLightColor()
        {
            float r = (float)LightColor.R / 255;
            float g = (float)LightColor.G / 255;
            float b = (float)LightColor.B / 255;
            return new Vec3(r, g, b);
        }

    }
}
