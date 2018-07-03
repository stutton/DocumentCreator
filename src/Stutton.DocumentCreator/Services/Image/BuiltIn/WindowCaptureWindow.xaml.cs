using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Stutton.DocumentCreator.Services.Image.BuiltIn
{
    /// <summary>
    /// Interaction logic for WindowCaptureWindow.xaml
    /// </summary>
    internal partial class WindowCaptureWindow : Window
    {
        private WindowInfo _internalSelectedWindow;

        public WindowCaptureWindow(List<WindowInfo> windows, Native.User32.RECT screenRect)
        {
            InitializeComponent();

            var left = screenRect.Left;
            Left = screenRect.Left;
            var top = screenRect.Top;
            Top = screenRect.Top;
            Width = screenRect.Width;
            Height = screenRect.Height;

            Loaded += (s, e) =>
            {
                BackgroundRect.Rect = new Rect(0, 0, Width, Height);
                Activate();
            };

            MouseMove += (s, e) =>
            {
                var mousePoint = e.GetPosition(HighlightCanvas);
                var mousePointScreen = new Point(mousePoint.X + left, mousePoint.Y + top);
                foreach (var windowInfo in windows)
                {
                    if (RectContainsPoint(windowInfo.Rect, mousePointScreen))
                    {
                        var currentRect = new Rect(windowInfo.Rect.Left + Math.Abs(left),
                                                     windowInfo.Rect.Top + Math.Abs(top),
                                                     windowInfo.Rect.Width,
                                                     windowInfo.Rect.Height);
                        CutoutRect.Rect = currentRect;
                        _internalSelectedWindow = windowInfo;
                        break;
                    }
                }
            };
        }

        public WindowInfo SelectedWindow { get; private set; }

        private void WindowCaptureWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                SelectedWindow = null;
                DialogResult = false;
                Close();
            }
        }

        private void WindowCaptureWindow_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            SelectedWindow = _internalSelectedWindow;
            DialogResult = true;
            Close();
        }

        private static bool RectContainsPoint(Native.User32.RECT rect, Point p)
        {
            return p.X > rect.Left && p.X < rect.Left + rect.Width
                                   && p.Y > rect.Top && p.Y < rect.Top + rect.Height;
        }
    }
}
