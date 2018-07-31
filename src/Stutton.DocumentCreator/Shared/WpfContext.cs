using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Stutton.DocumentCreator.Shared
{
    public static class WpfContext
    {
        private static Dispatcher _dispatcher;

        static WpfContext()
        {
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public static bool IsSynchronized => _dispatcher.Thread == Thread.CurrentThread;

        public static bool IsInvokeRequired => _dispatcher.Thread != Thread.CurrentThread;

        public static void BeginInvoke(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            _dispatcher.BeginInvoke(action);
        }

        public static void Invoke(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            _dispatcher.Invoke(action);
        }
    }
}
