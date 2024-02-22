using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Genesis.Core.GameElements
{
    /// <summary>
    /// Represents a 3D terrain element.
    /// </summary>
    public struct TerrainData{
        public float[] verticies;
        public float[] colors;
        public int tris;
        public int cellsX;
        public int cellsZ;
        public int cellWidth;
        public int cellHeight;
    }

    /// <summary>
    /// Represents a 3D terrain element.
    /// </summary>
    public class Terrain3D : GameElement
    {
        /// <summary>
        /// Gets or sets the terrain data.
        /// </summary>
        public TerrainData TerrainData { get; set; }

        /// <summary>
        /// Gets or sets the color of the terrain.
        /// </summary>
        public Color Color { get; set; } = Color.DarkGray;

        /// <summary>
        /// Gets or sets the alternate color of the terrain.
        /// </summary>
        public Color AltColor { get; set; } = Color.White;

        /// <summary>
        /// Initializes a new instance of the Terrain3D class with default values.
        /// </summary>
        /// <param name="name">The name of the terrain.</param>
        /// <param name="location">The initial location of the terrain.</param>
        public Terrain3D(String name, Vec3 location)
        {
            this.Name = name;
            this.Location = location;
            this.Size = new Vec3(1);
            this.Rotation = Vec3.Zero();
            TerrainData = GenerateTerrainData(32, 32, 1, 1, Color, AltColor);
        }

        /// <summary>
        /// Initializes a new instance of the Terrain3D class with specified name, location, height value, and heightmap.
        /// </summary>
        /// <param name="name">The name of the terrain.</param>
        /// <param name="location">The initial location of the terrain.</param>
        /// <param name="heightValue">The height value of the terrain.</param>
        /// <param name="heightmap">The heightmap for generating the terrain.</param>
        public Terrain3D(String name, Vec3 location, float heightValue, Bitmap heightmap)
        {
            this.Name = name;
            this.Location = location;
            this.Size = new Vec3(1);
            this.Rotation = Vec3.Zero();
            TerrainData = GenerateTerrain(heightmap, heightValue, 1);
        }

        /// <summary>
        /// Initializes a new instance of the Terrain3D class with specified name, location, cell counts, and cell dimensions.
        /// </summary>
        /// <param name="name">The name of the terrain.</param>
        /// <param name="location">The initial location of the terrain.</param>
        /// <param name="cellsX">The number of cells in the X direction.</param>
        /// <param name="cellsZ">The number of cells in the Z direction.</param>
        /// <param name="cellWidth">The width of each cell.</param>
        /// <param name="cellHeight">The height of each cell.</param>
        public Terrain3D(String name, Vec3 location, int cellsX, int cellsZ, int cellWidth, int cellHeight)
        {
            this.Name = name;
            this.Location = location;
            this.Size = new Vec3(1);
            this.Rotation = Vec3.Zero();
            TerrainData = GenerateTerrainData(cellsX, cellsZ, cellWidth, cellHeight, Color, AltColor);
        }

        /// <summary>
        /// Generates terrain data based on specified parameters.
        /// </summary>
        public static TerrainData GenerateTerrainData(int cellsX, int cellsZ, int cellWidth, int cellHeight, Color colorA, Color colorB)
        {
            TerrainData terrainData = new TerrainData();
            terrainData.cellsX = cellsX;
            terrainData.cellsZ = cellsZ;
            terrainData.cellWidth = cellWidth;
            terrainData.cellHeight = cellHeight;

            List<float> floats = new List<float>();
            List<float> colors = new List<float>();


            for (int z = 0; z < cellsZ; z++)
            {
                for (int x = 0; x < cellsX; x++)
                {
                    float posX = x * cellWidth;
                    float posZ = z * cellHeight;
                    floats.AddRange(GenerateTerrainTile(posX, posZ, cellWidth, cellHeight));
                    if ((x + z) % 2 == 0)
                    {
                        colors.AddRange(CreateTerrainTileColor(colorA));
                    }
                    else
                    {
                        colors.AddRange(CreateTerrainTileColor(colorB));
                    }
                }
            }
            terrainData.verticies = floats.ToArray();
            terrainData.colors = colors.ToArray();
            terrainData.tris = terrainData.verticies.Length / 3;
            return terrainData;
        }

        /// <summary>
        /// Generates terrain data from a heightmap.
        /// </summary>
        public static TerrainData GenerateTerrain(Bitmap heightmap, float heightScale, float cellSize)
        {
            TerrainData terrainData = new TerrainData();
            terrainData.cellHeight = (int)cellSize;
            terrainData.cellWidth = (int)cellSize;
            terrainData.cellsX = heightmap.Width;
            terrainData.cellsZ = heightmap.Height;

            List<float> floats = new List<float>();
            List<float> colors = new List<float>();

            for (int z = 0; z < heightmap.Height; z++)
            {
                for (int x = 0; x < heightmap.Width; x++)
                {
                    Color pixelColor = heightmap.GetPixel(x, z);

                    // Calculate grayscale value
                    float grayscaleValue = ((pixelColor.R + pixelColor.G + pixelColor.B) / 255) / 3.0f;
                    float heightValue = grayscaleValue * heightScale;
                    float posX = x * cellSize;
                    float posZ = z * cellSize;
                    floats.Add(posX);
                    floats.Add(heightValue);
                    floats.Add(posZ);
                    if ((x + z) % 2 == 0)
                    {
                        colors.AddRange(CreateTerrainTileColor(Color.White));
                    }
                    else
                    {
                        colors.AddRange(CreateTerrainTileColor(Color.Gray));
                    }
                }
            }
            terrainData.verticies = floats.ToArray();
            terrainData.colors = colors.ToArray();
            terrainData.tris = terrainData.verticies.Length / 3;
            return terrainData;
        }

        /// <summary>
        /// Generates a tile for the terrain grid.
        /// </summary>
        public static float[] GenerateTerrainTile(float x, float z, int cellWidth, int cellHeight)
        {
            return new float[] {
                x, 0.0f, z,                             // Front Left
                x, 0.0f, z + cellHeight,                // Back Left
                x + cellWidth, 0.0f, z,                 // Front Right

                x + cellWidth, 0.0f, z,                 // Front Right
                x, 0.0f, z + cellHeight,                // Back Left
                x + cellWidth, 0.0f, z + cellHeight     // Back Right
            };
        }

        /// <summary>
        /// Creates color data for a terrain tile.
        /// </summary>
        public static float[] CreateTerrainTileColor(Color color)
        {
            float[] c = Utils.ConvertColor(color);
            return new float[]
            {
                c[0], c[1], c[2],
                c[0], c[1], c[2],
                c[0], c[1], c[2],

                c[0], c[1], c[2],
                c[0], c[1], c[2],
                c[0], c[1], c[2]
            };
        }

        /// <summary>
        /// Initializes the terrain element.
        /// </summary>
        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
        }

        /// <summary>
        /// Renders the terrain element.
        /// </summary>
        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            base.OnRender(game, renderDevice);
            renderDevice.DrawGameElement(this);
        }

    }
}
