# Avalonia.Labs.RoutedCommand

Experimenta implemtation of RoutedCommand like WPF.

the currently supported flow is this:

```mermaid
sequenceDiagram
    participant RotendCommand
    participant View
    participant RoutedCommandsManager
    participant ViewModel
    participant TopLevel
    TopLevel->>RoutedCommandsManager: Got Focus
    RoutedCommandsManager->>RotendCommand: Raise CanExecuteChanged
    RotendCommand->>View: Raise RoutedCommandsManager.CanExecuteEvent
    View->>RoutedCommandsManager: Raise RoutedCommandsManager.CanExecuteEvent
    RoutedCommandsManager->>ViewModel: ICommand.CanExecute
    RoutedCommandsManager->>RotendCommand: Handle=True
    RotendCommand->>View: Raise RoutedCommandsManager.ExecuteEvent
    View->>RoutedCommandsManager: Raise RoutedCommandsManager.ExecuteEvent
    RoutedCommandsManager->>ViewModel: ICommand.Execute
``````

## Get Started

01. Install package

    ```bash
    dotnet add package package Avalonia.Labs.RoutedCommand
    ```

02. Add XML Namespace

    ```xaml+diff
    <UserControl xmlns="https://github.com/avaloniaui"
                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
                xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
                xmlns:lab="clr-namespace:Avalonia.Labs.Catalog"
    +             xmlns:rc="clr-namespace:Avalonia.Labs.Input;assembly=Avalonia.Labs.RoutedCommand"
                mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
                xmlns:viewModels="clr-namespace:Avalonia.Labs.Catalog.ViewModels"
                x:Class="Avalonia.Labs.Catalog.Views.RouteCommandView"
                x:DataType="viewModels:RouteCommandViewModel"
                HorizontalContentAlignment="Stretch"
                VerticalContentAlignment="Stretch"
                >
                ...
    </UserControl>
    ```

03. Define your's RouterCommands

    ```csharp
    public static class ApplicationCommands
    {
        public readonly static RoutedCommand Open = new RoutedCommand(nameof(Open));
        public readonly static RoutedCommand Save = new RoutedCommand(nameof(Save));
    }
    ```

04. Assing your `RotedCommand`

    ```xaml
            <UniformGrid Columns="2"
                        Grid.Column="2">
                <Button Content="Open"
                        Command="{x:Static lab:ApplicationCommands.Open}"
                        CommandParameter="{Binding .}"/>
                <Button Content="Save"
                        Command="{x:Static lab:ApplicationCommands.Save}"
                        CommandParameter="{Binding .}"/>
            </UniformGrid>
    ```

05. Binding `RouterCommand` to your's ViewModel

    ```xaml
    <UserControl xmlns="https://github.com/avaloniaui"
                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
                xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
                xmlns:lab="clr-namespace:Avalonia.Labs.Catalog"
                xmlns:rc="clr-namespace:Avalonia.Labs.Input;assembly=Avalonia.Labs.RoutedCommand"
                mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
                xmlns:viewModels="clr-namespace:Avalonia.Labs.Catalog.ViewModels"
                x:Class="Avalonia.Labs.Catalog.Views.RouteCommandView"
                x:DataType="viewModels:RouteCommandViewModel"
                HorizontalContentAlignment="Stretch"
                VerticalContentAlignment="Stretch"
                >
    <rc:RoutedCommandsManager.Commands>
        <rc:RoutedCommandBinding RoutedCommand="{x:Static lab:ApplicationCommands.Open}" Command="{Binding Open}" />
        <rc:RoutedCommandBinding RoutedCommand="{x:Static lab:ApplicationCommands.Save}" Command="{Binding Save}" />
    </rc:RoutedCommandsManager.Commands>
    ...
    </UserControl>
    ```

You can view full sample [here](https://github.com/AvaloniaUI/Avalonia.Labs/samples/Avalonia.Labs.Catalog/Views/RouteCommandView.axaml).

## Todo

- KeyBinding
- CommandTarget (_requires modification in Avalonia_).
