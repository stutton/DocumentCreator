using System;
using System.Collections.Generic;
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

namespace Stutton.DocumentCreator.Views.Controls.Cards
{
    /// <summary>
    /// Interaction logic for DocumentCard.xaml
    /// </summary>
    public partial class DocumentCard : UserControl
    {
        public DocumentCard()
        {
            InitializeComponent();
        }

        #region ICommand SelectCommand

        public static readonly DependencyProperty SelectCommandProperty =
            DependencyProperty.Register(nameof(SelectCommand), typeof(ICommand), typeof(DocumentCard), new PropertyMetadata(default(ICommand)));

        public ICommand SelectCommand
        {
            get => (ICommand) GetValue(SelectCommandProperty);
            set => SetValue(SelectCommandProperty, value);
        }

        #endregion

        #region string DocumentName

        public static readonly DependencyProperty DocumentNameProperty =
            DependencyProperty.Register(nameof(DocumentName), typeof(string), typeof(DocumentCard), new PropertyMetadata(default(string)));

        public string DocumentName
        {
            get => (string) GetValue(DocumentNameProperty);
            set => SetValue(DocumentNameProperty, value);
        }

        #endregion

        

        #region string Type

        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register(nameof(Type), typeof(string), typeof(DocumentCard), new PropertyMetadata(default(string)));

        public string Type
        {
            get => (string) GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        #endregion

        #region string Description

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(DocumentCard), new PropertyMetadata(default(string)));

        public string Description
        {
            get => (string) GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        #endregion

        #region ICommand ShareCommand

        public static readonly DependencyProperty ShareCommandProperty =
            DependencyProperty.Register(nameof(ShareCommand), typeof(ICommand), typeof(DocumentCard), new PropertyMetadata(default(ICommand)));

        public ICommand ShareCommand
        {
            get => (ICommand) GetValue(ShareCommandProperty);
            set => SetValue(ShareCommandProperty, value);
        }

        #endregion

        #region ICommand EditCommand

        public static readonly DependencyProperty EditCommandProperty =
            DependencyProperty.Register(nameof(EditCommand), typeof(ICommand), typeof(DocumentCard), new PropertyMetadata(default(ICommand)));

        public ICommand EditCommand
        {
            get => (ICommand) GetValue(EditCommandProperty);
            set => SetValue(EditCommandProperty, value);
        }

        #endregion

        
    }
}
