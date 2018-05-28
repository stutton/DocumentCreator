using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Stutton.DocumentCreator.Views.Controls
{
    [TemplatePart(Name = "PART_ExpandButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_CollapseButton", Type = typeof(Button))]
    public class EditorExpander : Expander
    {
        public const string ExpandButtonPartName = "PART_ExpandButton";
        public const string CollapseButtonPartName = "PART_CollapseButton";

        private Button _expandButton;
        private Button _collapseButton;

        #region string Group

        public static readonly DependencyProperty GroupProperty =
            DependencyProperty.Register(nameof(Group), typeof(string), typeof(EditorExpander), null);

        public string Group
        {
            get => (string) GetValue(GroupProperty);
            set => SetValue(GroupProperty, value);
        }

        #endregion

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _expandButton = GetTemplateChild(ExpandButtonPartName) as Button;
            _collapseButton = GetTemplateChild(CollapseButtonPartName) as Button;

            _expandButton.Click += (s, e) => IsExpanded = true;
            _collapseButton.Click += (s, e) => IsExpanded = false;
        }

        protected override void OnExpanded()
        {
            var siblingExpanders = this.GetSiblings<EditorExpander, ItemsControl>();
            foreach (var expander in siblingExpanders.Where(e => e.IsExpanded))
            {
                if (string.IsNullOrEmpty(Group))
                {
                    if (string.IsNullOrEmpty(expander.Group))
                    {
                        expander.IsExpanded = false;
                    }
                }
                else
                {
                    if (expander.Group == Group)
                    {
                        expander.IsExpanded = false;
                    }
                }
            }
        }
    }
}
