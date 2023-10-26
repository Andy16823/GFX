using Genesis.Graphics;
using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Genesis.Core
{
    public class Utils
    {
        public static long GetCurrentTimeMillis()
        {
            return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
        }

        public static float GetStringWidht(String text, float fontSize, float spacing)
        {
            int chars = text.Length;
            float baseWidth = chars * fontSize;
            float spaceWidth = (float)(fontSize * spacing);
            float spacingWidth = spaceWidth * (chars - 1);
            return baseWidth - spacingWidth;
        }

        public static Rect GetStringBounds(Vec3 location, String text, float fontSize, float spacing)
        {
            Rect rect = new Rect();
            rect.X = location.X;
            rect.Y = location.Y;
            rect.Width = GetStringWidht(text, fontSize, spacing);
            rect.Height = fontSize;
            return rect;
        }

        public static Vec3 GetVMirroredPosition(Vec3 vref, Camera camera)
        {
            return GetVMirroredPosition(vref.X, vref.Y, vref.Z, camera);
        }

        public static Vec3 GetVMirroredPosition(float x, float y, float z, Camera camera)
        {
            float diff = camera.Location.Y - y;

            float newX = x;
            float newY = diff + camera.Location.Y;
            float newZ = z;

            Console.WriteLine("Diff " + diff + " Cam " + camera.Location.ToString() + " new y " + newY);

            return new Vec3(newX, newY, newZ);
        }

        public static float ToRadians(float degrees)
        {
            return (float)(System.Math.PI * degrees / 180.0);
        }
    }
}
