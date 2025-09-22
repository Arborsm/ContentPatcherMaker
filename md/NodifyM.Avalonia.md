```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:d="http://schemas.microsoft.com/expression/blend/2008"
        xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
        xmlns:controls="clr-namespace:NodifyM.Avalonia.Controls;assembly=NodifyM.Avalonia"
        xmlns:example="clr-namespace:NodifyM.Avalonia.Example"
        xmlns:viewModelBase="clr-namespace:NodifyM.Avalonia.ViewModelBase;assembly=NodifyM.Avalonia"
        xmlns:converters="clr-namespace:NodifyM.Avalonia.Converters;assembly=NodifyM.Avalonia"
        mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="450"
        x:Class="NodifyM.Avalonia.Example.MainWindow"
        x:DataType="example:MainWindowViewModel"
        Title="Nodify.Avalonia.Example">
    <Grid>
        <StackPanel ZIndex="1">
            <Button  Content="Switch Theme" Command="{Binding ChangeThemeCommand}"></Button>
            <Button ZIndex="1" Content="new"  Click="Button_OnClick"></Button>
            <TextBlock Text="{Binding #Editor.SelectedItem}"></TextBlock>
        </StackPanel>

        <controls:NodifyEditor x:Name="Editor"
            Background="Transparent"
            ItemsSource="{Binding Nodes }"
            Connections="{Binding Connections}"
            PendingConnection="{Binding PendingConnection}"
            DisconnectConnectorCommand="{Binding DisconnectConnectorCommand}">
            <controls:NodifyEditor.DataTemplates>
                <DataTemplate DataType="viewModelBase:KnotNodeViewModel">
                    <controls:KnotNode
                        Location="{Binding Location,Mode=TwoWay}"
                        Content="{Binding Connector}">
                        <controls:KnotNode.ContentTemplate>
                            <DataTemplate DataType="viewModelBase:ConnectorViewModelBase">
                                <controls:Connector
                                    VerticalAlignment="Center"
                                    IsConnected="{Binding IsConnected}"
                                    Anchor="{Binding Anchor, Mode=OneWayToSource}">
                                    <controls:Connector.BorderBrush>
                                        <SolidColorBrush
                                            Color="CornflowerBlue"
                                            Opacity="0.5" />
                                    </controls:Connector.BorderBrush>
                                </controls:Connector>
                            </DataTemplate>
                        </controls:KnotNode.ContentTemplate>
                    </controls:KnotNode>
                </DataTemplate>

            </controls:NodifyEditor.DataTemplates>
            <controls:NodifyEditor.Resources>
                <converters:FlowToDirectionConverter x:Key="FlowToDirectionConverter" />

            </controls:NodifyEditor.Resources>
            <controls:NodifyEditor.GridLineTemplate>
                <DataTemplate>
                    <controls:LargeGridLine Width="{Binding $parent[controls:NodifyEditor].Bounds.Width}"
                                            OffsetX="{Binding $parent[controls:NodifyEditor].OffsetX}"
                                            OffsetY="{Binding $parent[controls:NodifyEditor].OffsetY}"
                                            Zoom="{Binding $parent[controls:NodifyEditor].Zoom}"
                                            Height="{Binding $parent[controls:NodifyEditor].Bounds.Height}"
                                            Spacing="30"
                                            Thickness="0.5"
                                            Brush="{StaticResource GridLinesColorBrush}"
                                            ZIndex="-2" />
                </DataTemplate>
            </controls:NodifyEditor.GridLineTemplate>
            <controls:NodifyEditor.ConnectionTemplate>
                <DataTemplate DataType="{x:Type viewModelBase:ConnectionViewModelBase}">
                    <Grid>
                        <controls:Connection
                            TextBrush="{StaticResource Connection.TextColorBrush}"
                            Text="{Binding Text}"
                            TextPoint="50%,50%"
                            SplitCommand="{Binding SplitConnectionCommand}"
                            DisconnectCommand="{Binding DisconnectConnectionCommand}"
                            Direction="{Binding .,Converter={StaticResource FlowToDirectionConverter}}"
                            Source="{Binding Source.Anchor}" Focusable="True"
                            Target="{Binding Target.Anchor}">
                            <controls:Connection.Stroke>
                                <SolidColorBrush Color="Red"
                                                 Opacity="0.5" />
                            </controls:Connection.Stroke>
                            <controls:Connection.ContextMenu>
                                <ContextMenu>
                                    <MenuItem Header="_Delete" />
                                </ContextMenu>
                            </controls:Connection.ContextMenu>
                        </controls:Connection>
                    </Grid>

                </DataTemplate>
            </controls:NodifyEditor.ConnectionTemplate>
            <controls:NodifyEditor.PendingConnectionTemplate>
                <DataTemplate DataType="{x:Type viewModelBase:PendingConnectionViewModelBase}">
                    <controls:PendingConnection
                        StartedCommand="{Binding StartCommand}"
                        CompletedCommand="{Binding FinishCommand}"
                        EnablePreview="True"
                        EnableSnapping="True"
                        Direction="{Binding Source.Flow,Converter={StaticResource FlowToDirectionConverter}}"
                        PreviewTarget="{Binding PreviewTarget, Mode=OneWayToSource}">
                        <controls:PendingConnection.Stroke>
                            <SolidColorBrush Color="Red"
                                             Opacity="0.5" />

                        </controls:PendingConnection.Stroke>
                        <controls:PendingConnection.Background>
                            <SolidColorBrush Color="DodgerBlue"
                                             Opacity="0.8" />
                        </controls:PendingConnection.Background>
                        <TextBlock Text="{Binding PreviewText}" />


                    </controls:PendingConnection>

                </DataTemplate>
            </controls:NodifyEditor.PendingConnectionTemplate>
            <controls:NodifyEditor.ItemTemplate>
                <DataTemplate DataType="viewModelBase:NodeViewModelBase">
                    <controls:Node x:Name="Node"
                                   Input="{Binding Input}"
                                   Header="{Binding Title}"
                                   Location="{Binding Location,Mode=TwoWay}"
                                   VerticalAlignment="Center"
                                   Output="{Binding Output}">
                        <controls:Node.Styles>
                            <Style Selector="controls|Node[IsSelected=False]:pointerover">
                                <Setter Property="BorderBrush" Value="AliceBlue"></Setter>
                            </Style>
                        </controls:Node.Styles>
                        <controls:Node.InputConnectorTemplate>
                            <DataTemplate DataType="{x:Type viewModelBase:ConnectorViewModelBase}">
                                <controls:NodeInput
                                    x:Name="NodeInput"
                                    VerticalAlignment="Center"
                                    IsConnected="{Binding IsConnected}"
                                    Anchor="{Binding Anchor, Mode=OneWayToSource}">
                                    <controls:NodeInput.Header>
                                        <StackPanel Orientation="Horizontal" VerticalAlignment="Center"
                                                    HorizontalAlignment="Right">

                                            <TextBlock VerticalAlignment="Center" x:Name="textBlock"
                                                       Text="{Binding Title}" />
                                        </StackPanel>
                                    </controls:NodeInput.Header>
                                    <controls:NodeInput.BorderBrush>
                                        <SolidColorBrush
                                            Color="CornflowerBlue"
                                            Opacity="0.5" />
                                    </controls:NodeInput.BorderBrush>
                                </controls:NodeInput>
                            </DataTemplate>
                        </controls:Node.InputConnectorTemplate>

                        <controls:Node.OutputConnectorTemplate>
                            <DataTemplate DataType="{x:Type viewModelBase:ConnectorViewModelBase}">
                                <controls:NodeOutput
                                    x:Name="NodeOutput"
                                    VerticalAlignment="Center"
                                    IsConnected="{Binding IsConnected}"
                                    Anchor="{Binding Anchor, Mode=OneWayToSource}">
                                    <controls:NodeOutput.Header>
                                        <StackPanel Orientation="Horizontal" VerticalAlignment="Center"
                                                    HorizontalAlignment="Right">

                                            <TextBlock VerticalAlignment="Center" x:Name="textBlock"
                                                       Text="{Binding Title}" />
                                        </StackPanel>
                                    </controls:NodeOutput.Header>
                                    <controls:NodeOutput.BorderBrush>
                                        <SolidColorBrush
                                            Color="CornflowerBlue"
                                            Opacity="0.5" />
                                    </controls:NodeOutput.BorderBrush>
                                </controls:NodeOutput>
                            </DataTemplate>
                        </controls:Node.OutputConnectorTemplate>
                    </controls:Node>
                </DataTemplate>

            </controls:NodifyEditor.ItemTemplate>

        </controls:NodifyEditor>
    </Grid>
</Window>
```

