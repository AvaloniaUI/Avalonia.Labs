using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Labs.Catalog.ViewModels;
using Avalonia.Markup.Xaml;

namespace Avalonia.Labs.Catalog.Views;

public partial class ContentDialogView : UserControl
{
    public ContentDialogView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if(DataContext is ContentDialogViewModel vm)
        {
            vm.TopLevel = TopLevel.GetTopLevel(this);
        }
    }
}
