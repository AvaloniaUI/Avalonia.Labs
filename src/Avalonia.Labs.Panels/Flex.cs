using System;

using Avalonia.Layout;

namespace Avalonia.Labs.Panels
{
    public static class Flex
    {
        /// <summary>
        /// Defines an attached property to control the alignment of a specific child in a flex layout.
        /// </summary>
        public static readonly AttachedProperty<AlignItems?> AlignSelfProperty =
            AvaloniaProperty.RegisterAttached<Layoutable, AlignItems?>("AlignSelf", typeof(Flex));

        /// <summary>
        /// Defines an attached property to control the order of a specific child in a flex layout.
        /// </summary>
        public static readonly AttachedProperty<int> OrderProperty =
            AvaloniaProperty.RegisterAttached<Layoutable, int>("Order", typeof(Flex));

        /// <summary>
        /// Gets the child alignment in a flex layout
        /// </summary>
        public static AlignItems? GetAlignSelf(Layoutable layoutable)
        {
            if (layoutable is null)
            {
                throw new ArgumentNullException(nameof(layoutable));
            }

            return layoutable.GetValue(AlignSelfProperty);
        }

        /// <summary>
        /// Sets the child alignment in a flex layout
        /// </summary>
        public static void SetAlignSelf(Layoutable layoutable, AlignItems? value)
        {
            if (layoutable is null)
            {
                throw new ArgumentNullException(nameof(layoutable));
            }

            layoutable.SetValue(AlignSelfProperty, value);
        }

        /// <summary>
        /// Gets the child order in a flex layout
        /// </summary>
        public static int GetOrder(Layoutable layoutable)
        {
            if (layoutable is null)
            {
                throw new ArgumentNullException(nameof(layoutable));
            }

            return layoutable.GetValue(OrderProperty);
        }

        /// <summary>
        /// Sets the child order in a flex layout
        /// </summary>
        public static void SetOrder(Layoutable layoutable, int value)
        {
            if (layoutable is null)
            {
                throw new ArgumentNullException(nameof(layoutable));
            }

            layoutable.SetValue(OrderProperty, value);
        }
    }
}
