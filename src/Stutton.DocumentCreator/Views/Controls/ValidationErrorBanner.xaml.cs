using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ValidationError = Stutton.DocumentCreator.Shared.ValidationError;

namespace Stutton.DocumentCreator.Views.Controls
{
    /// <summary>
    /// Interaction logic for ValidationErrorBanner.xaml
    /// </summary>
    public partial class ValidationErrorBanner : UserControl
    {
        public ValidationErrorBanner()
        {
            InitializeComponent();
        }

        #region ObservableCollection<ValidationError> ValidationErrors (Dependancy Property)

        public static readonly DependencyProperty ValidationErrorsProperty = DependencyProperty.Register(nameof(ValidationErrors), typeof(ObservableCollection<ValidationError>), typeof(ValidationErrorBanner), new PropertyMetadata(default(ObservableCollection<ValidationError>)));

        public ObservableCollection<ValidationError> ValidationErrors
        {
            get => (ObservableCollection<ValidationError>) GetValue(ValidationErrorsProperty);
            set => SetValue(ValidationErrorsProperty, value);
        }

        #endregion
    }
}
