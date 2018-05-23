using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.Shared
{
    public abstract class Observable : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string property = null)
        {
            if (Equals(field, value))
            {
                return false;
            }

            field = value;
            RaisePropetyChanged(property);
            return true;
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {

        }

        private void RaisePropetyChanged(string property)
        {
            var eventArgs = new PropertyChangedEventArgs(property);
            OnPropertyChanged(eventArgs);
            PropertyChanged?.Invoke(this, eventArgs);
        }
    }
}
