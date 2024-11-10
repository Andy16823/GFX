using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shapes
{
    public class SphereShape : Shape
    {
        public int LatitudeBands { get; set; } = 20;
        public int LongitudeBands { get; set; } = 20;
        public float Radius { get; set; } = 0.5f;

        public override float[] GetShape()
        {
            var vertices = SphereShape.GetVertices(LatitudeBands, LongitudeBands, Radius);
            var indices = GetIndices(LatitudeBands, LongitudeBands);
            var shape = new List<float>();

            // Für jeden Index aus dem indices-Array, die (x, y, z) Koordinaten hinzufügen
            foreach (var i in indices)
            {
                shape.Add(vertices[i * 3]);       // x-Wert
                shape.Add(vertices[i * 3 + 1]);   // y-Wert
                shape.Add(vertices[i * 3 + 2]);   // z-Wert
            }

            return shape.ToArray();
        }

        public static float[] GetVertices(int latitudebands, int longitudebands, float radius)
        {
            List<float> vertices = new List<float>();

            for (int lat = 0; lat <= latitudebands; lat++)
            {
                float theta = (float)(lat * System.Math.PI / latitudebands);
                float sinTheta = (float)System.Math.Sin(theta);
                float cosTheta = (float)System.Math.Cos(theta);

                for (int lon = 0; lon <= longitudebands; lon++)
                {
                    float phi = (float)(lon * 2 * System.Math.PI / longitudebands);
                    float sinPhi = (float)System.Math.Sin(phi);
                    float cosPhi = (float)System.Math.Cos(phi);

                    float x = cosPhi * sinTheta;
                    float y = cosTheta;
                    float z = sinPhi * sinTheta;

                    vertices.Add(radius * x);
                    vertices.Add(radius * y);
                    vertices.Add(radius * z);
                }
            }
            return vertices.ToArray();
        }

        public static int[] GetIndices(int latitudeBands, int longitudeBands)
        {
            List<int> indices = new List<int>();

            for (int lat = 0; lat < latitudeBands; lat++)
            {
                for (int lon = 0; lon < longitudeBands; lon++)
                {
                    // Berechne die Indizes der Eckpunkte für die beiden Dreiecke der Fläche
                    int first = (lat * (longitudeBands + 1)) + lon;
                    int second = first + longitudeBands + 1;

                    // Erster Dreieck der Fläche
                    indices.Add(first);
                    indices.Add(second);
                    indices.Add(first + 1);

                    // Zweites Dreieck der Fläche
                    indices.Add(second);
                    indices.Add(second + 1);
                    indices.Add(first + 1);
                }
            }

            return indices.ToArray();
        }

        public float[] GetOrderedNormals()
        {
            var normals = GetNormals(LatitudeBands, LongitudeBands); // Originale Normals für alle Vertices
            var indices = GetIndices(LatitudeBands, LongitudeBands);
            var orderedNormals = new List<float>();

            // Für jeden Index die entsprechenden Normalen (x, y, z) hinzufügen
            foreach (var i in indices)
            {
                orderedNormals.Add(normals[i * 3]);     // x-Wert der Normalen
                orderedNormals.Add(normals[i * 3 + 1]); // y-Wert der Normalen
                orderedNormals.Add(normals[i * 3 + 2]); // z-Wert der Normalen
            }

            return orderedNormals.ToArray();
        }

        public static float[] GetNormals(int latitudebands, int longitudebands)
        {
            List<float> normals = new List<float>();

            for (int lat = 0; lat <= latitudebands; lat++)
            {
                float theta = (float)(lat * System.Math.PI / latitudebands);
                float sinTheta = (float)System.Math.Sin(theta);
                float cosTheta = (float)System.Math.Cos(theta);

                for (int lon = 0; lon <= longitudebands; lon++)
                {
                    float phi = (float)(lon * 2 * System.Math.PI / longitudebands);
                    float sinPhi = (float)System.Math.Sin(phi);
                    float cosPhi = (float)System.Math.Cos(phi);

                    float x = cosPhi * sinTheta;
                    float y = cosTheta;
                    float z = sinPhi * sinTheta;

                    normals.Add(x);
                    normals.Add(y);
                    normals.Add(z);
                }
            }

            return normals.ToArray();
        }

        public float[] GetOrderedTextureCoordinates()
        {
            var textureCoords = GetTextureCoordinates(LatitudeBands, LongitudeBands); // Originale Texture-Koordinaten für alle Vertices
            var indices = GetIndices(LatitudeBands, LongitudeBands);
            var orderedTextureCoords = new List<float>();

            // Für jeden Index die entsprechenden Texture-Koordinaten (u, v) hinzufügen
            foreach (var i in indices)
            {
                orderedTextureCoords.Add(textureCoords[i * 2]);       // u-Wert der Texture-Koordinate
                orderedTextureCoords.Add(textureCoords[i * 2 + 1]);   // v-Wert der Texture-Koordinate
            }

            return orderedTextureCoords.ToArray();
        }

        public static float[] GetTextureCoordinates(int latitudebands, int longitudebands)
        {
            List<float> textureCoords = new List<float>();

            for (int lat = 0; lat <= latitudebands; lat++)
            {
                for (int lon = 0; lon <= longitudebands; lon++)
                {
                    float u = (float)lon / longitudebands;
                    float v = (float)lat / latitudebands;
                    textureCoords.Add(u);
                    textureCoords.Add(v);
                }
            }

            return textureCoords.ToArray();
        }

        public float[] GetColors(Color color)
        {
            return GetColors(color, LatitudeBands, LongitudeBands);
        }

        public static float[] GetColors(Color color, int latitudeBands, int longitudeBands)
        {
            float r = (float)color.R / 255;
            float g = (float)color.G / 255;
            float b = (float)color.B / 255;

            // Die Anzahl der Vertices in der Kugel entspricht (latitudeBands + 1) * (longitudeBands + 1)
            int vertexCount = (latitudeBands + 1) * (longitudeBands + 1);
            float[] colorArray = new float[vertexCount * 3]; // * 3 für RGB-Werte pro Vertex

            for (int i = 0; i < vertexCount; i++)
            {
                colorArray[i * 3] = r;
                colorArray[i * 3 + 1] = g;
                colorArray[i * 3 + 2] = b;
            }

            return colorArray;
        }

    }
}
