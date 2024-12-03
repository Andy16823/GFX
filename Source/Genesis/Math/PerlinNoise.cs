using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Math
{
    /// <summary>
    /// Represents a point in a noise map with coordinates and a noise value.
    /// </summary>
    public struct NoisePoint
    {
        public float x;
        public float y;
        public float value;

        /// <summary>
        /// Constructor for the NoisePoint struct.
        /// </summary>
        /// <param name="x">The x-coordinate of the point.</param>
        /// <param name="y">The y-coordinate of the point.</param>
        /// <param name="value">The noise value associated with the point.</param>
        public NoisePoint(float x, float y, float value)
        {
            this.x = x;
            this.y = y;
            this.value = value;
        }
    }

    /// <summary>
    /// Class for generating Perlin noise.
    /// </summary>
    public class PerlinNoise
    {
        /// <summary>
        /// Generates Perlin noise at a specific point.
        /// </summary>
        /// <param name="x">The x-coordinate of the point.</param>
        /// <param name="y">The y-coordinate of the point.</param>
        /// <param name="seed">The seed value for the noise generation.</param>
        /// <returns>The generated Perlin noise value at the specified point.</returns>
        private static float Noise(int x, int y, int seed)
        {
            int n = x + y * 57 + seed;
            n = (n << 13) ^ n;
            return (1.0f - ((n * (n * n * 15731 + 789221) + seed) & 0x7fffffff) / 1073741824.0f);
        }

        /// <summary>
        /// Applies smoothing to the noise at a specific point.
        /// </summary>
        /// <param name="x">The x-coordinate of the point.</param>
        /// <param name="y">The y-coordinate of the point.</param>
        /// <param name="seed">The seed value for the noise generation.</param>
        /// <returns>The smoothed noise value at the specified point.</returns>
        private static float SmoothedNoise(float x, float y, int seed)
        {
            int integer_X = (int)x;
            float fractional_X = x - integer_X;

            int integer_Y = (int)y;
            float fractional_Y = y - integer_Y;

            float v1 = Noise(integer_X, integer_Y, seed);
            float v2 = Noise(integer_X + 1, integer_Y, seed);
            float v3 = Noise(integer_X, integer_Y + 1, seed);
            float v4 = Noise(integer_X + 1, integer_Y + 1, seed);

            float i1 = Interpolate(v1, v2, fractional_X);
            float i2 = Interpolate(v3, v4, fractional_X);

            return Interpolate(i1, i2, fractional_Y);
        }

        /// <summary>
        /// Interpolates between two values.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <param name="x">The interpolation factor.</param>
        /// <returns>The interpolated value.</returns>
        private static float Interpolate(float a, float b, float x)
        {
            float ft = x * 3.1415927f;
            float f = (1 - (float)System.Math.Cos(ft)) * 0.5f;
            return a * (1 - f) + b * f;
        }

        /// <summary>
        /// Generates Perlin noise at a specific point with specified parameters.
        /// </summary>
        /// <param name="x">The x-coordinate of the point.</param>
        /// <param name="y">The y-coordinate of the point.</param>
        /// <param name="persistence">The persistence value affecting the amplitude of octaves.</param>
        /// <param name="octaves">The number of octaves to generate.</param>
        /// <param name="seed">The seed value for the noise generation.</param>
        /// <returns>The generated Perlin noise value at the specified point.</returns>
        public static float GenerateNoise(float x, float y, float persistence, int octaves, int seed)
        {
            float total = 0;
            float frequency = 0.1f;
            float amplitude = 1;
            float maxValue = 0;

            for (int i = 0; i < octaves; i++)
            {
                total += SmoothedNoise(x * frequency, y * frequency, seed) * amplitude;
                maxValue += amplitude;
                amplitude *= persistence;
                frequency *= 2;
            }

            return total / maxValue;
        }

        /// <summary>
        /// Generates a 2D noise map with specified dimensions and seed.
        /// </summary>
        /// <param name="width">The width of the noise map.</param>
        /// <param name="height">The height of the noise map.</param>
        /// <param name="seed">The seed value for the noise generation.</param>
        /// <returns>A list of NoisePoint objects representing the generated noise map.</returns>
        public List<NoisePoint> GenerateNoiseMap(int width, int height, int seed)
        {
            List<NoisePoint> noiseMap = new List<NoisePoint>();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float noiseVal = GenerateNoise((float)x, (float)y, 0.5f, 4, seed);
                    noiseMap.Add(new NoisePoint(x, y, noiseVal));
                }
            }

            return noiseMap;
        }
    }
}
