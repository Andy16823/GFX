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
    /// Represents a three-dimensional vector, used for representing points or directions in 3D space.
    /// </summary>
    public struct Vec3
    {
        /// <summary>
        /// Gets or sets the X-coordinate of the vector.
        /// </summary>
        public float X;

        /// <summary>
        /// Gets or sets the Y-coordinate of the vector.
        /// </summary>
        public float Y;

        /// <summary>
        /// Gets or sets the Z-coordinate of the vector.
        /// </summary>
        public float Z;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vec3"/> struct with the specified coordinates.
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
        /// Initializes a new instance of the <see cref="Vec3"/> struct with the specified X and Y coordinates, and Z set to 0.
        /// </summary>
        /// <param name="x">The X-coordinate of the vector.</param>
        /// <param name="y">The Y-coordinate of the vector.</param>
        public Vec3(float x, float y)
        {
            X = x;
            Y = y;
            Z = 0.0f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vec3"/> struct with all coordinates set to the same value.
        /// </summary>
        /// <param name="value">The value to assign to all coordinates of the vector.</param>
        public Vec3(float value)
        {
            X = value;
            Y = value;
            Z = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vec3"/> struct from a <see cref="Size"/> object, setting Z to 0.
        /// </summary>
        /// <param name="size">The <see cref="Size"/> object to extract coordinates from.</param>
        public Vec3(Size size)
        {
            X = size.Width;
            Y = size.Height;
            Z = 0f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vec3"/> struct from a <see cref="SizeF"/> object, setting Z to 0.
        /// </summary>
        /// <param name="size">The <see cref="SizeF"/> object to extract coordinates from.</param>>
        public Vec3(SizeF size) 
        {
            X = size.Width;
            Y = size.Height;
            Z = 0f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vec3"/> struct from a <see cref="Point"/> object, setting Z to 0.
        /// </summary>
        /// <param name="point">The <see cref="Point"/> object to extract coordinates from.</param>
        public Vec3(Point point)
        {
            X = point.X;
            Y = point.Y;
            Z = 0f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vec3"/> struct from a <see cref="PointF"/> object, setting Z to 0.
        /// </summary>
        /// <param name="point">The <see cref="PointF"/> object to extract coordinates from.</param>
        public Vec3(PointF point)
        {
            X = point.X;
            Y = point.Y;
            Z = 0f;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vec3"/> struct from a <see cref="glm.vec3"/> object.
        /// </summary>
        /// <param name="vec3">The <see cref="glm.vec3"/> object to extract coordinates from.</param>
        public Vec3(vec3 vec3)
        {
            X = vec3.x;
            Y = vec3.y;
            Z = vec3.z;
        }

        /// <summary>
        /// Returns a new vector representing the origin (0, 0, 0).
        /// </summary>
        /// <returns>A <see cref="Vec3"/> instance representing the zero vector.</returns>
        public static Vec3 Zero()
        {
            return new Vec3(0.0f);
        }

        /// <summary>
        /// Calculates the angle in degrees from this vector to the specified coordinates.
        /// </summary>
        /// <param name="x">The X-coordinate of the target point.</param>
        /// <param name="y">The Y-coordinate of the target point.</param>
        /// <returns>The angle in degrees from this vector to the specified point.</returns>
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
        /// Calculates the angle in degrees from this vector to the specified vector.
        /// </summary>
        /// <param name="vec3">The reference vector.</param>
        /// <returns>The angle in degrees from this vector to the specified vector.</returns>
        public float Degres(Vec3 vec3)
        {
            return this.Degres(vec3.X, vec3.Y);
        }

        /// <summary>
        /// Calculates the point in 3D space towards a specified angle and distance from this vector.
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <param name="dist">The distance from this vector.</param>
        /// <returns>A new <see cref="Vec3"/> instance representing the calculated point in 3D space.</returns>
        public Vec3 Towards(float degrees, float dist)
        {
            float radians = (float)((System.Math.PI / 180) * degrees);
            float x = (float)(dist * System.Math.Cos(radians));
            float y = (float)(dist * System.Math.Sin(radians));
            return new Vec3(x, y);
        }

        /// <summary>
        /// Returns a new vector in 2D space towards a specified angle from the given starting vector.
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <param name="start">The starting vector.</param>
        /// <param name="dist">The distance from the starting vector.</param>
        /// <returns>A new <see cref="Vec3"/> instance representing the target vector in 2D space.</returns>
        public static Vec3 Towards2D(float degrees, Vec3 start, float dist)
        {
            Vec3 twd = start.Towards(degrees, dist);
            return new Vec3(start.X + twd.X, start.Y + twd.Y);
        }

        /// <summary>
        /// Calculates a point in 3D space towards the specified rotation from a starting vector.
        /// </summary>
        /// <param name="rotation">The rotation vector representing angles.</param>
        /// <param name="start">The starting vector.</param>
        /// <param name="dist">The distance from the starting vector.</param>
        /// <returns>A new <see cref="Vec3"/> instance representing the calculated point in 3D space.</returns>
        public static Vec3 Towards3D(Vec3 rotation, Vec3 start, float dist)
        {
            Vec3 direction = new Vec3(0f);
            direction.X = (float)(System.Math.Cos(Utils.ToRadians(rotation.Y)) * System.Math.Cos(Utils.ToRadians(rotation.X)));
            direction.Y = (float)System.Math.Sin(Utils.ToRadians(rotation.X));
            direction.Z = (float)(System.Math.Sin(Utils.ToRadians(rotation.Y)) * System.Math.Cos(Utils.ToRadians(rotation.X)));

            return start + direction * dist;
        }

        /// <summary>
        /// Calculates a point in 3D space towards the specified rotation angles from a starting vector.
        /// </summary>
        /// <param name="rotX">The rotation angle around the X-axis.</param>
        /// <param name="rotY">The rotation angle around the Y-axis.</param>
        /// <param name="rotZ">The rotation angle around the Z-axis.</param>
        /// <param name="start">The starting vector.</param>
        /// <param name="dist">The distance from the starting vector.</param>
        /// <returns>A new <see cref="Vec3"/> instance representing the calculated point in 3D space.</returns>
        public static Vec3 Towards3D(float rotX, float rotY, float rotZ, Vec3 start, float dist)
        {
            return Vec3.Towards3D(new Vec3(rotX, rotY, rotZ), start, dist);
        }

        /// <summary>
        /// Calculates the distance between this vector and another vector.
        /// </summary>
        /// <param name="vec3">The vector to measure distance to.</param>
        /// <returns>The distance between this vector and the specified vector.</returns>
        public float Distance(Vec3 vec3)
        {
            float xDist = X - vec3.X;
            float yDist = Y - vec3.Y;

            return (float)System.Math.Sqrt(xDist* xDist + yDist * yDist);
        }

        /// <summary>
        /// Adds the value from another vector.
        /// </summary>
        /// <param name="vec">The vector to add.</param>
        /// <returns>A new Vec3 that is the sum of this vector and the specified vector.</returns>
        public Vec3 Add(Vec3 vec)
        {
            return this + vec;
        }

        /// <summary>
        /// Adds the specified X and Y values to this vector.
        /// </summary>
        /// <param name="x">The value to add to the X component.</param>
        /// <param name="y">The value to add to the Y component.</param>
        /// <returns>A new Vec3 with the added values.</returns>
        public Vec3 Add(float x, float y)
        {
            return this + new Vec3(x, y);
        }

        /// <summary>
        /// Adds the specified X, Y, and Z values to this vector.
        /// </summary>
        /// <param name="x">The value to add to the X component.</param>
        /// <param name="y">The value to add to the Y component.</param>
        /// <param name="z">The value to add to the Z component.</param>
        /// <returns>A new Vec3 with the added values.</returns>
        public Vec3 Add(float x, float y, float z)
        {
            return this + new Vec3(x, y, z);
        }

        /// <summary>
        /// Adds the specified value to the X component of this vector.
        /// </summary>
        /// <param name="x">The value to add to the X component.</param>
        /// <returns>A new Vec3 with the updated X component.</returns>
        public Vec3 AddX(float x)
        {
            return this + new Vec3(x, 0, 0);
        }

        /// <summary>
        /// Adds the specified value to the Y component of this vector.
        /// </summary>
        /// <param name="y">The value to add to the Y component.</param>
        /// <returns>A new Vec3 with the updated Y component.</returns>
        public Vec3 AddY(float y)
        {
            return this + new Vec3(0, y, 0);
        }

        /// <summary>
        /// Adds the specified value to the Z component of this vector.
        /// </summary>
        /// <param name="z">The value to add to the Z component.</param>
        /// <returns>A new Vec3 with the updated Z component.</returns>
        public Vec3 AddZ(float z)
        {
            return this + new Vec3(0, 0, z);
        }

        /// <summary>
        /// Subtracts the specified vector from this vector.
        /// </summary>
        /// <param name="v">The vector to subtract.</param>
        /// <returns>A new Vec3 that is the result of the subtraction.</returns>
        public Vec3 Sub(Vec3 v)
        {
            return this - v;
        }

        /// <summary>
        /// Subtracts the specified value from the X component of this vector.
        /// </summary>
        /// <param name="x">The value to subtract from the X component.</param>
        /// <returns>A new Vec3 with the updated X component.</returns>
        public Vec3 SubX(float x)
        {
            return this - new Vec3(x, 0, 0);
        }

        /// <summary>
        /// Subtracts the specified value from the Y component of this vector.
        /// </summary>
        /// <param name="y">The value to subtract from the Y component.</param>
        /// <returns>A new Vec3 with the updated Y component.</returns>
        public Vec3 SubY(float y)
        {
            return this - new Vec3(0, y, 0);
        }

        /// <summary>
        /// Subtracts the specified value from the Z component of this vector.
        /// </summary>
        /// <param name="z">The value to subtract from the Z component.</param>
        /// <returns>A new Vec3 with the updated Z component.</returns>
        public Vec3 SubZ(float z)
        {
            return this - new Vec3(0, 0, z);
        }

        /// <summary>
        /// Sets the X component of the vector to the specified value.
        /// </summary>
        /// <param name="x">The value to set the X component to.</param>
        /// <returns>A new Vec3 with the updated X component.</returns>
        public Vec3 SetX(float x)
        {
            return new Vec3(x, Y, Z);
        }

        /// <summary>
        /// Sets the Y component of the vector to the specified value.
        /// </summary>
        /// <param name="y">The value to set the Y component to.</param>
        /// <returns>A new Vec3 with the updated Y component.</returns>
        public Vec3 SetY(float y)
        {
            return new Vec3(X, y, Z);
        }

        /// <summary>
        /// Sets the Z component of the vector to the specified value.
        /// </summary>
        /// <param name="z">The value to set the Z component to.</param>
        /// <returns>A new Vec3 with the updated Z component.</returns>
        public Vec3 SetZ(float z)
        {
            return new Vec3(X, Y, z);
        }

        /// <summary>
        /// Sets the values for the vector.
        /// </summary>
        /// <param name="x">The value to set the X component to.</param>
        /// <param name="y">The value to set the Y component to.</param>
        /// <param name="z">The value to set the Z component to.</param>
        /// <returns>A new Vec3 with the specified values.</returns>
        public Vec3 Set(float x, float y, float z)
        {
            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Sets the values for the vector based on another vector.
        /// </summary>
        /// <param name="vec">The vector to copy values from.</param>
        /// <returns>A new Vec3 with values copied from the specified vector.</returns>
        public Vec3 Set(Vec3 vec)
        {
            return new Vec3(vec.X, vec.Y, vec.Z);
        }

        /// <summary>
        /// Returns the forward vector based on the specified rotation and distance.
        /// </summary>
        /// <param name="rotation">The rotation to apply.</param>
        /// <param name="dist">The distance to project forward.</param>
        /// <returns>A new Vec3 representing the forward vector.</returns>
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
        /// Multiply operator for vector multiplication with another vector.
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
        /// Divide operator for vector division by another vector.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector to divide by.</param>
        /// <returns>A new Vec3 instance representing the result of vector division.</returns>
        public static Vec3 operator /(Vec3 vec1, Vec3 vec2)
        {
            float x = vec1.X / vec2.X;
            float y = vec1.Y / vec2.Y;
            float z = vec1.Z / vec2.Z;
            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Multiplies the vector by a scalar.
        /// </summary>
        /// <param name="vec">The vector to multiply.</param>
        /// <param name="value">The scalar to multiply by.</param>
        /// <returns>A new Vec3 instance representing the scaled vector.</returns>
        public static Vec3 operator *(Vec3 vec, float value)
        {
            float x = vec.X * value;
            float y = vec.Y * value;
            float z = vec.Z * value;
            return new Vec3(x, y, z);
        }

        /// <summary>
        /// Subtract operator for vector subtraction.
        /// </summary>
        /// <param name="vec1">The first vector.</param>
        /// <param name="vec2">The second vector to subtract.</param>
        /// <returns>A new Vec3 instance representing the result of vector subtraction.</returns>
        public static Vec3 operator -(Vec3 vec1, Vec3 vec2)
        {
            float x = vec1.X - vec2.X;
            float y = vec1.Y - vec2.Y;
            float z = vec1.Z - vec2.Z;
            return new Vec3(x, y, z);
        }

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
        /// Returns the vector length
        /// </summary>
        /// <returns></returns>
        public float Length()
        {
            return (float)System.Math.Sqrt(X * X + Y * Y + Z * Z);
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
        /// Returns a new <see cref="Vec3"/> instance with each component halved.
        /// </summary>
        /// <param name="vec">The <see cref="Vec3"/> instance to halve.</param>
        /// <returns>A new <see cref="Vec3"/> instance with each component halved.</returns>
        public static Vec3 Half(Vec3 vec)
        {
            Vec3 newVec = new Vec3(vec.X / 2, vec.Y / 2, vec.Z / 2);
            return newVec;
        }

        /// <summary>
        /// Returns a new <see cref="Vec3"/> instance with the components of this instance halved.
        /// </summary>
        /// <returns>A new <see cref="Vec3"/> instance with the components halved.</returns>
        public Vec3 Half()
        {
            return new Vec3(X / 2, Y / 2, Z / 2);
        }

        /// <summary>
        /// Generates an rendom vector from min an max value with the given seed
        /// </summary>
        /// <param name="min">The min vector</param>
        /// <param name="max">The max vector</param>
        /// <param name="seed">The seed</param>
        /// <returns></returns>
        public static Vec3 Random(Vec3 min, Vec3 max, int seed)
        {
            // Min- und Max-Werte für jede Komponente
            float minX = min.X;
            float minY = min.Y;
            float minZ = min.Z;

            float maxX = max.X;
            float maxY = max.Y;
            float maxZ = max.Z;

            // Zufallsgenerator initialisieren
            Random random = new Random(seed);

            // Zufällige Werte für jede Komponente generieren
            float randomX = (float)(random.NextDouble() * (maxX - minX) + minX);
            float randomY = (float)(random.NextDouble() * (maxY - minY) + minY);
            float randomZ = (float)(random.NextDouble() * (maxZ - minZ) + minZ);

            return new Vec3(randomX, randomY, randomZ);
        }

        /// <summary>
        /// Generates an rendom vector from min an max value
        /// </summary>
        /// <param name="min">The min vector</param>
        /// <param name="max">The max vector</param>
        /// <returns></returns>
        public static Vec3 Random(Vec3 min, Vec3 max)
        {
            // Min- und Max-Werte für jede Komponente
            float minX = min.X;
            float minY = min.Y;
            float minZ = min.Z;

            float maxX = max.X;
            float maxY = max.Y;
            float maxZ = max.Z;

            // Zufallsgenerator initialisieren
            Random random = new Random();

            // Zufällige Werte für jede Komponente generieren
            float randomX = (float)(random.NextDouble() * (maxX - minX) + minX);
            float randomY = (float)(random.NextDouble() * (maxY - minY) + minY);
            float randomZ = (float)(random.NextDouble() * (maxZ - minZ) + minZ);

            return new Vec3(randomX, randomY, randomZ);
        }

        public static Vec3 NormalizeAngles(float vX, float vY, float vZ)
        {
            float x = Utils.NormalizeAngle(vX);
            float y = Utils.NormalizeAngle(vY);
            float z = Utils.NormalizeAngle(vZ);
            return new Vec3(x, y, z);
        }

        public static Vec3 NormalizeAngles(Vec3 value)
        {
            float x = Utils.NormalizeAngle(value.X);
            float y = Utils.NormalizeAngle(value.Y);
            float z = Utils.NormalizeAngle(value.Z);
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
