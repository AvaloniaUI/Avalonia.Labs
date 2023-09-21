namespace Avalonia.Labs.Controls
{
    public interface INavigation
    {
        object? Pop();
        void Push(object page);
    }
}
