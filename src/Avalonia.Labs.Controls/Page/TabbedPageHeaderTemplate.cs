using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace Avalonia.Labs.Controls
{
    internal class TabbedPageHeaderTemplate : IDataTemplate
    {
        public Control? Build(object? param)
        {
            if(param is Page page)
            {
                return new Label
                {
                    Content = page.Title
                };
            }
            return null;
        }

        public bool Match(object? data)
        {
            return data is not null;
        }
    }
}
