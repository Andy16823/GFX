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
    /// <summary>
    /// Provides utility methods for handling user input in the Genesis framework.
    /// </summary>
    public class Input
    {
        /// <summary>
        /// Gets the state of the specified key.
        /// </summary>
        /// <param name="vKey">The virtual key code to check.</param>
        /// <returns>True if the key is down; otherwise, false.</returns>
        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey); // Keys enumeration

        /// <summary>
        /// Gets the state of the specified key.
        /// </summary>
        /// <param name="vKey">The virtual key code to check.</param>
        /// <returns>True if the key is down; otherwise, false.</returns>
        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(System.Int32 vKey);

        /// <summary>
        /// Checks if the specified key is currently pressed.
        /// </summary>
        /// <param name="vKey">The virtual key to check.</param>
        /// <returns>True if the key is down; otherwise, false.</returns>
        public static bool IsKeyDown(System.Windows.Forms.Keys vKey)
        {
            short key = GetAsyncKeyState(vKey);
            return Convert.ToBoolean(key);
        }

        /// <summary>
        /// Gets the current mouse position.
        /// </summary>
        /// <returns>A Vec3 representing the current mouse position.</returns>
        public static Vec3 GetMousePos()
        {
            Point point = Cursor.Position;
            return new Vec3(point.X, point.Y);
        }

        /// <summary>
        /// Gets the mouse position relative to the specified control.
        /// </summary>
        /// <param name="control">The control relative to which the mouse position is obtained.</param>
        /// <returns>A Vec3 representing the mouse position relative to the control.</returns>
        public static Vec3 GetRefMousePos(Control control)
        {
            Point point = Cursor.Position;
            try
            {
                if(control != null)
                {
                    control.Invoke(new Action(() => { point = control.PointToClient(point); }));
                    float x = point.X;
                    float y = (float)control.ClientSize.Height - point.Y;
                    return new Vec3(x, y);
                }
            }
            catch
            {

            }
            //Anpassen an opengl coord system
            //float x = point.X;
            //float y = (float) control.ClientSize.Height - point.Y;
            return new Vec3(point);
        }

        /// <summary>
        /// Gets the mouse position relative to the control associated with the specified handle.
        /// </summary>
        /// <param name="handle">The handle of the control.</param>
        /// <returns>A Vec3 representing the mouse position relative to the control.</returns>
        public static Vec3 GetRefMousePos(IntPtr handle)
        {
            return Input.GetRefMousePos(Control.FromHandle(handle));
        }

        /// <summary>
        /// Gets the mouse position relative to the control associated with the specified game instance.
        /// </summary>
        /// <param name="game">The game instance.</param>
        /// <returns>A Vec3 representing the mouse position relative to the control.</returns>
        public static Vec3 GetRefMousePos(Game game)
        {
            return Input.GetRefMousePos(game.RenderDevice.GetHandle());
        }

    }
}
