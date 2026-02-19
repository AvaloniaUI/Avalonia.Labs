using System;
using System.Threading.Tasks;
using Avalonia.Labs.Catalog.Views;
using Avalonia.Labs.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.Labs.Catalog.ViewModels;

internal enum DialogMode
{
    Default,
    Deferal,
    Input,
}

public partial class ContentDialogViewModel : ViewModelBase
{
    static ContentDialogViewModel()
    {
        ViewLocator.Register(typeof(ContentDialogViewModel), () => new ContentDialogView());
    }

    public ContentDialogViewModel()
    {
        Title = "ContentDialog";
    }

    [ObservableProperty]
    public partial string? DialogTitle { get; set; } = "Title Here";

    [ObservableProperty]
    public partial string? Content { get; set; } = "Dialog message here (doesn't have to be a string, can be anything)";

    [ObservableProperty]
    public partial string? PrimaryButtonText { get; set; } = "Primary Button";

    [ObservableProperty]
    public partial string? SecondaryButtonText { get; set; } = "Secondary Button";

    [ObservableProperty]
    public partial string? CloseButtonText { get; set; } = "Close Button";

    [ObservableProperty]
    public partial bool FullSizeDesired { get; set; }

    public ContentDialogButton[] ContentDialogButtons { get; } = Enum.GetValues<ContentDialogButton>();

    [ObservableProperty]
    public partial ContentDialogButton ContentDialogDefaultButton { get; set; }

    public ContentDialogButtonsOrder[] ContentDialogButtonsOrders { get; } = Enum.GetValues<ContentDialogButtonsOrder>();

    [ObservableProperty]
    public partial ContentDialogButtonsOrder ContentDialogButtonsOrder { get; set; }

    [ObservableProperty]
    public partial bool IsPrimaryButtonEnabled { get; set; } = true;

    [ObservableProperty]
    public partial bool IsSecondaryButtonEnabled { get; set; } = true;


    public async void LaunchDialog(object? parameter)
    {
        bool hasDeferral = false;
        ContentDialog? dialog = default;
        switch (parameter)
        {
            case DialogMode.Input:
                dialog = new ContentDialog()
                {
                    Title = "Let's go ...",
                    PrimaryButtonText = "Ok :-)",
                    SecondaryButtonText = "Not OK :-(",
                    CloseButtonText = "Leave me alone!",
                    ButtonsOrder = ContentDialogButtonsOrder
                };

                var viewModel = new CustomContentDialogViewModel(dialog);
                dialog.Content = new ContentDialogInputExample()
                {
                    DataContext = viewModel
                };
                break;
            case DialogMode.Deferal:
            default:
                dialog = new ContentDialog
                {
                    PrimaryButtonText = PrimaryButtonText,
                    SecondaryButtonText = SecondaryButtonText,
                    CloseButtonText = CloseButtonText,
                    Title = DialogTitle,
                    Content = Content,
                    IsPrimaryButtonEnabled = IsPrimaryButtonEnabled,
                    IsSecondaryButtonEnabled = IsSecondaryButtonEnabled,
                    DefaultButton = ContentDialogDefaultButton,
                    FullSizeDesired = FullSizeDesired,
                    ButtonsOrder = ContentDialogButtonsOrder
                };
                hasDeferral = parameter is DialogMode.Deferal;
                break;
        }

        if (hasDeferral)
        {
            dialog.PrimaryButtonClick += OnPrimaryButtonClick;
            await dialog.ShowAsync();
            dialog.PrimaryButtonClick -= OnPrimaryButtonClick;
        }
        else
        {
            await dialog.ShowAsync();
        }
    }

    private async void OnPrimaryButtonClick(object? sender, ContentDialogButtonClickEventArgs args)
    {
        var def = args.GetDeferral();
        await Task.Delay(3000);
        def.Complete();
    }
}
