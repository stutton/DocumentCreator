using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Stutton.DocumentCreator.Views
{
    public static class UIExtensions
    {

        public static IEnumerable<TSibling> GetSiblings<TSibling, TParent>(this DependencyObject me) where TSibling : DependencyObject
        {
            if ((VisualTreeHelper.GetParent(me) is UIElement parent))
            {
                while (parent != null && !(parent is TParent))
                {
                    parent = VisualTreeHelper.GetParent(parent) as UIElement;
                }

                if (parent != null)
                {
                    foreach (var child in FindVisualChildren<TSibling>(parent))
                    {
                        if (!ReferenceEquals(child, me))
                        {
                            yield return child;
                        }
                    }
                }
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject me) where T : DependencyObject
        {
            if (me != null)
            {
                for (var i = 0; i < VisualTreeHelper.GetChildrenCount(me); i++)
                {
                    var child = VisualTreeHelper.GetChild(me, i);
                    if (child is T tChild)
                    {
                        yield return tChild;
                    }

                    foreach (var childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
