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
 - [FlipView](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Controls/FlipView)
 - [Lottie Viewer](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Lottie)
 - [NavigationControl](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Controls/Navigation)
 - [Qr Code Generator](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Qr)
 - [StepBar](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Controls/StepBar)
 - [SwipeView](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Controls/SwipeView)
 - [TabControl](https://github.com/AvaloniaUI/Avalonia.Labs/tree/main/src/Avalonia.Labs.Controls/TabLayout)
