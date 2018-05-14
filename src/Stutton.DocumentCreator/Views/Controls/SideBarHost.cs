using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MaterialDesignThemes.Wpf.Transitions;

namespace Stutton.DocumentCreator.Views.Controls
{
    [TemplateVisualState(Name = TemplateDrawerOpenStateName, GroupName = TemplateDrawerGroupName)]
    [TemplateVisualState(Name = TemplateDrawerClosedStateName, GroupName = TemplateDrawerGroupName)]
    [TemplatePart(Name = TemplateDrawerPartName, Type = typeof(FrameworkElement))]
    public sealed class SideBarHost : ContentControl
    {
        public const string TemplateDrawerClosedStateName = "DrawerClosed";
        public const string TemplateDrawerGroupName = "Drawer";
        public const string TemplateDrawerOpenStateName = "DrawerOpen";
        public const string TemplateDrawerPartName = "PART_Drawer";

        public static readonly DependencyProperty DrawerBackgroundProperty =
            DependencyProperty.Register("DrawerBackground", typeof(Brush), typeof(SideBarHost),
                new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty DrawerContentProperty =
            DependencyProperty.Register("DrawerContent", typeof(object), typeof(SideBarHost),
                new PropertyMetadata(default(object)));

        public static readonly DependencyProperty DrawerContentTemplateProperty =
            DependencyProperty.Register("DrawerContentTemplate", typeof(DataTemplate), typeof(SideBarHost),
                new PropertyMetadata(default(DataTemplate)));

        public static readonly DependencyProperty DrawerContentTemplateSelectorProperty =
            DependencyProperty.Register("DrawerContentTemplateSelector", typeof(DataTemplateSelector),
                typeof(SideBarHost), new PropertyMetadata(default(DataTemplateSelector)));

        public static readonly DependencyProperty IsDrawerOpenProperty =
            DependencyProperty.Register("IsDrawerOpen", typeof(bool), typeof(SideBarHost),
                new FrameworkPropertyMetadata(default(bool), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    IsDrawerOpenPropertyChangedCallback));

        private static readonly RoutedCommand CloseDrawerCommand = new RoutedCommand();
        private static readonly RoutedCommand OpenDrawerCommand = new RoutedCommand();
        private FrameworkElement _drawerElement;

        static SideBarHost()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SideBarHost),
                new FrameworkPropertyMetadata(typeof(SideBarHost)));
        }

        public SideBarHost()
        {
            CommandBindings.Add(new CommandBinding(OpenDrawerCommand, OpenDrawerHandler));
            CommandBindings.Add(new CommandBinding(CloseDrawerCommand, CloseDrawerHandler));
        }

        public Brush DrawerBackground
        {
            get => (Brush) GetValue(DrawerBackgroundProperty);
            set => SetValue(DrawerBackgroundProperty, value);
        }

        public object DrawerContent
        {
            get => GetValue(DrawerContentProperty);
            set => SetValue(DrawerContentProperty, value);
        }

        public DataTemplate DrawerContentTemplate
        {
            get => (DataTemplate) GetValue(DrawerContentTemplateProperty);
            set => SetValue(DrawerContentTemplateProperty, value);
        }

        public DataTemplateSelector DrawerContentTemplateSelector
        {
            get => (DataTemplateSelector) GetValue(DrawerContentTemplateSelectorProperty);
            set => SetValue(DrawerContentTemplateSelectorProperty, value);
        }

        public bool IsDrawerOpen
        {
            get => (bool) GetValue(IsDrawerOpenProperty);
            set => SetValue(IsDrawerOpenProperty, value);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _drawerElement = GetTemplateChild(TemplateDrawerPartName) as FrameworkElement;

            UpdateVisualStates(false);
        }

        private static void IsDrawerOpenPropertyChangedCallback(DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var sideBarHost = (SideBarHost) dependencyObject;
            sideBarHost.UpdateVisualStates();
        }

        private void CloseDrawerHandler(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (executedRoutedEventArgs.Handled) return;

            SetCurrentValue(IsDrawerOpenProperty, false);

            executedRoutedEventArgs.Handled = true;
        }

        private void OpenDrawerHandler(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if (executedRoutedEventArgs.Handled) return;

            SetCurrentValue(IsDrawerOpenProperty, true);

            executedRoutedEventArgs.Handled = true;
        }

        private void UpdateVisualStates(bool? useTransitions = null)
        {
            VisualStateManager.GoToState(this,
                IsDrawerOpen ? TemplateDrawerOpenStateName : TemplateDrawerClosedStateName,
                useTransitions ?? !TransitionAssist.GetDisableTransitions(this));
        }
    }
}