```c#
using System;
using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.Input;
using NodifyM.Avalonia.ViewModelBase;

namespace NodifyM.Avalonia.Example;

public partial class MainWindowViewModel : NodifyEditorViewModelBase{
    public MainWindowViewModel()
    {
        var knot1 = new KnotNodeViewModel()
        {
            Location = new Point(300,100)
        };
        var input1 = new ConnectorViewModelBase()
        {
            Title = "AS 1",
            Flow = ConnectorViewModelBase.ConnectorFlow.Input
        };
        var output1 = new ConnectorViewModelBase()
        {
            Title = "B 1",
            Flow = ConnectorViewModelBase.ConnectorFlow.Output
        };
        Connections.Add(new ConnectionViewModelBase(this,output1,knot1.Connector,"Test"));
        Connections.Add(new ConnectionViewModelBase(this,knot1.Connector,input1));
        Nodes  =new(){
                
                new NodeViewModelBase()
                {
                    Location = new Point(400, 2000),
                    Title = "Node 1",
                    Input = new ObservableCollection<object>
                    {
                        input1,
                       new ComboBox()
                       {
                            ItemsSource = new ObservableCollection<object>
                            {
                                 "Item 1",
                                 "Item 2",
                                 "Item 3"
                                 }
                       }
                    },
                    Output = new ObservableCollection<object>
                    {
                       
                        new ConnectorViewModelBase()
                        {
                            Title = "Output 2",
                            Flow = ConnectorViewModelBase.ConnectorFlow.Output
                        }
                    }
                },
                new NodeViewModelBase()
                {
                    Title = "Node 2",
                    Location = new Point(-100,-100),
                    Input = new ObservableCollection<object>
                    {
                        new ConnectorViewModelBase()
                        {
                            Title = "Input 1",
                            Flow = ConnectorViewModelBase.ConnectorFlow.Input
                        },
                        new ConnectorViewModelBase()
                        {
                            Flow = ConnectorViewModelBase.ConnectorFlow.Input,
                            Title = "Input 2"
                        }
                    },
                    Output = new ObservableCollection<object>
                    {
                        output1,
                        new ConnectorViewModelBase()
                        {
                            Flow = ConnectorViewModelBase.ConnectorFlow.Output,
                            Title = "Output 1"
                        },
                        new ConnectorViewModelBase()
                        {
                            Flow = ConnectorViewModelBase.ConnectorFlow.Output,
                            Title = "Output 2"
                        }
                    }
                }
            };
        Nodes.Add(knot1);
        knot1.Connector.IsConnected = true;
        output1.IsConnected = true;
        input1.IsConnected = true;
    }

    public override void Connect(ConnectorViewModelBase source, ConnectorViewModelBase target)
    {
        base.Connect(source, target);
    }

    public override void DisconnectConnector(ConnectorViewModelBase connector)
    {
        base.DisconnectConnector(connector);
    }
    [RelayCommand]
    private void ChangeTheme()
    {
        if (Application.Current.ActualThemeVariant==ThemeVariant.Dark)
        {
            Application.Current.RequestedThemeVariant = ThemeVariant.Light;
        }else
        {
            Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
        }
    }
}
```