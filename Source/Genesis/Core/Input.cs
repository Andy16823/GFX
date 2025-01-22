using Genesis.Math;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public enum Keys
        {
            None = 0,
            LButton = 0x01,     // Left mouse button
            RButton = 0x02,     // Right mouse button
            Cancel = 0x03,      // Control-break processing
            MButton = 0x04,     // Middle mouse button (three-button mouse)
            XButton1 = 0x05,    // X1 mouse button
            XButton2 = 0x06,    // X2 mouse button
            Back = 0x08,        // BACKSPACE key
            Tab = 0x09,         // TAB key
            Clear = 0x0C,       // CLEAR key
            Enter = 0x0D,       // ENTER key
            Shift = 0x10,       // SHIFT key
            Control = 0x11,     // CTRL key
            Alt = 0x12,         // ALT key
            Pause = 0x13,       // PAUSE key
            CapsLock = 0x14,    // CAPS LOCK key
            Escape = 0x1B,      // ESC key
            Space = 0x20,       // SPACEBAR
            PageUp = 0x21,      // PAGE UP key
            PageDown = 0x22,    // PAGE DOWN key
            End = 0x23,         // END key
            Home = 0x24,        // HOME key
            Left = 0x25,        // LEFT ARROW key
            Up = 0x26,          // UP ARROW key
            Right = 0x27,       // RIGHT ARROW key
            Down = 0x28,        // DOWN ARROW key
            Select = 0x29,      // SELECT key
            Print = 0x2A,       // PRINT key
            Execute = 0x2B,     // EXECUTE key
            PrintScreen = 0x2C, // PRINT SCREEN key
            Insert = 0x2D,      // INS key
            Delete = 0x2E,      // DEL key
            Help = 0x2F,        // HELP key
            D0 = 0x30,          // 0 key
            D1 = 0x31,          // 1 key
            D2 = 0x32,          // 2 key
            D3 = 0x33,          // 3 key
            D4 = 0x34,          // 4 key
            D5 = 0x35,          // 5 key
            D6 = 0x36,          // 6 key
            D7 = 0x37,          // 7 key
            D8 = 0x38,          // 8 key
            D9 = 0x39,          // 9 key
            A = 0x41,           // A key
            B = 0x42,           // B key
            C = 0x43,           // C key
            D = 0x44,           // D key
            E = 0x45,           // E key
            F = 0x46,           // F key
            G = 0x47,           // G key
            H = 0x48,           // H key
            I = 0x49,           // I key
            J = 0x4A,           // J key
            K = 0x4B,           // K key
            L = 0x4C,           // L key
            M = 0x4D,           // M key
            N = 0x4E,           // N key
            O = 0x4F,           // O key
            P = 0x50,           // P key
            Q = 0x51,           // Q key
            R = 0x52,           // R key
            S = 0x53,           // S key
            T = 0x54,           // T key
            U = 0x55,           // U key
            V = 0x56,           // V key
            W = 0x57,           // W key
            X = 0x58,           // X key
            Y = 0x59,           // Y key
            Z = 0x5A,           // Z key
            LWin = 0x5B,        // Left Windows key (Microsoft Natural keyboard)
            RWin = 0x5C,        // Right Windows key (Natural keyboard)
            Apps = 0x5D,        // Applications key (Natural keyboard)
            Sleep = 0x5F,       // Computer Sleep key
            NumPad0 = 0x60,     // Numeric keypad 0 key
            NumPad1 = 0x61,     // Numeric keypad 1 key
            NumPad2 = 0x62,     // Numeric keypad 2 key
            NumPad3 = 0x63,     // Numeric keypad 3 key
            NumPad4 = 0x64,     // Numeric keypad 4 key
            NumPad5 = 0x65,     // Numeric keypad 5 key
            NumPad6 = 0x66,     // Numeric keypad 6 key
            NumPad7 = 0x67,     // Numeric keypad 7 key
            NumPad8 = 0x68,     // Numeric keypad 8 key
            NumPad9 = 0x69,     // Numeric keypad 9 key
            Multiply = 0x6A,    // Multiply key (Numeric keypad)
            Add = 0x6B,         // Add key (Numeric keypad)
            Separator = 0x6C,   // Separator key (Numeric keypad)
            Subtract = 0x6D,    // Subtract key (Numeric keypad)
            Decimal = 0x6E,     // Decimal key (Numeric keypad)
            Divide = 0x6F,      // Divide key (Numeric keypad)
            F1 = 0x70,          // F1 key
            F2 = 0x71,          // F2 key
            F3 = 0x72,          // F3 key
            F4 = 0x73,          // F4 key
            F5 = 0x74,          // F5 key
            F6 = 0x75,          // F6 key
            F7 = 0x76,          // F7 key
            F8 = 0x77,          // F8 key
            F9 = 0x78,          // F9 key
            F10 = 0x79,         // F10 key
            F11 = 0x7A,         // F11 key
            F12 = 0x7B,         // F12 key
            F13 = 0x7C,         // F13 key
            F14 = 0x7D,         // F14 key
            F15 = 0x7E,         // F15 key
            F16 = 0x7F,         // F16 key
            F17 = 0x80,         // F17 key
            F18 = 0x81,         // F18 key
            F19 = 0x82,         // F19 key
            F20 = 0x83,         // F20 key
            F21 = 0x84,         // F21 key
            F22 = 0x85,         // F22 key
            F23 = 0x86,         // F23 key
            F24 = 0x87,         // F24 key
            NumLock = 0x90,     // NUM LOCK key
            Scroll = 0x91,      // SCROLL LOCK key
            LShiftKey = 0xA0,   // Left SHIFT key
            RShiftKey = 0xA1,   // Right SHIFT key
            LControlKey = 0xA2, // Left CONTROL key
            RControlKey = 0xA3, // Right CONTROL key
            LMenu = 0xA4,       // Left MENU key
            RMenu = 0xA5,       // Right MENU key
            BrowserBack = 0xA6, // Browser Back key
            BrowserForward = 0xA7, // Browser Forward key
            BrowserRefresh = 0xA8, // Browser Refresh key
            BrowserStop = 0xA9, // Browser Stop key
            BrowserSearch = 0xAA, // Browser Search key
            BrowserFavorites = 0xAB, // Browser Favorites key
            BrowserHome = 0xAC, // Browser Start and Home key
            VolumeMute = 0xAD, // Volume Mute key
            VolumeDown = 0xAE, // Volume Down key
            VolumeUp = 0xAF, // Volume Up key
            MediaNextTrack = 0xB0, // Next Track key
            MediaPreviousTrack = 0xB1, // Previous Track key
            MediaStop = 0xB2, // Stop Media key
            MediaPlayPause = 0xB3, // Play/Pause Media key
            LaunchMail = 0xB4, // Start Mail key
            SelectMedia = 0xB5, // Select Media key
            LaunchApplication1 = 0xB6, // Start Application 1 key
            LaunchApplication2 = 0xB7, // Start Application 2 key
            OemSemicolon = 0xBA, // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ';:' key
            OemPlus = 0xBB, // For any country/region, the '+' key
            OemComma = 0xBC, // For any country/region, the ',' key
            OemMinus = 0xBD, // For any country/region, the '-' key
            OemPeriod = 0xBE, // For any country/region, the '.' key
            OemQuestion = 0xBF, // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '/?' key
            OemTilde = 0xC0, // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '`~' key
            OemOpenBrackets = 0xDB, // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '[{' key
            OemPipe = 0xDC, // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '\|' key
            OemCloseBrackets = 0xDD, // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ']}' key
            OemQuotes = 0xDE, // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the 'single-quote/double-quote' key
            Oem8 = 0xDF, // Used for miscellaneous characters; it can vary by keyboard.
            OemBackslash = 0xE2, // Either the angle bracket key or the backslash key on the RT 102-key keyboard
            ProcessKey = 0xE5, // IME PROCESS key
            Packet = 0xE7, // Used to pass Unicode characters as if they were keystrokes. The PACKET key is the low word of a 32-bit Virtual Key value used for non-keyboard input methods
            Attn = 0xF6, // Attn key
            Crsel = 0xF7, // Crsel key
            Exsel = 0xF8, // Exsel key
            EraseEof = 0xF9, // Erase EOF key
            Play = 0xFA, // Play key
            Zoom = 0xFB, // Zoom key
            Pa1 = 0xFD, // PA1 key
            OemClear = 0xFE, // Clear key
        }

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
        /// Sets the cursor position on the screen.
        /// </summary>
        /// <param name="X">The new x-coordinate of the cursor position.</param>
        /// <param name="Y">The new y-coordinate of the cursor position.</param>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetCursorPos(int X, int Y);

        private static Dictionary<Keys, bool> m_keyStateHistory = new Dictionary<Keys, bool>();

        static Input()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                m_keyStateHistory[key] = false;
            }
        }

        /// <summary>
        /// Checks if the specified key is currently pressed.
        /// </summary>
        /// <param name="vKey">The virtual key to check.</param>
        /// <returns>True if the key is down; otherwise, false.</returns>
        public static bool IsKeyDown(System.Windows.Forms.Keys vKey)
        {
            //short key = GetAsyncKeyState(vKey);
            short key = (short)(GetAsyncKeyState(vKey) & 0x8000);
            return Convert.ToBoolean(key);
        }

        /// <summary>
        /// Checks whether the specified key is currently pressed.
        /// </summary>
        /// <param name="vKey">The virtual key to check.</param>
        /// <returns><c>true</c> if the key is currently down; otherwise, <c>false</c>.</returns>
        /// <example>
        /// This example demonstrates how to use IsKeyDown method:
        /// <code>
        /// if (Input.IsKeyDown(Input.Keys.A))
        /// {
        ///     Console.WriteLine("Key A is pressed.");
        /// }
        /// </code>
        /// </example>
        public static bool IsKeyDown(Keys vKey)
        {
            short key = (short)(GetAsyncKeyState((int) vKey) & 0x8000);
            return Convert.ToBoolean(key);
        }

        // <summary>
        /// Checks if a key was just pressed down (Key Hit).
        /// </summary>
        /// <param name="vKey">The virtual key to check.</param>
        /// <returns><c>true</c> if the key was just pressed down; otherwise, <c>false</c>.</returns>
        public static bool IsKeyHit(Keys vKey)
        {
            bool isKeyDown = IsKeyDown(vKey);
            if (isKeyDown && !m_keyStateHistory[vKey])
            {
                m_keyStateHistory[vKey] = true;
                return true;
            }
            else if(!isKeyDown)
            {
                m_keyStateHistory[vKey] = false;
            }

            return false;
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
            WindowUtilities.RECT rect;
            WindowUtilities.GetClientRect(handle, out rect);

            WindowUtilities.POINT topLeft;
            topLeft.X = rect.Left;
            topLeft.Y = rect.Top;
            WindowUtilities.ClientToScreen(handle, ref topLeft);

            int windowX = topLeft.X;
            int windowY = topLeft.Y;

            WindowUtilities.POINT point;
            WindowUtilities.GetCursorPos(out  point);
            
            var mouseX = point.X - windowX;
            var mouseY = point.Y - windowY;

            return new Vec3(mouseX, mouseY);

            //return Input.GetRefMousePos(Control.FromHandle(handle)); alte funktion
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

        /// <summary>
        /// Sets the cursor position on the screen.
        /// </summary>
        /// <param name="pos">The new position of the cursor.</param>
        /// <param name="handle">The handle of the control where the cursor position will be set.</param>
        public static void SetCursorPos(Vec3 pos, IntPtr handle)
        {
            SetCursorPos((int) pos.X, (int) pos.Y, handle);
        }


        /// <summary>
        /// Sets the cursor position on the screen.
        /// </summary>
        /// <param name="X">The new x-coordinate of the cursor position.</param>
        /// <param name="Y">The new y-coordinate of the cursor position.</param>
        /// /// <param name="handle">The handle of the control where the cursor position will be set.</param>
        public static void SetCursorPos(int x, int y, IntPtr handle)
        {
            Point point = new Point();
            Control c = Control.FromHandle(handle);
            c.Invoke(new Action(() => { point = c.Location; }));
            SetCursorPos(point.X + x, point.Y + y);
        }

        /// <summary>
        /// Shows the cursor.
        /// </summary>
        public static void ShowCursor()
        {
            Cursor.Show();
        }

        /// <summary>
        /// Hides the cursor.
        /// </summary>
        public static void HideCursor()
        {
            Cursor.Hide();
        }
    }
}
