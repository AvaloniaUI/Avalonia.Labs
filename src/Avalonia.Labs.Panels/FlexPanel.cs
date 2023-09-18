using System;
using System.Collections.Generic;
using System.Linq;

using Avalonia.Controls;
using Avalonia.Layout;

namespace Avalonia.Labs.Panels
{
    public sealed class FlexPanel : Panel
    {
        private static readonly AttachedProperty<FlexPanel?> OwnerFlexPanelProperty =
            AvaloniaProperty.RegisterAttached<FlexPanel, Layoutable, FlexPanel?>("OwnerFlexPanel");

        private static readonly Func<Layoutable, int> s_getOrder = x => x is { } y ? Flex.GetOrder(y) : 0;
        private static readonly Func<Layoutable, bool> s_isVisible = x => x.IsVisible;

        /// <summary>
        /// Defines the <see cref="Direction"/> property.
        /// </summary>
        public static readonly StyledProperty<FlexDirection> DirectionProperty =
            AvaloniaProperty.Register<FlexPanel, FlexDirection>(nameof(Direction));

        /// <summary>
        /// Defines the <see cref="JustifyContent"/> property.
        /// </summary>
        public static readonly StyledProperty<JustifyContent> JustifyContentProperty =
            AvaloniaProperty.Register<FlexPanel, JustifyContent>(nameof(JustifyContent));

        /// <summary>
        /// Defines the <see cref="AlignItems"/> property.
        /// </summary>
        public static readonly StyledProperty<AlignItems> AlignItemsProperty =
            AvaloniaProperty.Register<FlexPanel, AlignItems>(nameof(AlignItems));

        /// <summary>
        /// Defines the <see cref="AlignContent"/> property.
        /// </summary>
        public static readonly StyledProperty<AlignContent> AlignContentProperty =
            AvaloniaProperty.Register<FlexPanel, AlignContent>(nameof(AlignContent));

        /// <summary>
        /// Defines the <see cref="Wrap"/> property.
        /// </summary>
        public static readonly StyledProperty<FlexWrap> WrapProperty =
            AvaloniaProperty.Register<FlexPanel, FlexWrap>(nameof(Wrap), FlexWrap.Wrap);

        /// <summary>
        /// Defines the <see cref="ColumnSpacing"/> property.
        /// </summary>
        public static readonly StyledProperty<double> ColumnSpacingProperty =
            AvaloniaProperty.Register<FlexPanel, double>(nameof(ColumnSpacing));

        /// <summary>
        /// Defines the <see cref="RowSpacing"/> property.
        /// </summary>
        public static readonly StyledProperty<double> RowSpacingProperty =
            AvaloniaProperty.Register<FlexPanel, double>(nameof(RowSpacing));

        private FlexLayoutState? _state;

        static FlexPanel()
        {
            AffectsMeasure<FlexPanel>(
                DirectionProperty,
                JustifyContentProperty,
                WrapProperty,
                ColumnSpacingProperty,
                RowSpacingProperty);

            AffectsArrange<FlexPanel>(
                AlignItemsProperty,
                AlignContentProperty);

            AffectsParentMeasure<FlexPanel>(Flex.OrderProperty);

            AffectsParentArrange<FlexPanel>(Flex.AlignSelfProperty);
        }

        /// <summary>
        /// Gets or sets the flex direction
        /// </summary>
        public FlexDirection Direction
        {
            get => GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }

        /// <summary>
        /// Gets or sets the flex justify content mode
        /// </summary>
        public JustifyContent JustifyContent
        {
            get => GetValue(JustifyContentProperty);
            set => SetValue(JustifyContentProperty, value);
        }

        /// <summary>
        /// Gets or sets the flex align items mode
        /// </summary>
        public AlignItems AlignItems
        {
            get => GetValue(AlignItemsProperty);
            set => SetValue(AlignItemsProperty, value);
        }

