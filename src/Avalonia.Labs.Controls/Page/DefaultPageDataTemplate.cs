using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace Avalonia.Labs.Controls
{
    internal class DefaultPageDataTemplate : IDataTemplate
    {
        public Control? Build(object? param)
        {
            return param as Page ?? new ContentPage()
            {
                Content = param
            };
        }

        public bool Match(object? data)
        {
            return data is not null;
        }
    }
}
