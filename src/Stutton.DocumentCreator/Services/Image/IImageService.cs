using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Services.Image
{
    public interface IImageService
    {
        IResponse<BitmapSource> GetWindowCapture();
        IResponse<BitmapSource> GetImageFromClipboard();
    }
}
