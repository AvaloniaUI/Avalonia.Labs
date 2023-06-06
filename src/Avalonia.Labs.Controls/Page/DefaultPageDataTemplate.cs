using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Templates;
using Avalonia.VisualTree;

namespace Avalonia.Labs.Controls
{
    internal class DefaultPageDataTemplate : IRecyclingDataTemplate
    {
        public Control? Build(object? param)
        {
            if(param is Page page)
                return page;
            if(param is Control control)
            {
                var visualParent = control.GetVisualParent() as ContentPresenter;
                if (visualParent != null)
                    visualParent.Content = null;
            }
            return new ContentPage()
            {
                Content = param
            };
        }

        public Control? Build(object? data, Control? existing)
        {
            if(existing != null)
            {

            }

            return Build(data);
        }

        public bool Match(object? data)
        {
            return data is not null;
        }
    }
}
