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
    public class BytesToBitmapSourceTypeConverter : ITypeConverter<byte[], BitmapSource>
    {
        public BitmapSource Convert(byte[] source, BitmapSource destination, ResolutionContext context)
        {
            using (var stream = new MemoryStream(source))
            {
                return BitmapFrame.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
        }
    }
}
