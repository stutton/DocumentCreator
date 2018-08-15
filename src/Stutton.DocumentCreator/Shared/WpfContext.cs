using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Stutton.DocumentCreator.Shared
{
    public sealed class WpfContext : IContext
    {
        private readonly Dispatcher _dispatcher;

        public WpfContext(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        public WpfContext() : this(Dispatcher.CurrentDispatcher) { }

        public bool IsInvokeRequired => _dispatcher.Thread != Thread.CurrentThread;

        public void Invoke(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            _dispatcher.Invoke(action);
        }
    }
}
