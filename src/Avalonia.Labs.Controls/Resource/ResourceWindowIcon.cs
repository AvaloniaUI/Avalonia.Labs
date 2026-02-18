using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Avalonia.Controls;

namespace Avalonia.Labs.Controls.Resource
{
    [TypeConverter(typeof(ResourceWindowIconTypeConverter))]
    public class ResourceWindowIcon : WindowIcon
    {
        private readonly ResourceItem _item;

        public ResourceWindowIcon(ResourceItem item) : base(item.Open())
        {
            _item = item;
        }
    }
}
