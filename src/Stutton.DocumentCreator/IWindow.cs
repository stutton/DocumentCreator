using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator
{
    public interface IWindow
    {
        bool IsMaximized { get; }

        void Close();
        void Maximize();
        void Restore();
        void Minimize();
    }
}
