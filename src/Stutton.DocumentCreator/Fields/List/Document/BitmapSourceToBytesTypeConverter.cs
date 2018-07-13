using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using AutoMapper;

namespace Stutton.DocumentCreator.Fields.List.Document
{
    public sealed class BitmapSourceToBytesTypeConverter : ITypeConverter<BitmapSource, byte[]>
    {
        public byte[] Convert(BitmapSource source, byte[] destination, ResolutionContext context)
        {
            if (source == null)
            {
                return null;
            }
            using (var stream = new MemoryStream())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(source));
                encoder.Save(stream);
                return stream.ToArray();
            }
        }
    }
}
