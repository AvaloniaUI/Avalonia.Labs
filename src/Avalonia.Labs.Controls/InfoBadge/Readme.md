
# InfoBadge

Badging is a non-intrusive and intuitive way to display notifications or bring focus to an area within an app - whether that be for notifications, indicating new content, or showing an alert. An info badge is a small piece of UI that can be added into an app and customized to display a number, icon, or a simple dot. Differently from other implementations (example FluentAvalonia) it can be attached to any Visual.

## Types of info badges

There are three styles of info badge that you can choose from - dot, icon, and generic content, as shown in order below.

### Dot info badge

You should use the dot info badge for general scenarios in which you want to guide the user's focus towards the info badge for example, to indicate new content or updates are available.

### Icon info badge

You should use the icon info badge to send a quick message along with getting the user's attention for example, to alert the user that something non-blocking has gone wrong, an extra important update is available, or that something specific in the app is currently enabled (such as a countdown timer going).

### Generic content info badge

The numeric info badge is the same shape and size as the icon info badge, but it holds a number inside of it, determined by the Content property.

You should use the Generic content info badge to show that there are a specific number of items that need attention for example, new emails or messages.

## How To Use

### Using default styles

Sample:

```xaml
<UserControl xmlns="https://github.com/avaloniaui"
xmlns:labs="clr-namespace:Avalonia.Labs.Controls;assembly=Avalonia.Labs.Controls"
             ...>
       <StackPanel Spacing="5">
          <TextBlock Text="Attention" />
          <StackPanel Orientation="Horizontal" Spacing="20">
            <labs:InfoBadge Classes="Attention Icon" HorizontalAlignment="Right"/>
            <labs:InfoBadge Classes="Attention" HorizontalAlignment="Right" Content="10"/>
            <labs:InfoBadge Classes="Attention Dot" VerticalAlignment="Center"/>
          </StackPanel>

          <TextBlock Text="Informational" />
          <StackPanel Orientation="Horizontal" Spacing="20">
            <labs:InfoBadge Classes="Informational Icon" HorizontalAlignment="Right"/>
            <labs:InfoBadge Classes="Informational" HorizontalAlignment="Right"  Content="10"/>
            <labs:InfoBadge Classes="Informational Dot" VerticalAlignment="Center"/>
          </StackPanel>

          <TextBlock Text="Success" />
          <StackPanel Orientation="Horizontal" Spacing="20">
            <labs:InfoBadge Classes="Success Icon" HorizontalAlignment="Right"/>
            <labs:InfoBadge Classes="Success" HorizontalAlignment="Right"  Content="10"/>
            <labs:InfoBadge Classes="Success Dot" VerticalAlignment="Center"/>
          </StackPanel>

          <TextBlock Text="Caution" />
          <StackPanel Orientation="Horizontal" Spacing="20">
            <labs:InfoBadge Classes="Caution Icon" HorizontalAlignment="Right"/>
            <labs:InfoBadge Classes="Caution" HorizontalAlignment="Right"  Content="10"/>
            <labs:InfoBadge Classes="Caution Dot" VerticalAlignment="Center"/>
          </StackPanel>

          <TextBlock Text="Critical" />
          <StackPanel Orientation="Horizontal" Spacing="20">
            <labs:InfoBadge Classes="Critical Icon" HorizontalAlignment="Right"/>
            <labs:InfoBadge Classes="Critical" HorizontalAlignment="Right"  Content="10"/>
            <labs:InfoBadge Classes="Critical Dot" VerticalAlignment="Center" />
          </StackPanel>
        </StackPanel>
</UserControl>
```

### Custom shape

