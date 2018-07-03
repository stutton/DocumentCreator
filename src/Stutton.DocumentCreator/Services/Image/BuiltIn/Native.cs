using System;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace Stutton.DocumentCreator.Services.Image.BuiltIn
{
    internal static class Native
    {
        public static class Gdi32
        {
            public const int SRCCOPY = 0x00CC0020; // BitBlt dwRop parameter

            [DllImport("gdi32.dll")]
            public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest,
                                             int nWidth, int nHeight, IntPtr hObjectSource,
                                             int nXSrc, int nYSrc, CopyPixelOperation dwRop);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth,
                                                               int nHeight);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateCompatibleDC(IntPtr hDC);

            [DllImport("gdi32.dll")]
            public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

            [DllImport("gdi32.dll")]
            public static extern bool DeleteDC(IntPtr hDC);

            [DllImport("gdi32.dll")]
            public static extern bool DeleteObject(IntPtr hObject);

            [DllImport("gdi32.dll")]
            public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
        }

        public static class User32
        {
            public const uint SW_HIDE = 0;
            public const uint SW_MAXIMIZE = 3;
            public const uint SW_MINIMIZE = 6;
            public const uint SW_NORMAL = 1;
            public const uint SW_RESTORE = 9;
            public const uint SW_SHOW = 5;
            public const uint SW_SHOWMAXIMIZED = 3;
            public const uint SW_SHOWMINIMIZED = 2;
            public const uint SW_SHOWMINNOACTIVE = 7;
            public const uint SW_SHOWNA = 8;
            public const uint SW_SHOWNOACTIVATE = 4;
            public const uint SW_SHOWNORMAL = 1;

            public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

            [DllImport("user32.dll")]
            public static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

            [DllImport("user32.dll")]
            public static extern IntPtr GetDesktopWindow();

            [DllImport("user32.dll")]
            public static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll")]
            public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFOEX lpmi);

            [DllImport("user32.dll")]
            public static extern IntPtr GetWindowDC(IntPtr hWnd);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowInfo(IntPtr hWnd, out WINDOWINFO pwi);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetWindowRect(HandleRef hWnd, out RECT rect);

            [DllImport("user32.dll")]
            public static extern int GetWindowRgn(IntPtr hWnd, IntPtr hRgn);

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

            [DllImport("user32.dll", CharSet = CharSet.Unicode)]
            public static extern int GetWindowTextLength(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool IsWindowVisible(IntPtr hWnd);

            [DllImport("user32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, uint nFlags);

            [DllImport("user32.dll")]
            public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);

            public enum RegionFlags
            {
                ERROR = 0,
                NULLREGION = 1,
                SIMPLEREGION = 2,
                COMPLEXREGION = 3
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct MONITORINFOEX
            {
                public int Size;
                public Rect Monitor;
                public Rect WorkArea;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string DeviceName;
                public uint Flags;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct RECT
            {
                public int Left, Top, Right, Bottom;

                public RECT(int left, int top, int right, int bottom)
                {
                    Left = left;
                    Top = top;
                    Right = right;
                    Bottom = bottom;
                }

                public RECT(Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

                public int Height
                {
                    get { return Bottom - Top; }
                    set { Bottom = value + Top; }
                }

                public Point Location
                {
#pragma warning disable IdForAssignmendFromObjectCreationToPropertyNotDisposed // Undisposed ressource.
                    get { return new Point(Left, Top); }
#pragma warning restore IdForAssignmendFromObjectCreationToPropertyNotDisposed // Undisposed ressource.
                    set
                    {
                        X = value.X;
                        Y = value.Y;
                    }
                }

                public Size Size
                {
#pragma warning disable IdForAssignmendFromObjectCreationToPropertyNotDisposed // Undisposed ressource.
                    get { return new Size(Width, Height); }
#pragma warning restore IdForAssignmendFromObjectCreationToPropertyNotDisposed // Undisposed ressource.
                    set
                    {
                        Width = value.Width;
                        Height = value.Height;
                    }
                }

                public int Width
                {
                    get { return Right - Left; }
                    set { Right = value + Left; }
                }

                public int X
                {
                    get { return Left; }
                    set
                    {
                        Right -= Left - value;
                        Left = value;
                    }
                }

                public int Y
                {
                    get { return Top; }
                    set
                    {
                        Bottom -= Top - value;
                        Top = value;
                    }
                }

                public override bool Equals(object obj)
                {
                    if (obj is RECT)
                        return Equals((RECT)obj);
                    if (obj is Rectangle)
                        return Equals(new RECT((Rectangle)obj));
                    return false;
                }

                public override int GetHashCode()
                {
                    return ((Rectangle)this).GetHashCode();
                }

                public override string ToString()
                {
                    return string.Format(CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
                }

                public bool Equals(RECT r)
                {
                    return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
                }

                public static implicit operator Rectangle(RECT r)
                {
                    return new Rectangle(r.Left, r.Top, r.Width, r.Height);
                }

                public static implicit operator RECT(Rectangle r)
                {
                    return new RECT(r);
                }

                public static bool operator ==(RECT r1, RECT r2)
                {
                    return r1.Equals(r2);
                }

                public static bool operator !=(RECT r1, RECT r2)
                {
                    return !r1.Equals(r2);
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct WINDOWPLACEMENT
            {
                public uint Length;
                public uint Flags;
                public uint ShowCmd;
                public Point PtMinPosition;
                public Point PtMaxPosition;
                public RECT RcNormalPosition;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct WINDOWINFO
            {
                public uint cbSize;
                public RECT rcWindow;
                public RECT rcClient;
                public uint dwStyle;
                public uint dwExStyle;
                public uint dwWindowStatus;
                public uint cxWindowBorders;
                public uint cyWindowBorders;
                public ushort atomWindowType;
                public ushort wCreatorVersion;

                public WINDOWINFO(bool? filler) : this() // Allows automatic initialization of "cbSize" with "new WINDOWINFO(null/true/false)".
                {
                    cbSize = (uint)Marshal.SizeOf(typeof(WINDOWINFO));
                }
            }
        }
    }
}
