# Avalonia.Labs.CommandManager

Experimental WPF-like implementation of RoutedCommand.

the currently supported flow is this:

```mermaid
sequenceDiagram
    participant RotendCommand
    participant View
    participant CommandManager
    participant ViewModel
    participant TopLevel
    TopLevel->>CommandManager: Got Focus
    CommandManager->>RotendCommand: Raise CanExecuteChanged
    RotendCommand->>View: Raise CommandManager.CanExecuteEvent
    View->>CommandManager: Raise CommandManager.CanExecuteEvent
    CommandManager->>ViewModel: ICommand.CanExecute
    CommandManager->>RotendCommand: Handle=True
    RotendCommand->>View: Raise CommandManager.ExecuteEvent
    View->>CommandManager: Raise CommandManager.ExecuteEvent
    CommandManager->>ViewModel: ICommand.Execute
``````

## Get Started

01. Install package

    ```bash
    dotnet add package package Avalonia.Labs.CommandManager
    ```

02. Add XML Namespace

    ```xaml+diff
    <UserControl xmlns="https://github.com/avaloniaui"
                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
                xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
                xmlns:lab="using:Avalonia.Labs.Catalog"
    +             xmlns:rc="using:Avalonia.Labs.Input"
                mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
                xmlns:viewModels="using:Avalonia.Labs.Catalog.ViewModels"
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

04. Assign your `RotedCommand`

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

05. Binding `RouterCommand` to yours ViewModel

    ```xaml
    <UserControl xmlns="https://github.com/avaloniaui"
                xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
                xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
                xmlns:lab="using:Avalonia.Labs.Catalog"
                xmlns:rc="using:Avalonia.Labs.Input"
                mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
                xmlns:viewModels="using:Avalonia.Labs.Catalog.ViewModels"
                x:Class="Avalonia.Labs.Catalog.Views.RouteCommandView"
                x:DataType="viewModels:RouteCommandViewModel"
                HorizontalContentAlignment="Stretch"
                VerticalContentAlignment="Stretch"
                >
    <rc:CommandManager.CommandBindings>
        <rc:CommandBinding Command="{x:Static lab:ApplicationCommands.Open}" ExecutingCommand="{Binding Open}" />
        <rc:CommandBinding Command="{x:Static lab:ApplicationCommands.Save}" ExecutingCommand="{Binding Save}" />
    </rc:CommandManager.CommandBindings>
    ...
    </UserControl>
    ```

You can view full sample [here](https://github.com/AvaloniaUI/Avalonia.Labs/samples/Avalonia.Labs.Catalog/Views/RouteCommandView.axaml).

## Todo

- CommandTarget (_requires modification in Avalonia_).
