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
 - [Lottie Viewer](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Lottie)
 - [NavigationControl](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Controls/Navigation)
 - [Qr Code Generator](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Qr)
 - [RoutedCommand](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.RoutedCommand)
 - [StepBar](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Controls/StepBar)
 - [SKCanvasView](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Controls/SKCanvasView)
 - [SwipeView](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Controls/SwipeView)
 - [TabControl](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Controls/TabLayout)
