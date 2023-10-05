using System;
using Avalonia.Controls;

namespace Avalonia.Labs.Catalog.Views
{
    public partial class WelcomeView : ScrollViewer
    {
        public WelcomeView()
        {
            InitializeComponent();
        }

        protected override Type StyleKeyOverride => typeof(ScrollViewer);
    }
}
