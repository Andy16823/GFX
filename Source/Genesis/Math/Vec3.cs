using Genesis.Core;
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
    /// Represents a 3D vector for coordinates.
    /// </summary>
    public class Vec3
    {
        /// <summary>
        /// Gets or sets the X-coordinate of the vector.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Gets or sets the Y-coordinate of the vector.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Gets or sets the Z-coordinate of the vector.
        /// </summary>
        public float Z { get; set; }

        /// <summary>
        /// Creates a new 3D vector with specified coordinates.
        /// </summary>
        /// <param name="x">The X-coordinate of the vector.</param>
        /// <param name="y">The Y-coordinate of the vector.</param>
        /// <param name="z">The Z-coordinate of the vector.</param>
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

        /// <summary>
        /// Creates a new 3D vector with specified coordinates from a Size object.
        /// </summary>
        /// <param name="size">The Size object to extract coordinates from.</param>
        public Vec3(Size size)
        {
            X = size.Width;
            Y = size.Height;
            Z = 0f;
        }

        /// <summary>
        /// Creates a new 3D vector with specified coordinates from a SizeF object.
        /// </summary>
        /// <param name="size">The SizeF object to extract coordinates from.</param>
        public Vec3(SizeF size) 
        {
            X = size.Width;
            Y = size.Height;
            Z = 0f;
        }

        /// <summary>
        /// Creates a new 3D vector with specified coordinates from a Point object.
        /// </summary>
        /// <param name="size">The Point object to extract coordinates from.</param>
        public Vec3(Point point)
        {
            X = point.X;
            Y = point.Y;
            Z = 0f;
        }

        /// <summary>
        /// Creates a new 3D vector with specified coordinates from a PointF object.
        /// </summary>
        /// <param name="size">The PointF object to extract coordinates from.</param>
        public Vec3(PointF point)
        {
            X = point.X;
            Y = point.Y;
            Z = 0f;
        }

        /// <summary>
        /// Creates a new 3D vector with specified coordinates from a glm vec3 object.
        /// </summary>
        /// <param name="size">The glm vec3 object to extract coordinates from.</param>
        public Vec3(vec3 vec3)
        {
            X = vec3.x;
            Y = vec3.y;
            Z = vec3.z;
        }

        /// <summary>
        /// Returns a new 3D vector with X = 0, Y = 0, Z = 0.
        /// </summary>
        /// <returns>A Vec3 instance representing the zero vector.</returns>
        public static Vec3 Zero()
        {
            return new Vec3(0.0f);
        }

        /// <summary>
        /// Returns the offset angle to the vector.
        /// </summary>
        /// <param name="x">The X-coordinate of the vector.</param>
        /// <param name="y">The Y-coordinate of the vector.</param>
        /// <returns>The offset angle to the vector in degrees.</returns>
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
        /// Returns the offset angle to the vector.
        /// </summary>
        /// <param name="vec3">The reference vector.</param>
        /// <returns>The offset angle to the vector in degrees.</returns>
        public float Degres(Vec3 vec3)
        {
            return this.Degres(vec3.X, vec3.Y);
        }

        /// <summary>
        /// Calculates the vector towards in 3D.
        /// </summary>
        /// <param name="rotation">The rotation vector.</param>
        /// <param name="start">The starting vector.</param>
        /// <param name="dist">The distance.</param>
        /// <returns>A new Vec3 instance representing the vector towards in 3D.</returns>
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
        /// Calculates the Vector towards in 3D
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="start"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        public static Vec3 Towards3D(Vec3 rotation, Vec3 start, float dist)
        {
            Vec3 direction = new Vec3(0f);
            direction.X = (float)(System.Math.Cos(Utils.ToRadians(rotation.Y)) * System.Math.Cos(Utils.ToRadians(rotation.X)));
            direction.Y = (float)System.Math.Sin(Utils.ToRadians(rotation.X));
            direction.Z = (float)(System.Math.Sin(Utils.ToRadians(rotation.Y)) * System.Math.Cos(Utils.ToRadians(rotation.X)));

            return start + direction * dist;
        }

        /// <summary>
        /// Calculates the towards vector
        /// </summary>
        /// <param name="rotX"></param>
        /// <param name="rotY"></param>
        /// <param name="rotZ"></param>
        /// <param name="start"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        public static Vec3 Towards3D(float rotX, float rotY, float rotZ, Vec3 start, float dist)
        {
            return Vec3.Towards3D(new Vec3(rotX, rotY, rotZ), start, dist);
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
            Z += vec.Z;
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
        /// Adds the values to the vector
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Add(float x, float y, float z)
        {
            X += x;
            Y += y;
            Z += z;
        }

        /// <summary>
        /// Add the value to the X param
        /// </summary>
        /// <param name="x"></param>
        public void AddX(float x)
        {
            X += x;
        }

        /// <summary>
        /// Add the value to the y param
        /// </summary>
        /// <param name="y"></param>
        public void AddY(float y)
        {
            Y += y;
        }

        /// <summary>
        /// Add the value to the z param
        /// </summary>
        /// <param name="z"></param>
        public void AddZ(float z)
        {
            Z += z;
        }

        /// <summary>
        /// Subtract the vector
        /// </summary>
        /// <param name="v"></param>
        public void Sub(Vec3 v)
        {
            X -= v.X;
            Y -= v.Y;
            Z -= v.Z;
        }

        /// <summary>
        /// Sets the value for the vector
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public void Set(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Sets the value for the vector
        /// </summary>
        /// <param name="vec"></param>
        public void Set(Vec3 vec)
        {
            X = vec.X;
            Y = vec.Y;
            Z = vec.Z;
        }

        /// <summary>
        /// Returns the forward vector
        /// </summary>
        /// <param name="rotation"></param>
        /// <param name="dist"></param>
        /// <returns></returns>
        public Vec3 Forward(Vec3 rotation, float dist)
        {
            return Utils.ForwardVector(this, rotation, dist);
        }

        /// <summary>
        /// Add operator for vector addition.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector.</param>
        /// <returns>A new Vec3 instance representing the result of vector addition.</returns>
        public static Vec3 operator +(Vec3 vec1, Vec3 vec2)
        {
            float x = vec1.X + vec2.X;
            float y = vec1.Y + vec2.Y;
            float z = vec1.Z + vec2.Z;
            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Add operator for vector multiplication.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector.</param>
        /// <returns>A new Vec3 instance representing the result of vector multiplication.</returns>
        public static Vec3 operator *(Vec3 vec1, Vec3 vec2)
        {
            float x = vec1.X * vec2.X;
            float y = vec1.Y * vec2.Y;
            float z = vec1.Z * vec2.Z;
            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Add operator for vector substraction.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector.</param>
        /// <returns>A new Vec3 instance representing the result of vector substraction.</returns>
        public static Vec3 operator /(Vec3 vec1, Vec3 vec2)
        {
            float x = vec1.X / vec2.X;
            float y = vec1.Y / vec2.Y;
            float z = vec1.Z / vec2.Z;
            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Multiplies the vector by a scalar value.
        /// </summary>
        /// <param name="vec">The vector to multiply.</param>
        /// <param name="value">The scalar value.</param>
        /// <returns>A new Vec3 instance representing the result of the multiplication.</returns>
        public static Vec3 operator *(Vec3 vec, float value)
        {
            float x = vec.X * value;
            float y = vec.Y * value;
            float z = vec.Z * value;
            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Subtracts one vector from another.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector to subtract.</param>
        /// <returns>A new Vec3 instance representing the result of the subtraction.</returns>
        public static Vec3 operator -(Vec3 vec1, Vec3 vec2)
        {
            float x = vec1.X - vec2.X;
            float y = vec1.Y - vec2.Y;
            float z = vec1.Z - vec2.Z;
            return new Vec3(x, y, z);
        }

        //public static implicit operator Vec3(Vec3 v)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Calculates the cross product of two vectors.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector.</param>
        /// <returns>A new Vec3 instance representing the cross product of the two vectors.</returns>
        public static Vec3 Cross(Vec3 vec1, Vec3 vec2)
        {
            float x = vec1.Y * vec2.Z - vec1.Z * vec2.Y;
            float y = vec1.Z * vec2.X - vec1.X * vec2.Z;
            float z = vec1.X * vec2.Y - vec1.Y * vec2.X;
            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Calculates the dot product of two vectors.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector.</param>
        /// <returns>The dot product of the two vectors.</returns>
        public static float Dot(Vec3 vec1, Vec3 vec2)
        {
            return vec1.X * vec2.X + vec1.Y * vec2.Y + vec1.Z * vec2.Z;
        }

        /// <summary>
        /// Normalizes the vector.
        /// </summary>
        /// <returns>A normalized Vec3 instance.</returns>
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

        /// <summary>
        /// Returns a normalized version of the input vector.
        /// </summary>
        /// <param name="vec">The vector to normalize.</param>
        /// <returns>A normalized Vec3 instance.</returns>
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

        /// <summary>
        /// Returns a new Vec3 object where each component is the smallest integer greater than or equal to the corresponding component of the input Vec3.
        /// </summary>
        /// <param name="vec">The input Vec3 to be processed.</param>
        /// <returns>A new Vec3 with each component rounded up to the nearest integer.</returns>
        public static Vec3 Ceiling(Vec3 vec)
        {
            float x = (float)System.Math.Ceiling(vec.X);
            float y = (float)System.Math.Ceiling(vec.Y);
            float z = (float)System.Math.Ceiling(vec.Y);
            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Returns a new Vec3 object where each component is rounded down to the nearest integer
        /// </summary>
        /// <param name="vec">The input Vec3 to be processed.</param>
        /// <returns>A new Vec3 with each component rounded down to the nearest integer.</returns>
        public static Vec3 Floor(Vec3 vec)
        {
            float x = (float)System.Math.Floor(vec.X);
            float y = (float)System.Math.Floor(vec.Y);
            float z = (float)System.Math.Floor(vec.Z);
            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Returns a new Vec3 object where each component is rounded to the nearest integer
        /// </summary>
        /// <param name="vec">The input Vec3 to be processed.</param>
        /// <returns>A new Vec3 with each component rounded to the nearest integer.</returns>
        public static Vec3 Round(Vec3 vec)
        {
            float x = (float)System.Math.Round(vec.X);
            float y = (float)System.Math.Round(vec.Y);
            float z = (float)System.Math.Round(vec.Z);

            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Returns a string representation of the vector.
        /// </summary>
        /// <returns>A string containing the X, Y, and Z coordinates of the vector.</returns>
        public override string ToString()
        {
            return X.ToString() + ";" + Y.ToString() + ";" + Z.ToString();
        }

        /// <summary>
        /// Converts the vector to its GLM equivalent.
        /// </summary>
        /// <returns>A GLM vec3 instance representing the same vector.</returns>
        public vec3 ToGlmVec3()
        {
            return new vec3(X, Y, Z);
        }

        /// <summary>
        /// Converts the vector to its BulletSharp equivalent.
        /// </summary>
        /// <returns>A BulletSharp Vector3 instance representing the same vector.</returns>
        public BulletSharp.Math.Vector3 ToBulletVec3()
        {
            return new BulletSharp.Math.Vector3(X, Y, Z);
        }

    }
}
