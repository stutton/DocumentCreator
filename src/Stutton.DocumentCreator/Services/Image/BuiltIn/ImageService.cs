using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Stutton.DocumentCreator.Shared;
using Clipboard = System.Windows.Clipboard;

namespace Stutton.DocumentCreator.Services.Image.BuiltIn
{
    public class ImageService : IImageService
    {
        public IResponse<BitmapSource> GetWindowCapture()
        {
            var visibleWindows = WindowHelper.GetVisibleWindowPositions();
            var screenRect = WindowHelper.GetScreenRectangle();
            var captureWindow = new WindowCaptureWindow(visibleWindows, screenRect);
            var result = captureWindow.ShowDialog();
            if (result != true)
            {
                Response<BitmapSource>.FromFailure("Window capture canceled");
            }

            var selectedWindow = captureWindow.SelectedWindow;

            var captureWindowBitmap = CaptureRectangle(selectedWindow.Rect);
            captureWindowBitmap.Freeze();
            return Response<BitmapSource>.FromSuccess(captureWindowBitmap);
        }

        public IResponse<BitmapSource> GetImageFromClipboard()
        {
            if (!Clipboard.ContainsImage())
            {
                return Response<BitmapSource>.FromFailure("No image on clipboard");
            }

            var clipboardData = System.Windows.Forms.Clipboard.GetDataObject();
            if (clipboardData == null)
            {
                return Response<BitmapSource>.FromFailure("No data on clipboard");
            }

            if (!clipboardData.GetDataPresent(System.Windows.Forms.DataFormats.Bitmap))
            {
                return Response<BitmapSource>.FromFailure("Failed to get bitmap from clipboard");
            }

            var bitmap =
                (System.Drawing.Bitmap) clipboardData.GetData(System.Windows.Forms.DataFormats.Bitmap);
            var hBitmap = bitmap.GetHbitmap();
            try
            {
                var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                return Response<BitmapSource>.FromSuccess(bitmapSource);
            }
            finally
            {
                Native.Gdi32.DeleteObject(hBitmap);
            }
        }

        private static Rectangle GetScreenBounds()
        {
            return SystemInformation.VirtualScreen;
        }

        private static BitmapSource CaptureRectangle(Rectangle rect)
        {
            var bounds = GetScreenBounds();
            var finalRect = Rectangle.Intersect(bounds, rect);

            var desktopHandle = Native.User32.GetDesktopWindow();

            var hdcSrc = Native.User32.GetWindowDC(desktopHandle);
            var hdcDest = Native.Gdi32.CreateCompatibleDC(hdcSrc);
            var hBitmap = Native.Gdi32.CreateCompatibleBitmap(hdcSrc, finalRect.Width, finalRect.Height);
            var hOld = Native.Gdi32.SelectObject(hdcDest, hBitmap);

            Native.Gdi32.BitBlt(hdcDest, 0, 0, finalRect.Width, finalRect.Height, hdcSrc, finalRect.X, finalRect.Y, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);

            Native.Gdi32.SelectObject(hdcDest, hOld);
            Native.Gdi32.DeleteDC(hdcDest);
            Native.User32.ReleaseDC(desktopHandle, hdcSrc);
            var captureImage = Imaging.CreateBitmapSourceFromHBitmap(hBitmap,
                                                                     IntPtr.Zero,
                                                                     Int32Rect.Empty,
                                                                     BitmapSizeOptions.FromEmptyOptions());
            Native.Gdi32.DeleteObject(hBitmap);

            return captureImage;
        }
    }
}
