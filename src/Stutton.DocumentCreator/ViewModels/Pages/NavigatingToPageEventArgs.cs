using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stutton.DocumentCreator.ViewModels.Pages
{
    public class NavigatingToPageEventArgs : EventArgs
    {
        public NavigatingToPageEventArgs(IPage oldPage, IPage newPage)
        {
            OldPage = oldPage;
            NewPage = newPage;
        }

        public IPage OldPage { get; }
        public IPage NewPage { get; }
    }
}
