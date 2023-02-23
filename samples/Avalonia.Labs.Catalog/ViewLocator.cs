using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Labs.Catalog.ViewModels;

namespace Avalonia.Labs.Catalog;

public class ViewLocator : IDataTemplate
{
    private static readonly Dictionary<Type, Func<Control>> ViewModelRegistry = new Dictionary<Type, Func<Control>>();

    public static void Register(Type type, Func<Control> factory)
    {
        ViewModelRegistry.TryAdd(type, factory);
    }

    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        var type = data?.GetType();

        if (type != null && ViewModelRegistry.TryGetValue(type, out var factory))
        {
            return factory();
        }
        
        return new TextBlock { Text = type?.FullName };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
