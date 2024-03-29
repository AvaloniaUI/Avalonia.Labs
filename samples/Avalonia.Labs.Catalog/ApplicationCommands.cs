using System;
using System.Collections.Generic;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Labs.Input;

namespace Avalonia.Labs.Catalog;

public static class ApplicationCommands
{
    private static RoutedCommand? s_open, s_save, s_delete;

    public static RoutedCommand Open => s_open ??= new RoutedCommand(nameof(Open),
        new KeyGesture(Key.O, GetFromHotKeys(h => h.CommandModifiers)));
    public static RoutedCommand Save => s_save ??= new RoutedCommand(nameof(Save),
        new KeyGesture(Key.S, GetFromHotKeys(h => h.CommandModifiers)));
    public static RoutedCommand Delete => s_delete ??= new RoutedCommand(nameof(Delete),
        new KeyGesture(Key.Delete));

    private static T GetFromHotKeys<T>(Func<PlatformHotkeyConfiguration, T> filter)
    {
        var hotkeys = Application.Current?.PlatformSettings!.HotkeyConfiguration
            ?? throw new InvalidOperationException("HotkeyConfiguration was not initialized");
        return filter(hotkeys);
    }
}
