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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _expandButton = GetTemplateChild(ExpandButtonPartName) as Button;
            _collapseButton = GetTemplateChild(CollapseButtonPartName) as Button;

            _expandButton.Click += (s, e) => IsExpanded = true;
            _collapseButton.Click += (s, e) => IsExpanded = false;
        }
    }
}
