
# AsyncImage
AsycImage is an image control that asynchronously loading an image resource using a Uri. Supported uri schemes include `http`, `https`, `file`,  and`avares`. Relative paths are not supported as `Source`.
A default image resource can be set using the `PlaceholderSource` property. It supports all sources that `Avalonia.Controls.Image` supports.
## How To Use
### Loading an image resource from a web url
Sample:
```xaml
<UserControl xmlns="https://github.com/avaloniaui"             
             xmlns:labs="clr-namespace:Avalonia.Labs.Controls;assembly=Avalonia.Labs.Controls"
             ...>
    <labs:AsyncImage PlaceholderSource="/Assets/avalonia-logo.ico"
                         Width="80"
                         Source="https://avatars.githubusercontent.com/u/14075148?v=4"
                         Height="80"
                         />
</UserControl>
```
### Loading an image resource from a file
Loading a file resource isn't supported on platforms that require a storage provider.
Sample:
```xaml
<UserControl xmlns="https://github.com/avaloniaui"             
             xmlns:labs="clr-namespace:Avalonia.Labs.Controls;assembly=Avalonia.Labs.Controls"
             ...>
    <labs:AsyncImage PlaceholderSource="/Assets/avalonia-logo.ico"
                         Width="80"
                         Source="file://C:\\path\\image.jpg"
                         Height="80"
                         />
</UserControl>
```
### Loading an image resource from an Avalonia resource
Sample:
```xaml
<UserControl xmlns="https://github.com/avaloniaui"             
             xmlns:labs="clr-namespace:Avalonia.Labs.Controls;assembly=Avalonia.Labs.Controls"
             ...>
    <labs:AsyncImage PlaceholderSource="/Assets/avalonia-logo.ico"
                         Width="80"
                         Source="avares://Avalonia.Labs.Catalog/Assets/maple-leaf-888807_640.jpg"
                         Height="80"
                         />
</UserControl>
