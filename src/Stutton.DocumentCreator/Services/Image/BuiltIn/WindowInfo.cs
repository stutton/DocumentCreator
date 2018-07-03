using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Services.Image.BuiltIn
{
    internal class WindowInfo
    {
        public Native.User32.RECT Rect { get; set; }
        public string Title { get; set; }
        public IntPtr Hwnd { get; set; }
    }
}
