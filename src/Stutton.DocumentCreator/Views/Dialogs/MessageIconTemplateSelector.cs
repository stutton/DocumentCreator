using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Stutton.DocumentCreator.ViewModels.Dialogs;

namespace Stutton.DocumentCreator.Views.Dialogs
{
    internal class MessageIconTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ErrorIconTemplate { get; set; }
        public DataTemplate SuccessIconTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ErrorMessageDialogViewModel)
            {
                return ErrorIconTemplate;
            }
            if (item is SuccessMessageDialogViewModel)
            {
                return SuccessIconTemplate;
            }
            return null;
        }
    }
}
