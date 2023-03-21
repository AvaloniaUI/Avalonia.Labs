using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Avalonia.Labs.Controls
{
    public class TabbedPage : MultiPage
    {
        private TabControl? _tabControl;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            _tabControl = e.NameScope.Get<TabControl>("PART_TabControl");

            if(_tabControl != null )
            {
                _tabControl.HeaderTemplate = new TabbedPageHeaderTemplate();

                UpdateActivePage();
            }
        }

        protected override void UpdateActivePage()
        {
            if(_tabControl != null )
            {
                ActiveChildPage = _tabControl.SelectedItem as Page;
            }
        }

        protected override void UpdatePresenterPadding()
        {
            base.UpdatePresenterPadding();
            
            if(_tabControl != null)
            {
                _tabControl.Margin = new Thickness(SafeAreaPadding.Left, SafeAreaPadding.Top, SafeAreaPadding.Right, 0);

                if(ActiveChildPage != null )
                {
                    ActiveChildPage.SafeAreaPadding = new Thickness(SafeAreaPadding.Left, 0, SafeAreaPadding.Right, SafeAreaPadding.Bottom);
                }
            }
        }
    }
}
