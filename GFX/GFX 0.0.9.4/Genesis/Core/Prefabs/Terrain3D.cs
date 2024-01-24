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

namespace Genesis.Core.Prefabs
{
    public struct TerrainData{
        public float[] verticies;
        public float[] colors;
        public int tris;
        public int cellsX;
        public int cellsZ;
        public int cellWidth;
        public int cellHeight;
    }

    public class Terrain3D : GameElement
    {
        public TerrainData TerrainData { get; set; }
        public Color Color { get; set; } = Color.DarkGray;
        public Color AltColor { get; set; } = Color.White;

        public Terrain3D(String name, Vec3 location)
        {
            this.Name = name;
            this.Location = location;
            this.Size = new Vec3(1);
            this.Rotation = Vec3.Zero();
            TerrainData = GenerateTerrainData(32, 32, 1, 1, Color, AltColor);
        }

        public Terrain3D(String name, Vec3 location, float heightValue, Bitmap heightmap)
        {
            this.Name = name;
            this.Location = location;
            this.Size = new Vec3(1);
            this.Rotation = Vec3.Zero();
            TerrainData = GenerateTerrain(heightmap, heightValue, 1);
        }

        public Terrain3D(String name, Vec3 location, int cellsX, int cellsZ, int cellWidth, int cellHeight)
        {
            this.Name = name;
            this.Location = location;
            this.Size = new Vec3(1);
            this.Rotation = Vec3.Zero();
            TerrainData = GenerateTerrainData(cellsX, cellsZ, cellWidth, cellHeight, Color, AltColor);
        }

        /// <summary>
        /// Generates the terrain data
        /// </summary>
        /// <param name="cellsX"></param>
        /// <param name="cellsZ"></param>
        /// <param name="cellWidth"></param>
        /// <param name="cellHeight"></param>
        /// <param name="colorA"></param>
        /// <param name="colorB"></param>
        /// <returns></returns>
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
        /// Generates a tile for the terrain grid
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="cellWidth"></param>
        /// <param name="cellHeight"></param>
        /// <returns></returns>
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
        /// Gets the float values for a terrain tile
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
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

        public override void Init(Game game, IRenderDevice renderDevice)
        {
            base.Init(game, renderDevice);
        }

        public override void OnRender(Game game, IRenderDevice renderDevice)
        {
            base.OnRender(game, renderDevice);
            renderDevice.DrawGameElement(this);
        }

    }
}
