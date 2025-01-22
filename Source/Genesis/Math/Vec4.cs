using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Math
{
    public struct Vec4
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public Vec4(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        public static Vec4 operator +(Vec4 lhs, Vec4 rhs)
        {
            return new Vec4(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z, lhs.W + rhs.W);
        }

        public static Vec4 operator +(Vec4 lhs, Vec3 rhs)
        {
            return new Vec4(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z, lhs.W);
        }

        public static Vec4 operator +(Vec4 lhs, float rhs)
        {
            return new Vec4(lhs.X + rhs, lhs.Y + rhs, lhs.Z + rhs, lhs.W + rhs);
        }

        public static Vec4 operator -(Vec4 lhs, Vec4 rhs)
        {
            return new Vec4(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z, lhs.W - rhs.W);
        }

        public static Vec4 operator -(Vec4 lhs, Vec3 rhs)
        {
            return new Vec4(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z, lhs.W);
        }

        public static Vec4 operator -(Vec4 lhs, float rhs)
        {
            return new Vec4(lhs.X - rhs, lhs.Y - rhs, lhs.Z - rhs, lhs.W - rhs);
        }

        public static Vec4 operator *(Vec4 lhs, Vec4 rhs)
        {
            return new Vec4(lhs.X * rhs.X, lhs.Y * rhs.Y, lhs.Z * rhs.Z, lhs.W * rhs.W);
        }

        public static Vec4 operator *(Vec4 lhs, Vec3 rhs)
        {
            return new Vec4(lhs.X * rhs.X, lhs.Y * rhs.Y, lhs.Z * rhs.Z, lhs.W);
        }

        public static Vec4 operator *(Vec4 lhs, float rhs)
        {
            return new Vec4(lhs.X * rhs, lhs.Y * rhs, lhs.Z * rhs, lhs.W * rhs);
        }

        public static Vec4 operator /(Vec4 lhs, Vec4 rhs)
        {
            return new Vec4(lhs.X / rhs.X, lhs.Y / rhs.Y, lhs.Z / rhs.Z, lhs.W / rhs.W);
        }

        public static Vec4 operator /(Vec4 lhs, Vec3 rhs)
        {
            return new Vec4(lhs.X / rhs.X, lhs.Y / rhs.Y, lhs.Z / rhs.Z, lhs.W);
        }

        public static Vec4 operator /(Vec4 lhs, float rhs)
        {
            return new Vec4(lhs.X / rhs, lhs.Y / rhs, lhs.Z / rhs, lhs.W / rhs);
        }

        public static bool operator ==(Vec4 lhs, Vec4 rhs)
        {
            return lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z && lhs.W == rhs.W;
        }

        public static bool operator !=(Vec4 lhs, Vec4 rhs)
        {
            return lhs.X != rhs.X || lhs.Y != rhs.Y || lhs.Z != rhs.Z || lhs.W != rhs.W;
        }

        public override bool Equals(object obj)
        {
            if (obj is Vec4)
            {
                return this == (Vec4)obj;
            }
            return false;
        }
    }
}
