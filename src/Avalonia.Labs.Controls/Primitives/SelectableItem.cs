using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Automation.Peers;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Mixins;

namespace Avalonia.Labs.Controls.Primitives
{
    [PseudoClasses(":pressed", ":selected")]
    public class SelectableItem : ContentControl, ISelectable
    {
        /// <summary>
        /// Defines the <see cref="IsSelected"/> property.
        /// </summary>
        public static readonly StyledProperty<bool> IsSelectedProperty =
            AvaloniaProperty.Register<SelectableItem, bool>(nameof(IsSelected));

        /// <summary>
        /// Initializes static members of the <see cref="SelectableItem"/> class.
        /// </summary>
        static SelectableItem()
        {
            SelectableMixin.Attach<SelectableItem>(IsSelectedProperty);
            PressedMixin.Attach<SelectableItem>();
            FocusableProperty.OverrideDefaultValue<SelectableItem>(true);
        }

        /// <summary>
        /// Gets or sets the selection state of the item.
        /// </summary>
        public bool IsSelected
        {
            get { return GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ListItemAutomationPeer(this);
        }
    }
}
