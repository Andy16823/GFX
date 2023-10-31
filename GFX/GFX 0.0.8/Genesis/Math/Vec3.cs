using GlmSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Math
{
    /// <summary>
    /// Vector for coordinates
    /// </summary>
    public class Vec3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        /// <summary>
        /// Creates a new Vector
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Vec3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Creates a new Vector
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Vec3(float x, float y)
        {
            X = x;
            Y = y;
            Z = 0.0f;
        }

        /// <summary>
        /// Creates a new Vector
        /// </summary>
        /// <param name="value"></param>
        public Vec3(float value)
        {
            X = value;
            Y = value;
            Z = value;
        }

        public Vec3(Size size)
        {
            X = size.Width;
            Y = size.Height;
            Z = 0f;
        }

        public Vec3(SizeF size) 
        {
            X = size.Width;
            Y = size.Height;
            Z = 0f;
        }

        public Vec3(Point point)
        {
            X = point.X;
            Y = point.Y;
            Z = 0f;
        }

        public Vec3(PointF point)
        {
            X = point.X;
            Y = point.Y;
            Z = 0f;
        }

        /// <summary>
        /// Create a new vector with X = 0, Y = 0, Z = 0
        /// </summary>
        /// <returns></returns>
        public static Vec3 Zero()
        {
            return new Vec3(0.0f);
        }

        /// <summary>
        /// Returns the offset angle to the vector
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public float Degres(float x, float y)
        {
            float rad = (float) System.Math.Atan2(x - X, y - Y);
            double degrees = (180 / System.Math.PI) * rad;
            if(degrees < 0)
            {
                //degrees += 360f;
            }
            return (float)System.Math.Abs(degrees - 180f);
        }

        /// <summary>
        /// Returns the offset angle to the vector
        /// </summary>
        /// <param name="vec3"></param>
        /// <returns></returns>
        public float Degres(Vec3 vec3)
        {
            return this.Degres(vec3.X, vec3.Y);
        }

        /// <summary>
        /// Returns a new vector with the offset cordinates towards the given vector.
        /// </summary>
        /// <param name="degrees"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        public Vec3 Towards(float degrees, float dist)
        {
            float radians = (float)((System.Math.PI / 180) * degrees);
            float x = (float)(dist * System.Math.Cos(radians));
            float y = (float)(dist * System.Math.Sin(radians));
            return new Vec3(x, y);
        }

        /// <summary>
        /// Returns a new vector towards the given direction.
        /// </summary>
        /// <param name="degrees"></param>
        /// <param name="start"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        public static Vec3 Towards2D(float degrees, Vec3 start, float dist)
        {
            Vec3 twd = start.Towards(degrees, dist);
            return new Vec3(start.X + twd.X, start.Y + twd.Y);
        }

        /// <summary>
        /// Returns the distance between two vectors
        /// </summary>
        /// <param name="vec3"></param>
        /// <returns></returns>
        public float Distance(Vec3 vec3)
        {
            float xDist = X - vec3.X;
            float yDist = Y - vec3.Y;

            return (float)System.Math.Sqrt(xDist* xDist + yDist * yDist);
        }

        /// <summary>
        /// Adds the value from a vector
        /// </summary>
        /// <param name="vec"></param>
        public void Add(Vec3 vec)
        {
            X += vec.X;
            Y += vec.Y;
        }

        /// <summary>
        /// Adds the values to the vector
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Add(float x, float y)
        {
            X += x;
            Y += y;
        }

        /// <summary>
        /// Add operator
        /// </summary>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        /// <returns></returns>
        public static Vec3 operator +(Vec3 vec1, Vec3 vec2)
        {
            float x = vec1.X + vec2.X;
            float y = vec1.Y + vec2.Y;
            float z = vec1.Z + vec2.Z;
            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Multiply with float
        /// </summary>
        /// <param name="vec"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Vec3 operator *(Vec3 vec, float value)
        {
            float x = vec.X * value;
            float y = vec.Y * value;
            float z = vec.Z * value;
            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Sub operator
        /// </summary>
        /// <param name="vec1"></param>
        /// <param name="vec2"></param>
        /// <returns></returns>
        public static Vec3 operator -(Vec3 vec1, Vec3 vec2)
        {
            float x = vec1.X - vec2.X;
            float y = vec1.Y - vec2.Y;
            float z = vec1.Z - vec2.Z;
            return new Vec3(x, y, z);
        }

        public static Vec3 Cross(Vec3 vec1, Vec3 vec2)
        {
            float x = vec1.Y * vec2.Z - vec1.Z * vec2.Y;
            float y = vec1.Z * vec2.X - vec1.X * vec2.Z;
            float z = vec1.X * vec2.Y - vec1.Y * vec2.X;
            return new Vec3(x, y, z);
        }

        public static float Dot(Vec3 vec1, Vec3 vec2)
        {
            return vec1.X * vec2.X + vec1.Y * vec2.Y + vec1.Z * vec2.Z;
        }

        public Vec3 Normalize()
        {
            float length = (float)System.Math.Sqrt(X * X + Y * Y + Z * Z);
            if (length != 0)
            {
                return new Vec3(X / length, Y / length, Z / length);
            }
            else
            {
                return new Vec3(0, 0, 0); // or throw an exception, depending on your preference
            }
        }

        public static Vec3 Normalized(Vec3 vec)
        {
            float length = (float)System.Math.Sqrt(vec.X * vec.X + vec.Y * vec.Y + vec.Z * vec.Z);
            if (length != 0)
            {
                return new Vec3(vec.X / length, vec.Y / length, vec.Z / length);
            }
            else
            {
                return new Vec3(0, 0, 0); // or throw an exception, depending on your preference
            }
        }

        public override string ToString()
        {
            return X.ToString() + ";" + Y.ToString() + ";" + Z.ToString();
        }

        public vec3 ToGlmVec3()
        {
            return new vec3(X, Y, Z);
        }

    }
}
