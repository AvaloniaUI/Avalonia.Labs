using System;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Avalonia.Labs.Controls
{
    public class AnimatedScrollViewer : ScrollViewer
    {
        protected override Type StyleKeyOverride => typeof(ScrollViewer);

        public readonly static StyledProperty<bool> IsAnimatedProperty = AvaloniaProperty.Register<AnimatedScrollViewer, bool>(nameof(IsAnimated), true);
        private VectorTransition? _offsetTransitions;

        public bool IsAnimated
        {
            get => GetValue(IsAnimatedProperty);
            set => SetValue(IsAnimatedProperty, value);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _offsetTransitions = new VectorTransition()
            {
                Property = OffsetProperty,
                Duration = new TimeSpan(0, 0, 0, 0, 250)
            };

            Transitions = new Transitions();

            if (IsAnimated)
            {
                if (Transitions?.Contains(_offsetTransitions) == false)
                {
                    Transitions?.Add(_offsetTransitions);
                }
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == IsAnimatedProperty)
            {
                if (_offsetTransitions != null)
                    if (IsAnimated)
                    {
                        if (Transitions?.Contains(_offsetTransitions) == false)
                        {
                            Transitions?.Add(_offsetTransitions);
                        }
                    }
                    else
                    {
                        Transitions?.Remove(_offsetTransitions);
                    }
            }
        }
    }
}
