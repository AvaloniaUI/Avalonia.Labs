using Avalonia.Media;

namespace Avalonia.Labs.Lottie;

internal record struct LottiePayload(
    LottieCommand LottieCommand,
    SkiaSharp.Skottie.Animation? Animation = null,
    Stretch? Stretch = null,
    StretchDirection? StretchDirection = null,
    int? RepeatCount = null);
