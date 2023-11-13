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

            AffectsParentMeasure<FlexPanel>(
                Flex.OrderProperty,
                Flex.ShrinkProperty,
                Flex.GrowProperty);

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

            var max = Uv.FromSize(availableSize, isColumn);
            var spacing = Uv.FromSize(layout.ColumnSpacing, layout.RowSpacing, isColumn);

            var (u, shrink, grow) = (0.0, 0.0, 0.0);
            var m = 0;

            var v = 0.0;
            var maxV = 0.0;
            var n = 0;

            var lines = new List<FlexLine>();
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

                if (layout.Wrap != FlexWrap.NoWrap && u + size.U + m * spacing.U > max.U)
                {
                    lines.Add(new FlexLine(first, i - 1, u, maxV, shrink, grow));

                    (u, shrink, grow) = (0.0, 0.0, 0.0);
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
                shrink += Flex.GetShrink(element);
                grow += Flex.GetGrow(element);
                m++;

                i++;
            }

            if (m != 0)
            {
                lines.Add(new FlexLine(first, first + m - 1, u, maxV, shrink, grow));
            }

            if (layout.Wrap == FlexWrap.WrapReverse)
            {
                lines.Reverse();
            }

            _state = new FlexLayoutState(children, lines);

            if (lines.Count == 0)
            {
                return default;
            }

            var panelSize = new Uv(
                lines.Max(line => line.U + (line.Count - 1) * spacing.U),
                v + maxV + (lines.Count - 1) * spacing.V);
            return Uv.ToSize(panelSize, isColumn);
        }

        /// <inheritdoc />
        protected override Size ArrangeOverride(Size finalSize)
        {
            var layout = this;

            var isColumn = layout.Direction == FlexDirection.Column || layout.Direction == FlexDirection.ColumnReverse;
            var isReverse = layout.Direction == FlexDirection.RowReverse || layout.Direction == FlexDirection.ColumnReverse;

            var state = _state ?? throw new InvalidOperationException();

            var size = Uv.FromSize(finalSize, isColumn);
            var spacing = Uv.FromSize(layout.ColumnSpacing, layout.RowSpacing, isColumn);

            var n = state.Lines.Count;
            var totalLineV = state.Lines.Sum(s => s.V);
            var totalSpacingV = (n - 1) * spacing.V;
            var totalV = totalLineV + totalSpacingV;
            var freeV = size.V - totalV;

            var alignContent = freeV >= 0.0 ? layout.AlignContent : layout.AlignContent switch
            {
                AlignContent.FlexStart or AlignContent.Stretch or AlignContent.SpaceBetween => AlignContent.FlexStart,
                AlignContent.Center or AlignContent.SpaceAround or AlignContent.SpaceEvenly => AlignContent.Center,
                AlignContent.FlexEnd => AlignContent.FlexEnd,
                _ => throw new NotImplementedException()
            };

            var (spacingV, v) = alignContent switch
            {
                AlignContent.FlexStart => (spacing.V, 0.0),
                AlignContent.FlexEnd => (spacing.V, freeV),
                AlignContent.Center => (spacing.V, freeV / 2),
                AlignContent.Stretch => (spacing.V, 0.0),
                AlignContent.SpaceBetween => (spacing.V + freeV / (n - 1), 0.0),
                AlignContent.SpaceAround => (spacing.V + freeV / n, freeV / n / 2),
                AlignContent.SpaceEvenly => (spacing.V + freeV / (n + 1), freeV / (n + 1)),
                _ => throw new NotImplementedException()
            };

            var scaleV = alignContent == AlignContent.Stretch ? (size.V - totalSpacingV) / totalLineV : 1.0;

            foreach (var line in state.Lines)
            {
                var m = line.Count;
                var lineV = scaleV * line.V;
                var totalSpacingU = (m - 1) * spacing.U;
                var totalU = line.U + totalSpacingU;
                var freeU = size.U - totalU;

                var (spacingU, u) = line.Grow > 0 ? (spacing.U, 0.0) : layout.JustifyContent switch
                {
                    JustifyContent.FlexStart => (spacing.U, 0.0),
                    JustifyContent.FlexEnd => (spacing.U, freeU),
                    JustifyContent.Center => (spacing.U, freeU / 2),
                    JustifyContent.SpaceBetween => (spacing.U + freeU / (m - 1), 0.0),
                    JustifyContent.SpaceAround => (spacing.U + freeU / m, freeU / m / 2),
                    JustifyContent.SpaceEvenly => (spacing.U + freeU / (m + 1), freeU / (m + 1)),
                    _ => throw new NotImplementedException()
                };

                for (int i = line.First; i <= line.Last; i++)
                {
                    var element = state.Children[i];
                    var elementSize = Uv.FromSize(element.DesiredSize, isColumn);
                    var elementShrink = Flex.GetShrink(element);
                    var elementGrow = Flex.GetGrow(element);

                    var align = layout.AlignItems;
                    if (element is { } layoutable)
                    {
                        align = Flex.GetAlignSelf(layoutable) ?? align;
                    }

                    double finalV = align switch
                    {
                        AlignItems.FlexStart => v,
                        AlignItems.FlexEnd => v + lineV - elementSize.V,
                        AlignItems.Center => v + (lineV - elementSize.V) / 2,
                        AlignItems.Stretch => v,
                        _ => throw new NotImplementedException()
                    };

                    if (align == AlignItems.Stretch)
                    {
                        elementSize = new Uv(elementSize.U, lineV);
                    }
                    if (freeU > 0 && line.Grow > 0 && elementGrow > 0)
                    {
                        elementSize = new Uv(elementSize.U + freeU * elementGrow / line.Grow, elementSize.V);
                    }
                    else if (freeU < 0 && line.Shrink > 0 && elementShrink > 0)
                    {
                        elementSize = new Uv(Math.Max(0.0, elementSize.U + freeU * elementShrink / line.Shrink), elementSize.V);
                    }

                    var position = new Uv(isReverse ? (size.U - elementSize.U - u) : u, finalV);

                    element.Arrange(new Rect(Uv.ToPoint(position, isColumn), Uv.ToSize(elementSize, isColumn)));

                    u += elementSize.U + spacingU;
                }

                v += lineV + spacingV;
            }

            return finalSize;
        }

        private struct FlexLayoutState
        {
            public FlexLayoutState(IReadOnlyList<Layoutable> children, IReadOnlyList<FlexLine> lines)
            {
                Children = children;
                Lines = lines;
            }

            public IReadOnlyList<Layoutable> Children { get; }

            public IReadOnlyList<FlexLine> Lines { get; }
        }

        private readonly struct FlexLine
        {
            public FlexLine(int first, int last, double u, double v, double shrink, double grow)
            {
                First = first;
                Last = last;
                U = u;
                V = v;
                Shrink = shrink;
                Grow = grow;
            }

            public int First { get; }

            public int Last { get; }

            public double U { get; }

            public double V { get; }

            public double Shrink { get; }

            public double Grow { get; }

            public int Count => Last - First + 1;
        }
    }
}
