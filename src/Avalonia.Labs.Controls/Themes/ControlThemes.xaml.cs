using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using System;
using System.Collections.Generic;

namespace Avalonia.Labs.Controls
{
    public enum Mode
    {
        Fluent,
        Simple
    }

    /// <summary>
    /// Includes the labs control themes in an application.
    /// </summary>
    public class ControlThemes : Styles
    {
       // private readonly IResourceDictionary _fluent;
       // private readonly IResourceDictionary _simple;


        public static readonly StyledProperty<Mode> ModeProperty =
            AvaloniaProperty.Register<ControlThemes, Mode>(nameof(Mode));

        /// <summary>
        /// Initializes a new instance of the <see cref="ControlThemes"/> class.
        /// </summary>
        /// <param name="sp">The parent's service provider.</param>
        public ControlThemes(IServiceProvider? sp = null)
        {
            AvaloniaXamlLoader.Load(sp, this);

           // _fluent = (IResourceDictionary)GetAndRemove("ResourcesFluent");
           // _simple = (IResourceDictionary)GetAndRemove("ResourcesSimple");

            //EnsureThemeVariants();

            object GetAndRemove(string key)
            {
                var val = Resources[key]
                          ?? throw new KeyNotFoundException($"Key {key} was not found in the resources");
                Resources.Remove(key);
                return val;
            }
        }

        public Mode Mode
        {
            get => GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == ModeProperty)
            {
                EnsureThemeVariants();
            }
        }

        private void EnsureThemeVariants()
        {
           /* var themeVariantResource = Mode == Mode.Fluent ? _fluent : _simple;
            var dict = Resources.MergedDictionaries;
            if (dict.Count == 0)
            {
                dict.Add(themeVariantResource);
            }
            else
            {
                dict[0] = themeVariantResource;
            }*/
        }
    }
}
