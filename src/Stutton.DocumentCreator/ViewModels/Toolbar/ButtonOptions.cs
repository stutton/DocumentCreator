using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Stutton.DocumentCreator.Shared;

namespace Stutton.DocumentCreator.ViewModels.Toolbar
{
    public class ButtonOptions : Observable
    {
        private bool _isShown;

        public bool IsShown
        {
            get => _isShown;
            set => Set(ref _isShown, value);
        }

        private ICommand _command;

        public ICommand Command
        {
            get => _command;
            set => Set(ref _command, value);
        }
    }
}
