using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Labs.Catalog.ViewModels;

namespace Avalonia.Labs.Catalog.Views
{
    public partial class GifView : UserControl
    {
        public GifView()
        {
            InitializeComponent();

            AddHandler(DragDrop.DragOverEvent, DragOver);
            AddHandler(DragDrop.DropEvent, Drop);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            Focus();
        }

        private void DragOver(object? sender, DragEventArgs e)
        {
            e.DragEffects &= DragDropEffects.Copy | DragDropEffects.Link;

            if (!(e.DataTransfer.TryGetFiles()?.Length > 0))
            {
                e.DragEffects = DragDropEffects.None;
            }
        }

        private void Drop(object? sender, DragEventArgs e)
        {
            if (!(e.DataTransfer.TryGetFiles()?.Length > 0))
            {
                return;
            }

            if (DataContext is not LottieViewModel vm)
            {
                return;
            }

            var paths = e.DataTransfer.TryGetFiles()?.ToList();
            if (paths is null)
            {
                return;
            }

            for (var i = 0; i < paths.Count; i++)
            {
                var path = paths[i];
                if (string.IsNullOrWhiteSpace(path.Name))
                {
                    continue;
                }

                vm.Add(path.Path.ToString());

                if (i == 0)
                {
                    vm.SelectedAsset = vm.Assets[^1];
                }
            }
        }
    }
}
