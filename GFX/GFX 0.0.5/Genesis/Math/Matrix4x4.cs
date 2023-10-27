using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Math
{
    using System;

    using System;

    public class Matrix4x4
    {
        private float[,] matrix;

        public Matrix4x4()
        {
            matrix = new float[4, 4];
        }

        public float this[int row, int col]
        {
            get { return matrix[row, col]; }
            set { matrix[row, col] = value; }
        }

        public void Normalize()
        {
            float length = (float)Math.Sqrt(
                matrix[0, 0] * matrix[0, 0] +
                matrix[1, 0] * matrix[1, 0] +
                matrix[2, 0] * matrix[2, 0] +
                matrix[3, 0] * matrix[3, 0]);

            for (int i = 0; i < 4; i++)
            {
                matrix[i, 0] /= length;
            }
        }

        public static Matrix4x4 Ortho(float left, float right, float bottom, float top, float near, float far)
        {
            Matrix4x4 result = new Matrix4x4();

            result[0, 0] = 2.0f / (right - left);
            result[1, 1] = 2.0f / (top - bottom);
            result[2, 2] = -2.0f / (far - near);
            result[0, 3] = -(right + left) / (right - left);
            result[1, 3] = -(top + bottom) / (top - bottom);
            result[2, 3] = -(far + near) / (far - near);
            result[3, 3] = 1.0f;

            return result;
        }

        public static Matrix4x4 Identity()
        {
            Matrix4x4 result = new Matrix4x4();
            for (int i = 0; i < 4; i++)
                result[i, i] = 1.0f;
            return result;
        }

        public Matrix4x4 Multiply(Matrix4x4 right)
        {
            Matrix4x4 result = new Matrix4x4();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    for (int k = 0; k < 4; k++)
                    {
                        result[i, j] += matrix[i, k] * right[k, j];
                    }
                }
            }
            return result;
        }

        public Vec3 Multiply(Vec3 vector)
        {
            float x = matrix[0, 0] * vector.X + matrix[0, 1] * vector.Y + matrix[0, 2] * vector.Z + matrix[0, 3];
            float y = matrix[1, 0] * vector.X + matrix[1, 1] * vector.Y + matrix[1, 2] * vector.Z + matrix[1, 3];
            float z = matrix[2, 0] * vector.X + matrix[2, 1] * vector.Y + matrix[2, 2] * vector.Z + matrix[2, 3];
            return new Vec3(x, y, z);
        }

        public float[] ToArray()
        {
            float[] array = new float[16];
            int index = 0;
            for (int col = 0; col < 4; col++)
            {
                for (int row = 0; row < 4; row++)
                {
                    array[index++] = matrix[row, col];
                }
            }
            return array;
        }

        public static Matrix4x4 LookAt(Vec3 eye, Vec3 target, Vec3 up)
        {
            Vec3 forward = (target - eye).Normalize();
            Vec3 right = Vec3.Cross(up, forward).Normalize();
            Vec3 newUp = Vec3.Cross(forward, right);

            Matrix4x4 result = new Matrix4x4();

            result[0, 0] = right.X;
            result[0, 1] = right.Y;
            result[0, 2] = right.Z;

            result[1, 0] = newUp.X;
            result[1, 1] = newUp.Y;
            result[1, 2] = newUp.Z;

            result[2, 0] = -forward.X;
            result[2, 1] = -forward.Y;
            result[2, 2] = -forward.Z;

            result[3, 0] = -Vec3.Dot(right, eye);
            result[3, 1] = -Vec3.Dot(newUp, eye);
            result[3, 2] = Vec3.Dot(forward, eye);
            result[3, 3] = 1.0f;

            return result;
        }

        public static Matrix4x4 Translate(Vec3 translation)
        {
            Matrix4x4 result = Identity();
            result[0, 3] = -translation.X;
            result[1, 3] = translation.Y;
            result[2, 3] = translation.Z;
            return result;
        }

        public static Matrix4x4 RotateX(float angleInRadians)
        {
            Matrix4x4 result = Identity();
            float cosTheta = (float)Math.Cos(angleInRadians);
            float sinTheta = (float)Math.Sin(angleInRadians);

            result[1, 1] = cosTheta;
            result[1, 2] = -sinTheta;
            result[2, 1] = sinTheta;
            result[2, 2] = cosTheta;

            return result;
        }

        public static Matrix4x4 RotateY(float angleInRadians)
        {
            Matrix4x4 result = Identity();
            float cosTheta = (float)Math.Cos(angleInRadians);
            float sinTheta = (float)Math.Sin(angleInRadians);

            result[0, 0] = cosTheta;
            result[0, 2] = sinTheta;
            result[2, 0] = -sinTheta;
            result[2, 2] = cosTheta;

            return result;
        }

        public static Matrix4x4 RotateZ(float angleInRadians)
        {
            Matrix4x4 result = Identity();
            float cosTheta = (float)Math.Cos(angleInRadians);
            float sinTheta = (float)Math.Sin(angleInRadians);

            result[0, 0] = cosTheta;
            result[0, 1] = -sinTheta;
            result[1, 0] = sinTheta;
            result[1, 1] = cosTheta;

            return result;
        }

        public static Matrix4x4 Scale(Vec3 scale)
        {
            Matrix4x4 result = Identity();
            result[0, 0] = scale.X;
            result[1, 1] = scale.Y;
            result[2, 2] = scale.Z;
            return result;
        }

        public static Matrix4x4 operator *(Matrix4x4 left, Matrix4x4 right)
        {
            Matrix4x4 result = left.Multiply(right);
            return result;
        }
    }
}
