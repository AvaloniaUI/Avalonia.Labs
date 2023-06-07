using System;
using System.Threading.Tasks;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Styling;

namespace Avalonia.Labs.Controls;

static internal class AnimationHelpers
{
    private static readonly Easing DefaultEasing = new LinearEasing();

    public static Task BeginAnimation<T>(this Animatable control
        , StyledProperty<T> property
        , TimeSpan duration
        , T to
        , Easing? easing = default
        )
    {
        var from = control.GetValue<T>(property);
        return BeginAnimation(control, property, duration, from, to, easing);
    }

    public static Task BeginAnimation<T>(this Animatable control
        , StyledProperty<T> property
        , TimeSpan duration
        , T from
        , T to
        , Easing? easing = default
        )
    {
        var animation = new Animation.Animation()
        {
            Easing = easing ?? DefaultEasing,
            Duration = duration,
            PlaybackDirection = PlaybackDirection.Normal,
            FillMode = FillMode.Both,
            Children =
            {
                new()
                {
                    Cue = default,
                    Setters =
                    {
                        new Setter(property, from),
                    }
                },
                new()
                {
                    Cue = new Cue(1),
                    Setters =
                    {
                        new Setter(property, to),
                    }
                }
            }
        };
        return animation.RunAsync(control);
    }
}
