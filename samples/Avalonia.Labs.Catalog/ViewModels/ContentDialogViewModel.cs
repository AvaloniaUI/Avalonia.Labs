using System.Threading.Tasks;
using System;
using Avalonia.Labs.Catalog.Views;
using Avalonia.Labs.Controls;
using ReactiveUI;

namespace Avalonia.Labs.Catalog.ViewModels;

internal enum DialogMode
{
    Default,
    Deferal,
    Input,
}

internal class ContentDialogViewModel : ViewModelBase
{
    static ContentDialogViewModel()
    {
        ViewLocator.Register(typeof(ContentDialogViewModel), () => new ContentDialogView());
    }

    public ContentDialogViewModel()
    {
        Title = "ContentDialog";
    }

    public string? DialogTitle
    {
        get => _dialogTitle;
        set => this.RaiseAndSetIfChanged(ref _dialogTitle, value);
    }

    public string? Content
    {
        get => _content;
        set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    public string PrimaryButtonText
    {
        get => _primaryText;
        set => this.RaiseAndSetIfChanged(ref _primaryText, value);
    }

    public string SecondaryButtonText
    {
        get => _secondaryText;
        set => this.RaiseAndSetIfChanged(ref _secondaryText, value);
    }

    public string CloseButtonText
    {
        get => _closeText;
        set => this.RaiseAndSetIfChanged(ref _closeText, value);
    }

    public bool FullSizeDesired
    {
        get => _fullSize;
        set => this.RaiseAndSetIfChanged(ref _fullSize, value);
    }

    public ContentDialogButton[] ContentDialogButtons { get; } = Enum.GetValues<ContentDialogButton>();

    public ContentDialogButton ContentDialogDefaultButton
    {
        get => _defaultButton;
        set => this.RaiseAndSetIfChanged(ref _defaultButton, value);
    }

    public bool IsPrimaryButtonEnabled
    {
        get => _primaryEnabled;
        set => this.RaiseAndSetIfChanged(ref _primaryEnabled, value);
    }

    public bool IsSecondaryButtonEnabled
    {
        get => _secondaryEnabled;
        set => this.RaiseAndSetIfChanged(ref _secondaryEnabled, value);
    }


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
                    CloseButtonText = "Leave me alone!"
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
                    FullSizeDesired = FullSizeDesired
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




    //private string _title = "Title Here";
    private string _content = "Dialog message here (doesn't have to be a string, can be anything)";
    private string _primaryText = "Primary Button";
    private string _secondaryText = "Secondary Button";
    private string _closeText = "Close Button";
    private bool _fullSize;
    private ContentDialogButton _defaultButton;
    private bool _primaryEnabled = true;
    private bool _secondaryEnabled = true;
    private string? _dialogTitle = "Title Here";
}
