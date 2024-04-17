using Blazor.Diagrams.Core;
using GraphUtil.Algorithms;
using GraphUtil.Algorithms.Arguments.ArgumentTypes;
using GraphUtil.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json;

namespace GraphUtil.Pages;

public partial class Index
{
    public static Index Instance;
    
    private Diagram _diagram = new Diagram(new DiagramOptions
    {
        EnableVirtualization = false,
        Zoom = new DiagramZoomOptions
        {
            Inverse = true
        },
        Constraints = new DiagramConstraintsOptions
        {
            ShouldDeleteLink = _ => false,
            ShouldDeleteNode = _ => false,
            ShouldDeleteGroup = _ => false
        }
    });
    internal Dictionary<string, GraphNode> Nodes = new();
    internal List<WeightedLink> Links = new();
    internal bool Weighted;

    internal IJSInProcessRuntime JS;
    private AlgorithmBase[] Algorithms = Array.Empty<AlgorithmBase>();

    private IEnumerator<StopLevel>? ExecutingAlgorithm;
    private AlgorithmBase? ExecutingAlgorithmObj;

    private bool ResetButton;
    
    public int NumInfoPages = 1;
    public int CurrentInfoPage = 1;
    
    public string? Log(string message)
    {
        Console.WriteLine(message);
        return null;
    }

    protected override void OnInitialized()
    {
        Instance = this;
        JS = (IJSInProcessRuntime)_JS;
        var predefinedScriptsString = JS.Invoke<string?>("localStorage.getItem", "scripts");
        if (predefinedScriptsString != null)
        {
            var predefinedScripts = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, ScriptArgument>>>(predefinedScriptsString);
            if(predefinedScripts != null)
                ScriptArgument.PredefinedScripts = predefinedScripts;
        }
        Algorithms = new AlgorithmBase[]
        {
            new BreadthFirstSearch(this),
            new DepthFirstSearch(this),
            new Dijkstra(this),
            new AStar(this),
            new BellmanFord(this),
            new DominatorSet(this),
            new Kosaraju(this),
            new Kruskal(this),
            new TarjanBridge(this)
        };
        
        base.OnInitialized();

        _diagram.RegisterModelComponent<GraphNode, GraphNodeWidget>();
    }

    private void Reset()
    {
        ExecutingAlgorithmObj = null;
        ExecutingAlgorithm = null;
        foreach (var node in Nodes.Values)
            node.Reset();
        foreach (var link in Links)
            link.Reset();
        ResetButton = false;
        NumInfoPages = 1;
        CurrentInfoPage = 1;
    }

    private void NextInfoPage()
    {
        if (++CurrentInfoPage > NumInfoPages)
            CurrentInfoPage = 1;
        foreach(var node in Nodes.Values)
            node.Refresh();
    }

    private async Task CopyGraph()
    {
        var isolatedPoints = new List<string>();
        foreach (var node in Nodes.Values)
        {
            if (node.AdjNodes.Count == 0 && !Nodes.Values.Any(p => p.AdjNodes.Any(q => q.Node == node)))
                isolatedPoints.Add(node.Text);
        }
        var edges = new List<string>();
        foreach (var link in Links)
        {
            edges.Add(link.Visible
                ? $"{link.SourceGraphNode.Text} {link.TargetGraphNode.Text} {link.Weight}"
                : $"{link.SourceGraphNode.Text} {link.TargetGraphNode.Text}");
            if (link.Bidirectional)
                edges.Add(link.Visible
                    ? $"{link.TargetGraphNode.Text} {link.SourceGraphNode.Text} {link.Weight}"
                    : $"{link.TargetGraphNode.Text} {link.SourceGraphNode.Text}");
        }
        var result = string.Join("\n", edges);
        if (isolatedPoints.Count > 0)
            result = $"#{string.Join(" ", isolatedPoints)}\n{result}";
        await JS.InvokeVoidAsync("copy", result);
    }

    private void ResetGraph()
    {
        Nodes.Clear();
        Links.Clear();
        Weighted = false;
        foreach(var group in _diagram.Groups)
            group.Ungroup();
        _diagram.Links.Clear();
        _diagram.Nodes.Clear();
    }

    private bool StepAlgorithm()
    {
        if (!ExecutingAlgorithm.MoveNext())
        {
            ExecutingAlgorithm = null;
            ResetButton = true;
            ExecutingAlgorithmObj.CurrentEvent = "VÉGE";
            return false;
        }
        return true;
    }

    private void JumpAlgorithm()
    {
        do
        {
            if (!StepAlgorithm())
                return;
        } while (ExecutingAlgorithm.Current != StopLevel.Always);
    }

    private void JumpToEnd()
    {
        while (StepAlgorithm())
        {
        }
    }
}