```xaml
<UserControl xmlns="https://github.com/avaloniaui"
xmlns:labs="clr-namespace:Avalonia.Labs.Controls;assembly=Avalonia.Labs.Controls"
             ...>
       <StackPanel Spacing="5">
          <Button Content="Trinagle Badge" ClipToBounds="False">
            <labs:InfoBadge.Badge>
                <labs:InfoBadge Background="Green"
                                ClipToBounds="False"
                                Foreground="White"
                                BorderThickness="2"
                                FontWeight="Bold"
                                HorizontalAlignment="Right"
                                VerticalAlignment="Center"
                                Width="20"
                                Height="20"
                                Content="1">
                <labs:InfoBadge.Shape>
                    <StreamGeometry>M0,20 L 10,0 L 20,20z</StreamGeometry>
                </labs:InfoBadge.Shape>
                </labs:InfoBadge>
            </labs:InfoBadge.Badge>
          </Button>
       </StackPanel>
</UserControl>
```

## Resources

### Classes

|Class | | Dot | Icon |
|-|-:|-:|-:|
|Attention| ![Attention](/Assets/Attention.png)| ![Attention Dot](/Assets/Attention.Dot.png) | ![Attention Icon](/Assets/Attention.Icon.png) |
|Informational| ![Informational](/Assets/Informational.png)| ![Informational Dot](/Assets/Informational.Dot.png) | ![Informational Icon](/Assets/Informational.Icon.png) |
|Success| ![Success](/Assets/Success.png)| ![Success Dot](/Assets/Success.Dot.png) | ![Success Icon](/Assets/Success.Icon.png) |
|Caution| ![Caution](/Assets/Caution.png)| ![Caution Dot](/Assets/Caution.Dot.png) | ![Caution Icon](/Assets/Caution.Icon.png) |
|Critical| ![Critical](/Assets/Critical.png)| ![Critical Dot](/Assets/Critical.Dot.png) | ![Critical Icon](/Assets/Critical.Icon.png) |

### Default Brush

|Key |Value|
|-|-|
|SystemFillColorAttentionBrush| ![#FFD7D710](https://placehold.co/15x15/D7D710/D7D710.png)`#FFD7D710` |
|SystemFillColorInformationalBrush| ![#FF8A8A8A](https://placehold.co/15x15/8A8A8A/8A8A8A.png)`#FF8A8A8A`|
|SystemFillColorSuccessBrush| ![#FF0F7B0F](https://placehold.co/15x15/0F7B0F/0F7B0F.png)`#FF0F7B0F` |
|SystemFillColorCautionBrush| ![#FF9D5D00](https://placehold.co/15x15/9D5D00/9D5D00.png)`#FF9D5D00`|
|SystemFillColorCriticalBrush| ![#FFCC0000](https://placehold.co/15x15/CC0000/CC0000.png)`#FFCC0000`|

### Default Shape

|Key|Description|
|-|-|
|DefaultBadgeShape| Base shape for all classes |
|DefaultDotBadgeShape| Base shape for Dot classes |
|SystemAttentionDefaultShape| Default shape for `Attention` class |
|SystemAttentionDotShape| Deafult shape for `Attention Dot` class |
|SystemAttentionIconShape| Deafult shape for `Attention Icon` class |
|SystemInformationalDefaultShape| Default shape for `Informational` class |
|SystemInformationalDotShape| Deafult shape for `Informational Dot` class |
|SystemInformationalIconShape| Deafult shape for `Informational Icon` class |
|SystemSuccessDefaultShape| Default shape for `Success` class |
|SystemSuccessDotShape| Deafult shape for `Success Dot` class |
|SystemSuccessIconShape| Deafult shape for `Success Icon` class |
|SystemCautionDefaultShape| Default shape for `Attention` class |
|SystemCautionDotShape| Deafult shape for `Attention Dot` class |
|SystemCautionIconShape| Deafult shape for `Attention Icon` class |
|SystemCriticalDefaultShape| Default shape for `Attention` class |
|SystemCriticalDotShape| Deafult shape for `Attention Dot` class |
|SystemCriticalIconShape| Deafult shape for `Attention Icon` class |
