using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Fields.List
{
    public class ListStepModel : Observable
    {
        private string _text;

        public string Text
        {
            get => _text;
            set => Set(ref _text, value);
        }

        private BitmapSource _image;

        public BitmapSource Image
        {
            get => _image;
            set => Set(ref _image, value);
        }
    }
}
