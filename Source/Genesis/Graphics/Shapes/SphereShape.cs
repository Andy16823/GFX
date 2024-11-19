using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Graphics.Shapes
{
    /// <summary>
    /// Represents a 3D sphere shape with configurable latitude, longitude, and radius.
    /// Provides methods to retrieve vertex positions, normals, texture coordinates, and colors.
    /// </summary>
    public class SphereShape : Shape
    {
        /// <summary>
        /// Gets or sets the number of horizontal segments of the sphere.
        /// </summary>
        public int LatitudeBands { get; set; } = 20;

        /// <summary>
        /// Gets or sets the number of vertical segments of the sphere.
        /// </summary>
        public int LongitudeBands { get; set; } = 20;

        /// <summary>
        /// Gets or sets the radius of the sphere.
        /// </summary>
        public float Radius { get; set; } = 0.5f;

        /// <summary>
        /// Constructs the geometry of the sphere by combining vertex positions and indices.
        /// </summary>
        /// <returns>A float array containing the ordered vertex positions of the sphere.</returns>
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

        /// <summary>
        /// Generates the vertices for the sphere based on latitude and longitude segments and the radius.
        /// </summary>
        /// <param name="latitudebands">The number of horizontal segments.</param>
        /// <param name="longitudebands">The number of vertical segments.</param>
        /// <param name="radius">The radius of the sphere.</param>
        /// <returns>A float array containing the vertex positions for the sphere.</returns>
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

        /// <summary>
        /// Generates the indices required to construct triangles for the sphere's surface.
        /// </summary>
        /// <param name="latitudeBands">The number of horizontal segments.</param>
        /// <param name="longitudeBands">The number of vertical segments.</param>
        /// <returns>An array of integers representing the indices of the sphere's vertices.</returns>
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

        /// <summary>
        /// Retrieves the ordered normals for the sphere's vertices.
        /// </summary>
        /// <returns>A float array containing the normals ordered by vertex indices.</returns>
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

        /// <summary>
        /// Generates the normal vectors for each vertex of the sphere.
        /// </summary>
        /// <param name="latitudebands">The number of horizontal segments.</param>
        /// <param name="longitudebands">The number of vertical segments.</param>
        /// <returns>A float array containing the normals for each vertex.</returns>
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

        /// <summary>
        /// Retrieves the ordered texture coordinates for the sphere's vertices.
        /// </summary>
        /// <returns>A float array containing the texture coordinates ordered by vertex indices.</returns>
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

        /// <summary>
        /// Generates the texture coordinates for each vertex of the sphere.
        /// </summary>
        /// <param name="latitudebands">The number of horizontal segments.</param>
        /// <param name="longitudebands">The number of vertical segments.</param>
        /// <returns>A float array containing the texture coordinates for each vertex.</returns>
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

        /// <summary>
        /// Generates an array of color values for each vertex based on the specified color.
        /// </summary>
        /// <param name="color">The color to apply to each vertex.</param>
        /// <returns>A float array containing RGB color values for each vertex.</returns>
        public float[] GetColors(Color color)
        {
            return GetColors(color, LatitudeBands, LongitudeBands);
        }

        /// <summary>
        /// Retrieves the ordered color values for the sphere's vertices based on the specified color.
        /// </summary>
        /// <param name="color">The color to apply to each vertex.</param>
        /// <returns>A float array containing ordered RGB color values for each vertex.</returns>
        public float[] GetOrderedColors(Color color)
        {
            float r = (float)color.R / 255;
            float g = (float)color.G / 255;
            float b = (float)color.B / 255;

            var indices = GetIndices(LatitudeBands, LongitudeBands);
            var orderedColors = new List<float>();

            // Für jeden Index die entsprechenden Farbwerte (r, g, b) hinzufügen
            foreach (var i in indices)
            {
                orderedColors.Add(r);
                orderedColors.Add(g);
                orderedColors.Add(b);
            }

            return orderedColors.ToArray();
        }

        /// <summary>
        /// Generates an array of RGB color values for each vertex of the sphere.
        /// </summary>
        /// <param name="color">The color to apply to each vertex.</param>
        /// <param name="latitudeBands">The number of horizontal segments.</param>
        /// <param name="longitudeBands">The number of vertical segments.</param>
        /// <returns>A float array containing RGB color values for each vertex.</returns>
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
