using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Labs.Catalog.Views;
using Avalonia.Labs.Qr;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using static Avalonia.Labs.Qr.QrCode;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public partial class QrViewModel : ViewModelBase
    {
        private const string Chars = "qwertyuiopasdfghjklzxcvbnm";

        [ObservableProperty]
        public partial string? QrCodeString { get; set; }

        [ObservableProperty]
        public partial Thickness QrCodePadding { get; set; } = new Thickness(10);

        [ObservableProperty]
        public partial double QrCodeSize { get; set; } = 250;

        [ObservableProperty]
        public partial CornerRadius QrCodeCornerRadius { get; set; } = new CornerRadius(12);

        [ObservableProperty]
        public partial QrCode.EccLevel QrCodeEccLevel { get; set; }

        [ObservableProperty]
        public partial Color QrCodeForegroundColor1 { get; set; }

        [ObservableProperty]
        public partial Color QrCodeForegroundColor2 { get; set; }

        [ObservableProperty]
        public partial Color QrCodeBackgroundColor1 { get; set; }

        [ObservableProperty]
        public partial Color QrCodeBackgroundColor2 { get; set; }

        public ObservableCollection<EccLevel> Levels { get; }

        static QrViewModel()
        {
            ViewLocator.Register(typeof(QrViewModel), () => new QrView());
        }

        public QrViewModel()
        {
            ResetQrCode();
            Title = "Qr Generator";

            Levels = new ObservableCollection<EccLevel>(Enum.GetValues<EccLevel>());
        }

        public void UpdateQrCode(string? text)
        {
            if (string.IsNullOrEmpty(text))
                text = "You didn't put anything here?";
            QrCodeString = text;
        }

        public void RandomizeData()
        {
            UpdateQrCode(string.Join("", Enumerable.Range(0, 150).Select(_ => Chars[Random.Shared.Next(0, Chars.Length)])));
        }

        public void RandomizeColors()
        {
            var newColors = new byte[12];
            Random.Shared.NextBytes(newColors);

            QrCodeForegroundColor1 = Color.FromRgb(newColors[0], newColors[1], newColors[2]);
            QrCodeForegroundColor2 = Color.FromRgb(newColors[3], newColors[4], newColors[5]);

            QrCodeBackgroundColor1 = Color.FromRgb(newColors[6], newColors[7], newColors[8]);
            QrCodeBackgroundColor2 = Color.FromRgb(newColors[9], newColors[10], newColors[11]);

            var cuurentCode = QrCodeString;
            QrCodeString = string.Empty;

            UpdateQrCode(cuurentCode);
        }

        public void ResetQrCode()
        {
            QrCodeEccLevel = QrCode.EccLevel.Medium;

            QrCodeString = "I'm a very long text that you might find somewhere as a link or something else.  It's rendered with smooth edges and gradients for the foreground and background";

            QrCodeForegroundColor1 = Colors.Navy;
            QrCodeForegroundColor2 = Colors.DarkRed;
            QrCodeBackgroundColor1 = Colors.White;
            QrCodeBackgroundColor2 = Colors.White;
        }
    }
}
