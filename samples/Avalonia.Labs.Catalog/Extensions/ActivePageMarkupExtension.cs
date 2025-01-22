using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Labs.Controls;
using Avalonia.Markup.Xaml;

namespace Avalonia.Labs.Catalog.Extensions
{
    internal class ActivePageMarkupExtension : MarkupExtension
    {
        private readonly BindingBase _currentPageBinding;
        private readonly Type _pageType;

        public ActivePageMarkupExtension(BindingBase currentPageBinding, Type page)
        {
            _currentPageBinding = currentPageBinding;
            _pageType = page;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var mb = new MultiBinding()
            {
                Bindings = new[] { _currentPageBinding },
                Converter = new FuncMultiValueConverter<object, bool>(router => router is List<object> list && list.FirstOrDefault()?.GetType() == _pageType)
            };

            return mb;
        }
    }
}
