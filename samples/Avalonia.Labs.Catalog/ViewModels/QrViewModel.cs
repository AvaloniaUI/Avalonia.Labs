using System;
using System.Linq;
using Avalonia.Labs.Catalog.Views;
using Avalonia.Media;
using Avalonia.Labs.Qr;
using ReactiveUI;
using System.Collections.ObjectModel;
using static Avalonia.Labs.Qr.QrCode;

namespace Avalonia.Labs.Catalog.ViewModels
{
    public class QrViewModel : ViewModelBase
    {
        private const string Chars = "qwertyuiopasdfghjklzxcvbnm";
        private string? _qrCodeString;

        private double _qrCodeSize = 250;

        private Thickness _qrCodePadding = new(10);

        private CornerRadius _qrCodeCornerRadius = new(12);

        private Color _qrCodeForegroundColor1;
        private Color _qrCodeForegroundColor2;

        private Color _qrCodeBackgroundColor1;
        private Color _qrCodeBackgroundColor2;

        private QrCode.EccLevel _qrCodeEccLevel;

        public string? QrCodeString
        {
            get => _qrCodeString;
            set => this.RaiseAndSetIfChanged(ref _qrCodeString, value);
        }

        public Thickness QrCodePadding
        {
            get => _qrCodePadding;
            set => this.RaiseAndSetIfChanged(ref _qrCodePadding, value);
        }

        public double QrCodeSize
        {
            get => _qrCodeSize;
            set => this.RaiseAndSetIfChanged(ref _qrCodeSize, value);
        }

        public CornerRadius QrCodeCornerRadius
        {
            get => _qrCodeCornerRadius;
            set => this.RaiseAndSetIfChanged(ref _qrCodeCornerRadius, value);
        }

        public QrCode.EccLevel QrCodeEccLevel
        {
            get => _qrCodeEccLevel;
            set => this.RaiseAndSetIfChanged(ref _qrCodeEccLevel, value);
        }

        public Color QrCodeForegroundColor1
        {
            get => _qrCodeForegroundColor1;
            set => this.RaiseAndSetIfChanged(ref _qrCodeForegroundColor1, value);
        }

        public Color QrCodeForegroundColor2
        {
            get => _qrCodeForegroundColor2;
            set => this.RaiseAndSetIfChanged(ref _qrCodeForegroundColor2, value);
        }

        public Color QrCodeBackgroundColor1
        {
            get => _qrCodeBackgroundColor1;
            set => this.RaiseAndSetIfChanged(ref _qrCodeBackgroundColor1, value);
        }

        public Color QrCodeBackgroundColor2
        {
            get => _qrCodeForegroundColor2;
            set => this.RaiseAndSetIfChanged(ref _qrCodeBackgroundColor2, value);
        }

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

        public void UpdateQrCode(string text)
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