        /// <summary>
        /// Gets or sets the flex align content mode
        /// </summary>
        public AlignContent AlignContent
        {
            get => GetValue(AlignContentProperty);
            set => SetValue(AlignContentProperty, value);
        }

        /// <summary>
        /// Gets or sets the flex wrap mode
        /// </summary>
        public FlexWrap Wrap
        {
            get => GetValue(WrapProperty);
            set => SetValue(WrapProperty, value);
        }

        /// <summary>
        /// Gets or sets the column spacing
        /// </summary>
        public double ColumnSpacing
        {
            get => GetValue(ColumnSpacingProperty);
            set => SetValue(ColumnSpacingProperty, value);
        }

        /// <summary>
        /// Gets or sets the row spacing
        /// </summary>
        public double RowSpacing
        {
            get => GetValue(RowSpacingProperty);
            set => SetValue(RowSpacingProperty, value);
        }

        /// <inheritdoc />
        protected override Size MeasureOverride(Size availableSize)
        {
            var layout = this;
            var children = (IReadOnlyList<Layoutable>)Children;

            var isColumn = layout.Direction == FlexDirection.Column || layout.Direction == FlexDirection.ColumnReverse;
            var even = layout.JustifyContent == JustifyContent.SpaceEvenly ? 2 : 0;

            var max = Uv.FromSize(availableSize, isColumn);
            var spacing = Uv.FromSize(layout.ColumnSpacing, layout.RowSpacing, isColumn);

            var u = 0.0;
            var m = 0;

            var v = 0.0;
            var maxV = 0.0;
            var n = 0;

            var sections = new List<Section>();
            var first = 0;

            var i = 0;

            children = children.Where(s_isVisible).OrderBy(s_getOrder).ToArray();

            foreach (var element in children)
            {
                if (element is AvaloniaObject obj)
                {
                    var owner = obj.GetValue(OwnerFlexPanelProperty);

                    if (owner is null)
                    {
                        obj.SetValue(OwnerFlexPanelProperty, layout);
                    }
                    else if (owner != layout)
                    {
                        throw new InvalidOperationException();
                    }
                }

                element.Measure(availableSize);

                var size = Uv.FromSize(element.DesiredSize, isColumn);

                if (layout.Wrap != FlexWrap.NoWrap && u + size.U + (m + even) * spacing.U > max.U)
                {
                    sections.Add(new Section(first, i - 1, u, maxV));

                    u = 0.0;
                    m = 0;

                    v += maxV;
                    maxV = 0.0;
                    n++;

                    first = i;
                }

                if (size.V > maxV)
                {
                    maxV = size.V;
                }

                u += size.U;
                m++;

                i++;
            }

            if (m != 0)
            {
                sections.Add(new Section(first, first + m - 1, u, maxV));
            }

            if (layout.Wrap == FlexWrap.WrapReverse)
            {
                sections.Reverse();
            }

            _state = new FlexLayoutState(children, sections);

            if (sections.Count == 0)
            {
                return default;
            }

            return Uv.ToSize(new Uv(sections.Max(s => s.U + (s.Last - s.First + even) * spacing.U), v + maxV + (sections.Count - 1) * spacing.V), isColumn);
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            var layout = this;

            var isColumn = layout.Direction == FlexDirection.Column || layout.Direction == FlexDirection.ColumnReverse;
            var isReverse = layout.Direction == FlexDirection.RowReverse || layout.Direction == FlexDirection.ColumnReverse;

            var state = _state ?? throw new InvalidOperationException();
            var n = state.Sections.Count;

            var size = Uv.FromSize(finalSize, isColumn);
            var spacing = Uv.FromSize(layout.ColumnSpacing, layout.RowSpacing, isColumn);

            var totalSectionV = 0.0;

            foreach (var section in state.Sections)
            {
                totalSectionV += section.V;
            }

            var totalSpacingV = (n - 1) * spacing.V;

            var totalV = totalSectionV + totalSpacingV;

            var spacingV = layout.AlignContent switch
            {
                AlignContent.FlexStart => spacing.V,
                AlignContent.FlexEnd => spacing.V,
                AlignContent.Center => spacing.V,
                AlignContent.Stretch => spacing.V,
                AlignContent.SpaceBetween => spacing.V + (size.V - totalV) / (n - 1),
                AlignContent.SpaceAround => (size.V - totalSectionV) / n,
                AlignContent.SpaceEvenly => (size.V - totalSectionV) / (n + 1),
                _ => throw new NotImplementedException()
            };

            var scaleV = layout.AlignContent == AlignContent.Stretch ? ((size.V - totalSpacingV) / totalSectionV) : 1.0;

            var v = layout.AlignContent switch
            {
                AlignContent.FlexStart => 0.0,
                AlignContent.FlexEnd => size.V - totalV,
                AlignContent.Center => (size.V - totalV) / 2,
                AlignContent.Stretch => 0,
                AlignContent.SpaceBetween => 0.0,
                AlignContent.SpaceAround => spacingV / 2,
                AlignContent.SpaceEvenly => spacingV,
                _ => throw new NotImplementedException()
            };

            foreach (var section in state.Sections)
            {
                var sectionV = scaleV * section.V;

                var (spacingU, u) = layout.JustifyContent switch
                {
                    JustifyContent.FlexStart => (spacing.U, 0.0),
                    JustifyContent.FlexEnd => (spacing.U, size.U - section.U - (section.Last - section.First) * spacing.U),
                    JustifyContent.Center => (spacing.U, (size.U - section.U - (section.Last - section.First) * spacing.U) / 2),
                    JustifyContent.SpaceBetween => ((size.U - section.U) / (section.Last - section.First), 0.0),
                    JustifyContent.SpaceAround => (spacing.U, (size.U - section.U - (section.Last - section.First) * spacing.U) / 2),
                    JustifyContent.SpaceEvenly => ((size.U - section.U) / (section.Last - section.First + 2), (size.U - section.U) / (section.Last - section.First + 2)),
                    _ => throw new NotImplementedException()
                };

                for (int i = section.First; i <= section.Last; i++)
                {
                    var element = state.Children[i];
                    var elementSize = Uv.FromSize(element.DesiredSize, isColumn);

                    var align = layout.AlignItems;

                    if (element is { } layoutable)
                    {
                        align = Flex.GetAlignSelf(layoutable) ?? align;
                    }

                    double finalV = align switch
                    {
                        AlignItems.FlexStart => v,
                        AlignItems.FlexEnd => v + sectionV - elementSize.V,
                        AlignItems.Center => v + (sectionV - elementSize.V) / 2,
                        AlignItems.Stretch => v,
                        _ => throw new NotImplementedException()
                    };

                    if (align == AlignItems.Stretch)
                    {
                        elementSize = new Uv(elementSize.U, sectionV);
                    }

                    var position = new Uv(isReverse ? (size.U - elementSize.U - u) : u, finalV);

                    element.Arrange(new Rect(Uv.ToPoint(position, isColumn), Uv.ToSize(elementSize, isColumn)));

                    u += elementSize.U + spacingU;
                }

                v += sectionV + spacingV;
            }

            return finalSize;
        }

        private struct FlexLayoutState
        {
            public FlexLayoutState(IReadOnlyList<Layoutable> children, IReadOnlyList<Section> sections)
            {
                Children = children;
                Sections = sections;
            }

            public IReadOnlyList<Layoutable> Children { get; }

            public IReadOnlyList<Section> Sections { get; }
        }

        private struct Section
        {
            public Section(int first, int last, double u, double v)
            {
                First = first;
                Last = last;
                U = u;
                V = v;
            }

            public int First { get; }

            public int Last { get; }

            public double U { get; }

            public double V { get; }
        }
    }
}
