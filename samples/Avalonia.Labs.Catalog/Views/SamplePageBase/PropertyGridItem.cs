using Avalonia.Controls.Templates;

namespace Avalonia.Labs.Catalog.Views.SamplePageBase;

public class PropertyGridItem(AvaloniaProperty targetProperty, AvaloniaObject targetObject, string? category = null, string? name = null, IDataTemplate? valueTemplate = null, IDataTemplate? valueEditingTemplate = null)
{
    public string? Category { get; } = category;
    public string? Name { get; } = name ?? targetProperty.Name;
    public AvaloniaProperty TargetProperty { get; } = targetProperty;
    public IDataTemplate? ValueTemplate { get; } = valueTemplate ?? new ValueTemplate();
    public IDataTemplate? ValueEditingTemplate { get; } = valueEditingTemplate;
    public AvaloniaObject TargetObject { get; } = targetObject;
}
