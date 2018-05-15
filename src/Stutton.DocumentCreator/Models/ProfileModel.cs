using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.Models
{
    public class ProfileModel : Observable
    {
        private string _name;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private BitmapSource _profilePicture;

        public BitmapSource ProfilePicture
        {
            get => _profilePicture;
            set => Set(ref _profilePicture, value);
        }
    }
}
