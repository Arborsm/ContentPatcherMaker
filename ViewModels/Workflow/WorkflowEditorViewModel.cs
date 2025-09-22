using System.Collections.ObjectModel;
using Avalonia;
using NodifyM.Avalonia.ViewModelBase;

namespace ContentPatcherMaker.ViewModels.Workflow;

public class WorkflowEditorViewModel : NodifyEditorViewModelBase
{
    public WorkflowEditorViewModel()
    {
        var knot = new KnotNodeViewModel
        {
            Location = new Point(300, 100)
        };

        var input = new ConnectorViewModelBase
        {
            Title = "Input",
            Flow = ConnectorViewModelBase.ConnectorFlow.Input
        };

        var output = new ConnectorViewModelBase
        {
            Title = "Output",
            Flow = ConnectorViewModelBase.ConnectorFlow.Output
        };

        Connections.Add(new ConnectionViewModelBase(this, output, knot.Connector, "Demo"));
        Connections.Add(new ConnectionViewModelBase(this, knot.Connector, input));

        Nodes = new()
        {
            new NodeViewModelBase
            {
                Title = "Node A",
                Location = new Point(100, 100),
                Input = new ObservableCollection<object>
                {
                    input
                },
                Output = new ObservableCollection<object>
                {
                    new ConnectorViewModelBase
                    {
                        Title = "Out",
                        Flow = ConnectorViewModelBase.ConnectorFlow.Output
                    }
                }
            }
        };

        Nodes.Add(knot);
        knot.Connector.IsConnected = true;
        output.IsConnected = true;
        input.IsConnected = true;
    }
}

