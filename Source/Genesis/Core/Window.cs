using Genesis.Graphics;
using Genesis.Math;
using GlmSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Genesis.Core
{
    /// <summary>
    /// Represents a window with specified properties and events.
    /// </summary>
    public class Window
    {
        private const int WS_OVERLAPPEDWINDOW = 0x00CF0000;
        private const int WS_VISIBLE = 0x10000000;
        private const int CW_USEDEFAULT = unchecked((int)0x80000000);
        private const int WM_DESTROY = 0x0002;
        private const int WM_CLOSE = 0x0010;
        private const int WM_SIZE = 0x0005;
        private const uint WM_QUIT = 0x0012;

        private static WndProc s_windowProcDelegate;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr CreateWindowEx(
            int dwExStyle,
            string lpClassName,
            string lpWindowName,
            int dwStyle,
            int x,
            int y,
            int nWidth,
            int nHeight,
            IntPtr hWndParent,
            IntPtr hMenu,
            IntPtr hInstance,
            IntPtr lpParam);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr DefWindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern IntPtr LoadCursor(IntPtr hInstance, int lpCursorName);

        [DllImport("user32.dll")]
        private static extern void PostQuitMessage(int nExitCode);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMessage(out MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        private static extern IntPtr DispatchMessage(ref MSG lpmsg);

        [DllImport("user32.dll")]
        private static extern IntPtr TranslateMessage(ref MSG lpmsg);

        private delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Represents extended window class information.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct WNDCLASSEX
        {
            public uint cbSize;
            public uint style;
            public WndProc lpfnWndProc;
            public int cbClsExtra;
            public int cbWndExtra;
            public IntPtr hInstance;
            public IntPtr hIcon;
            public IntPtr hCursor;
            public IntPtr hbrBackground;
            public string lpszMenuName;
            public string lpszClassName;
            public IntPtr hIconSm;
        }

        /// <summary>
        /// Represents a Windows message.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct MSG
        {
            public IntPtr hwnd;
            public uint message;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public System.Drawing.Point pt;
        }

        /// <summary>
        /// Delegate for window event handlers.
        /// </summary>
        /// <param name="window">The window associated with the event.</param>
        public delegate void WindowEventHandler(Window window);

        /// <summary>
        /// Gets or sets the handle to the window.
        /// </summary>
        public IntPtr Handle { get; set; }

        public Vec3 WindowSize { get; set; }

        /// <summary>
        /// Event triggered when the window is closed.
        /// </summary>
        public event WindowEventHandler Close;

        /// <summary>
        /// Event triggered when the window size changes.
        /// </summary>
        public event WindowEventHandler OnSizeChange;
        

        private bool m_isWindowClosed = false;
        private Game m_game;
        private object m_lock = new object();


        /// <summary>
        /// Callback function for processing window messages.
        /// </summary>
        /// <param name="hWnd">Handle to the window.</param>
        /// <param name="msg">The message received.</param>
        /// <param name="wParam">Additional message information.</param>
        /// <param name="lParam">Additional message information.</param>
        /// <returns>Result of the message processing.</returns>
        private IntPtr WindowProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case WM_SIZE:
                    int width = lParam.ToInt32() & 0xFFFF;
                    int height = (lParam.ToInt32() >> 16) & 0xFFFF;
                    this.WindowSize = new Vec3(width, height);
                    if (m_game != null)
                    {
                        m_game.Viewport.Width = width;
                        m_game.Viewport.Height = height;
                        if(m_game.SelectedScene != null)
                        {
                            m_game.SelectedScene.ResizeScene(m_game.Viewport);
                        }
                        OnSizeChange?.Invoke(this);
                    }
                    return IntPtr.Zero;
                case WM_CLOSE:
                    CloseWindow();
                    return IntPtr.Zero;
                case WM_DESTROY:
                    PostQuitMessage(0);
                    return IntPtr.Zero;
                default:
                    return DefWindowProc(hWnd, msg, wParam, lParam);
            }
        }

        /// <summary>
        /// Creates a new window with specified title and viewport size.
        /// </summary>
        /// <param name="title">Title of the window.</param>
        /// <param name="viewport">Initial viewport size.</param>
        /// <returns>Handle to the created window.</returns>
        public IntPtr CreateWindowHandle(String title, Viewport viewport)
        {
            s_windowProcDelegate = WindowProc;
            WNDCLASSEX wNDCLASSEX = new WNDCLASSEX
            {
                cbSize = (uint)Marshal.SizeOf(typeof(WNDCLASSEX)),
                style = 0,
                lpfnWndProc = s_windowProcDelegate,
                cbClsExtra = 0,
                cbWndExtra = 0,
                hInstance = GetModuleHandle(null),
                hIcon = IntPtr.Zero,
                hCursor = LoadCursor(IntPtr.Zero, 32512),
                hbrBackground = IntPtr.Zero,
                lpszMenuName = null,
                lpszClassName = title,
                hIconSm = IntPtr.Zero
            };

            RegisterClassEx(ref wNDCLASSEX);

            Handle = CreateWindowEx(
                0,
                title,
                title,
                WS_OVERLAPPEDWINDOW | WS_VISIBLE,
                CW_USEDEFAULT,
                CW_USEDEFAULT,
                (int) viewport.Width,
                (int) viewport.Height,
                IntPtr.Zero,
                IntPtr.Zero,
                wNDCLASSEX.hInstance,
                IntPtr.Zero);

            if(Handle == IntPtr.Zero)
            {
                throw new System.ComponentModel.Win32Exception(Marshal.GetLastWin32Error());
            }

            return Handle;
        }

        /// <summary>
        /// Shows the window and starts the game loop.
        /// </summary>
        /// <param name="game">The game instance associated with the window.</param>
        public void RunGame(Game game)
        {
            m_game = game;
            m_game.Viewport.Width = WindowSize.X;
            m_game.Viewport.Height = WindowSize.Y;
            m_game.Start();
            while(!m_isWindowClosed)
            {
                MSG msg;
                while (GetMessage(out msg, IntPtr.Zero, 0, 0))
                {
                    TranslateMessage(ref msg);
                    DispatchMessage(ref msg);
                }
                m_isWindowClosed = true;
            }
        }

        /// <summary>
        /// Closes the window and stops the associated game.
        /// </summary>
        private void CloseWindow()
        {
            m_game.Stop();
            m_game.GameThread.Join();
            DestroyWindow(Handle);
            s_windowProcDelegate = null;
        }

        /// <summary>
        /// Returns an vector with the window client size
        /// </summary>
        /// <returns></returns>
        public Vec3 GetClientSize()
        {
            WindowUtilities.RECT rect;
            WindowUtilities.GetClientRect(Handle, out rect);

            WindowUtilities.POINT topLeft;
            topLeft.X = rect.Left;
            topLeft.Y = rect.Top;
            WindowUtilities.ClientToScreen(Handle, ref topLeft);

            WindowUtilities.POINT btmRight;
            btmRight.X = rect.Right;
            btmRight.Y = rect.Bottom;
            WindowUtilities.ClientToScreen(Handle, ref btmRight);

            int width = btmRight.X - topLeft.X;
            int height = btmRight.Y - topLeft.Y;

            return new Vec3(width, height);
        }

        public static Vec3 GetClientLocation(IntPtr handle)
        {
            WindowUtilities.RECT rect;
            WindowUtilities.GetClientRect(handle, out rect);

            WindowUtilities.POINT topLeft;
            topLeft.X = rect.Left;
            topLeft.Y = rect.Top;
            WindowUtilities.ClientToScreen(handle, ref topLeft);

            var x = topLeft.X;
            var y = topLeft.Y;

            return new Vec3(x, y);
        }

        public Vec3 GetClientLocation()
        {
            return Window.GetClientLocation(Handle);
        }
    }
}
