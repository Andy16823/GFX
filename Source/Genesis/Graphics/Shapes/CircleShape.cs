using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shapes
{
    public class CircleShape : Shape
    {
        public int Segments { get; set; } = 64;

        public override float[] GetShape()
        {
            List<float> vertices = new List<float>();  

            const float radius = 0.5f;

            vertices.Add(0.0f);
            vertices.Add(0.0f);
            vertices.Add(0.0f);

            for (int i = 0; i < Segments; i++)
            {
                float angle = 2 * (float)System.Math.PI * i / Segments;
                float x = radius * (float)System.Math.Cos(angle);
                float y = radius * (float)System.Math.Sin(angle);

                // Vertex position
                vertices.Add(x);
                vertices.Add(y);
                vertices.Add(0);

            }

            // Close the circle by connecting the last point to the first point
            float firstX = vertices[3];
            float firstY = vertices[4];
            float firstZ = vertices[5];

            vertices.Add(firstX);
            vertices.Add(firstY);
            vertices.Add(firstZ);

            return vertices.ToArray();
        }
    }
}
