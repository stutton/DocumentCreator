using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stutton.DocumentCreator.Services.Image.BuiltIn
{
    internal static class WindowHelper
    {
        private static Native.User32.RECT _screenRectangle;
        private static bool _screenRectInit;

        public static Native.User32.RECT GetScreenRectangle()
        {
            if (_screenRectInit) return _screenRectangle;
            foreach (var screen in Screen.AllScreens)
            {
                if (screen.Bounds.Left < _screenRectangle.Left)
                {
                    _screenRectangle.Left = screen.Bounds.Left;
                }
                if (screen.Bounds.Top < _screenRectangle.Top)
                {
                    _screenRectangle.Top = screen.Bounds.Top;
                }
                _screenRectangle.Width += screen.Bounds.Width;
                _screenRectangle.Height += screen.Bounds.Height;
            }
            _screenRectInit = true;
            return _screenRectangle;
        }

        public static List<WindowInfo> GetVisibleWindowPositions()
        {
            var windows = FindWindows((wnd, param) =>
            {
                var windowTitle = GetWindowsText(wnd);
                return Native.User32.IsWindowVisible(wnd) && !string.IsNullOrWhiteSpace(windowTitle);
            });

            var result = new List<WindowInfo>();
            foreach (var window in windows)
            {
                // Ignore Program Manager and Settings?
                if (window.Title == "Program Manager" || window.Title == "Settings")
                {
                    continue;
                }
                // Check if window is maximized and adjust window rectangle
                Native.User32.GetWindowPlacement(window.Hwnd, out var windowPlacement);
                Native.User32.GetWindowInfo(window.Hwnd, out var windowInfo);
                var winRect = new Native.User32.RECT();
                if (windowPlacement.ShowCmd == Native.User32.SW_MAXIMIZE)
                {
                    winRect.Top = window.Rect.Top + (int)windowInfo.cyWindowBorders;
                    winRect.Height = window.Rect.Height - (int)windowInfo.cyWindowBorders * 2;
                }
                else
                {
                    winRect.Top = window.Rect.Top;
                    winRect.Height = window.Rect.Height - (int)windowInfo.cyWindowBorders;
                }
                winRect.Left = window.Rect.Left + (int)windowInfo.cxWindowBorders;
                winRect.Width = window.Rect.Width - (int)windowInfo.cxWindowBorders * 2;
                window.Rect = winRect;

                if (result.Count == 0 || !result.Any(r => RectContains(r.Rect, window.Rect)))
                {
                    result.Add(window);
                }
            }
            return result;
        }

        public static List<WindowInfo> FindWindows(Native.User32.EnumWindowsProc filter)
        {
            var windows = new List<IntPtr>();
            Native.User32.EnumWindows((wnd, param) =>
            {
                if (filter(wnd, param))
                {
                    windows.Add(wnd);
                }
                return true;
            }, IntPtr.Zero);

            var results = new List<WindowInfo>();
            foreach (var wnd in windows)
            {
                Native.User32.GetWindowRect(new HandleRef(null, wnd), out var rect);
                results.Add(new WindowInfo
                {
                    Hwnd = wnd,
                    Title = GetWindowsText(wnd),
                    Rect = rect
                });
            }
            return results;
        }

        public static string GetWindowsText(IntPtr hWnd)
        {
            var size = Native.User32.GetWindowTextLength(hWnd);
            if (size <= 0)
            {
                return string.Empty;
            }

            var builder = new StringBuilder(size + 1);
            Native.User32.GetWindowText(hWnd, builder, builder.Capacity);
            return builder.ToString();
        }

        private static bool RectContains(Native.User32.RECT rect1, Native.User32.RECT rect2)
        {
            return rect1.Left <= rect2.Left
                   && rect1.Top <= rect2.Top
                   && rect1.Right >= rect2.Right
                   && rect1.Bottom >= rect2.Bottom;
        }
    }
}
