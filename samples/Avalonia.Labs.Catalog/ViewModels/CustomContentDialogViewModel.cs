using System;
using System.ComponentModel;
using Avalonia.Labs.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Avalonia.Labs.Catalog.ViewModels;

public partial class CustomContentDialogViewModel : ViewModelBase
{
    private readonly ContentDialog dialog;

    public CustomContentDialogViewModel(ContentDialog dialog)
    {
        if (dialog is null)
        {
            throw new ArgumentNullException(nameof(dialog));
        }

        this.dialog = dialog;
        dialog.Closed += DialogOnClosed;
    }

    private void DialogOnClosed(object? sender, ContentDialogClosedEventArgs args)
    {
        dialog.Closed -= DialogOnClosed;

        var resultHint = new ContentDialog()
        {
            Content = $"You chose \"{args.Result}\"",
            Title = "Result",
            PrimaryButtonText = "Thanks"
        };

        _ = resultHint.ShowAsync();
    }

    /// <summary>
    /// Gets or sets the user input to check 
    /// </summary>
    [ObservableProperty]
    public partial string? UserInput { get; set; }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(UserInput))
        {
            HandleUserInput();
        }
    }

    private void HandleUserInput()
    {
        switch (UserInput?.ToLowerInvariant())
        {
            case "accept":
            case "ok":
                dialog.Hide(ContentDialogResult.Primary);
                break;

            case "dismiss":
            case "not ok":
                dialog.Hide(ContentDialogResult.Secondary);
                break;

            case "cancel":
            case "close":
            case "hide":
                dialog.Hide();
                break;
        }
    }

    private static readonly string[] _AvailableKeyWords = new[]
    {
        "Accept",
        "OK",
        "Dismiss",
        "Not OK",
        "Close",
        "Cancel",
        "Hide"
    };

    public string[] AvailableKeyWords => _AvailableKeyWords;
}
