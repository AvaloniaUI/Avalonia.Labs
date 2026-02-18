using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Avalonia.Markup.Xaml;

namespace Avalonia.Labs.Controls
{
    public class ResourceMarkupExtension : MarkupExtension
    {
        public ResourceItem? Key { get; set; }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {

            return Key?.Open() ?? new MemoryStream();
        }
    }
}
