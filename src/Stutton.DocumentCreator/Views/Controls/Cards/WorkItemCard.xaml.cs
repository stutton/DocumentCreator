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
    /// Interaction logic for WorkItemCard.xaml
    /// </summary>
    public partial class WorkItemCard : UserControl
    {
        public WorkItemCard()
        {
            InitializeComponent();
        }

        #region ICommand SelectCommand

        public static readonly DependencyProperty SelectCommandProperty =
            DependencyProperty.Register(nameof(SelectCommand), typeof(ICommand), typeof(WorkItemCard), new PropertyMetadata(default(ICommand)));

        public ICommand SelectCommand
        {
            get => (ICommand) GetValue(SelectCommandProperty);
            set => SetValue(SelectCommandProperty, value);
        }

        #endregion

        #region object SelectCommandParameter

        public static readonly DependencyProperty SelectCommandParameterProperty =
            DependencyProperty.Register(nameof(SelectCommandParameter), typeof(object), typeof(WorkItemCard), new PropertyMetadata(default(object)));

        public object SelectCommandParameter
        {
            get => (object) GetValue(SelectCommandParameterProperty);
            set => SetValue(SelectCommandParameterProperty, value);
        }

        #endregion

        

        #region string Title

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(WorkItemCard), new PropertyMetadata(default(string)));

        public string Title
        {
            get => (string) GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion

        #region string Description

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register(nameof(Description), typeof(string), typeof(WorkItemCard), new PropertyMetadata(default(string)));

        public string Description
        {
            get => (string) GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        #endregion

        #region int Id

        public static readonly DependencyProperty IdProperty =
            DependencyProperty.Register(nameof(Id), typeof(int), typeof(WorkItemCard), new PropertyMetadata(default(int)));

        public int Id
        {
            get => (int) GetValue(IdProperty);
            set => SetValue(IdProperty, value);
        }

        #endregion

        #region string State

        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register(nameof(State), typeof(string), typeof(WorkItemCard), new PropertyMetadata(default(string)));

        public string State
        {
            get => (string) GetValue(StateProperty);
            set => SetValue(StateProperty, value);
        }

        #endregion

        #region bool IsSelected

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(WorkItemCard), new PropertyMetadata(default(bool)));

        public bool IsSelected
        {
            get => (bool) GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        #endregion
    }
}
