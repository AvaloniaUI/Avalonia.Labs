
![Labs Header](https://github.com/AvaloniaUI/Avalonia.Labs/assets/552074/b9a462fc-8cb7-437b-9023-07e44ab0aabd)

# Avalonia Labs

Experimental Controls for [Avalonia](https://github.com/AvaloniaUI/Avalonia).

This repository serves as a staging ground for new controls for [Avalonia](https://github.com/AvaloniaUI/Avalonia), with the intention of including them in the core AvaloniaUI controls. The controls available here are unstable and are suspected to breaking changes as they are being worked on.

## Usage

Add the `Avalonia.Labs.Controls.ControlThemes` theme after your main app theme in `App.xaml.cs`.

```xaml
<Application xmlns="https://github.com/avaloniaui"
             xmlns:labs="using:Avalonia.Labs.Controls"
             ...
             />
    <Application.Styles>
        <FluentTheme/>
        <labs:ControlThemes/>
    </Application.Styles>
```

## Available Controls

 - [AsyncImage](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Controls/AsyncImage)
 - [FlexPanel](https://github.com/AvaloniaUI/Avalonia.Labs/blob/main/src/Avalonia.Labs.Panels/FlexPanel.cs)
 - [FlipView](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Controls/FlipView)
 - [InfoBadge](/src/Avalonia.Labs.Controls/InfoBadge/Readme.md)
 - [GIF image renderer and player](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Gif)
 - [Lottie Viewer](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Lottie)
 - [NavigationControl](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Controls/Navigation)
 - [Qr Code Generator](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Qr)
 - [RoutedCommand](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.RoutedCommand)
 - [StepBar](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Controls/StepBar)
 - [SKCanvasView](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Controls/SKCanvasView)
 - [SwipeView](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Controls/SwipeView)
 - [TabControl](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Controls/TabLayout)

## Nightly Builds
Nightly builds are published on the Avalonia Nightly feed. Instructions for using them can be found here https://github.com/AvaloniaUI/Avalonia/wiki/Using-nightly-build-feed
