using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using Avalonia.Collections;
using Avalonia.Platform;

namespace Avalonia.Labs.Controls
{
    public class ResourceItem
    {
        public ResourceItem(string name, Uri uri)
        {
            Name = name;
            Uri = uri;
        }

        public string Name { get; }
        public Uri Uri { get; }

        public Stream Open()
        {
            return AssetLoader.Open(Uri);
        }
    }
}
