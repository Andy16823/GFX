using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Genesis.Core
{
    public class Input
    {
        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey); // Keys enumeration
        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(System.Int32 vKey);

        public static bool IsKeyDown(System.Windows.Forms.Keys vKey)
        {
            short key = GetAsyncKeyState(vKey);
            return Convert.ToBoolean(key);
        }

        public static Vec3 GetMousePos()
        {
            Point point = Cursor.Position;
            return new Vec3(point.X, point.Y);
        }

        public static Vec3 GetRefMousePos(Control control)
        {
            Point point = Cursor.Position;
            try
            {
                if(control != null)
                {
                    control.Invoke(new Action(() => { point = control.PointToClient(point); }));
                }
            }
            catch
            {

            }
            return new Vec3(point.X, point.Y);
        }

        public static Vec3 GetRefMousePos(IntPtr handle)
        {
            return Input.GetRefMousePos(Control.FromHandle(handle));
        }

        public static Vec3 GetRefMousePos(Game game)
        {
            return Input.GetRefMousePos(game.RenderDevice.GetHandle());
        }

    }
}
