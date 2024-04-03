using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Index = GraphUtil.Pages.Index;

namespace GraphUtil.Components;

public static class NodeColor
{
    public const string WHITE = "white";
    public const string GRAY = "gray";
    public const string BLACK = "black";
    public const string RED = "red";
    public const string GREEN = "green";
    public const string YELLOW = "yellow";
}

public class AdjacentNode
{
    public WeightedLink Link;
    public GraphNode Node;
}

public class EdgeInfo
{
    public string StartNode;
    public string EndNode;
    public bool Bidirectional;
    public double? Weight;
}

public class WeightedLink : LinkModel
{
    private double _Weight;
    private bool _Visible;

    public readonly bool Bidirectional;
    
    private readonly LinkLabelModel WeightLabel;
    private readonly LinkLabelModel DistanceLabel;

    public GraphNode SourceGraphNode => (GraphNode)SourceNode;
    public GraphNode TargetGraphNode => (GraphNode)TargetNode;

    public double Weight
    {
        get => _Weight;
        set
        {
            if (value == _Weight)
                return;
            _Weight = value;
            WeightLabel.Content = _Weight.ToString();
            if(Visible)
                Refresh();
        }
    }
    
    public bool Visible
    {
        get => _Visible;
        set
        {
            if (value == _Visible)
                return;
            _Visible = value;
            if (_Visible)
                Labels.Add(WeightLabel);
            else Labels.Remove(WeightLabel);
            Refresh();
        }
    }

    public int Distance
    {
        set
        {
            var added = DistanceLabel.Content != "";
            DistanceLabel.Content = $"-> {value}";
            if(!added)
                Labels.Add(DistanceLabel);
            Refresh();
        }
    }

    public void Reset()
    {
        Color = NodeColor.BLACK;
        Labels.Remove(DistanceLabel);
        DistanceLabel.Content = "";
        Refresh();
    }

    public WeightedLink(NodeModel sourceNode, NodeModel endNode, double weight, bool bidirectional) : base(sourceNode, endNode)
    {
        WeightLabel = new(this, "");
        DistanceLabel = new(this, "");
        Weight = weight;
        Bidirectional = bidirectional;
    }
}

public class NodeInfo
{
    private readonly GraphNode Node;

    public string ID => Node.Text;
    public double? G => Node.DistanceInfo.DistanceFromStart;
    public double X => Node.Position.X;
    public double Y => Node.Position.Y;
    
    public NodeInfo(GraphNode node)
    {
        Node = node;
    }
}

public class NodeDistanceInfo
{
    private readonly GraphNode Node;
    
    private double? _distanceFromStart;     //G(n) - A*, Dijkstra, Bellman Ford, Tarjan bridge
    private double? _distanceToEnd;         //H(n) - A*
    private bool _negInf;                   //Bellman Ford - negative cycles

    public double? DistanceFromStart
    {
        get => _distanceFromStart;
        set
        {
            _distanceFromStart = value;
            Node.Refresh();
        }
    }

    public double? DistanceToEnd
    {
        get => _distanceToEnd;
        set
        {
            _distanceToEnd = value;
            Node.Refresh();
        }
    }

    public bool NegInf
    {
        get => _negInf;
        set
        {
            _negInf = value;
            Node.Refresh();
        }
    }
    
    public double CombinedDistance => (double)DistanceFromStart + (double)DistanceToEnd;        //F(n) - A*

    public void Reset()
    {
        DistanceFromStart = null;
        DistanceToEnd = null;
        NegInf = false;
    }

    public override string ToString() => Math.Round(CombinedDistance, 2).ToString();

    public NodeDistanceInfo Copy()
    {
        return new NodeDistanceInfo(Node)
        {
            _distanceFromStart = DistanceFromStart,
            _distanceToEnd = DistanceToEnd,
            _negInf = NegInf
        };
    }

    public NodeDistanceInfo(GraphNode node)
    {
        Node = node;
    }
}

public class GraphNode : NodeModel
{
    private string _color = NodeColor.WHITE;
    public string Text;
    public List<AdjacentNode> AdjNodes = new();

    public NodeDistanceInfo DistanceInfo;
    public WeightedLink? SourceLink;
    public GraphNode? SourceNode;
    public int? Rank;

    public int InQueue = 0;
    
    public int _Disc = -1;
    public int _Low = -1;
    public bool _ShowLow = false;

    public int Disc
    {
        get => _Disc;
        set
        {
            _Disc = value;
            Refresh();
        }
    }
    public int Low
    {
        get => _Low;
        set
        {
            _Low = value;
            Refresh();
        }
    }
    
    public bool ShowLow
    {
        get => _ShowLow;
        set
        {
            _ShowLow = value;
            Refresh();
        }
    }

    public int? Component = null;   //Kosaraju

    public readonly NodeInfo Info;

    public int InfoLines;
    public int RequiredPages;
    
    public void Reset()
    {
        Color = NodeColor.WHITE;
        SourceLink = null;
        SourceNode = null;
        Rank = null;
        Disc = -1;
        Low = -1;
        ShowLow = false;
        InfoLines = 0;
        RequiredPages = 0;
        InQueue = 0;
        Component = null;
        DistanceInfo.Reset();
    }

    public string ApplyPages()
    {
        RequiredPages = InfoLines == 0 ? 1 : (InfoLines - 1) / 2 + 1;
        if (RequiredPages > Index.Instance.NumInfoPages)
            Index.Instance.NumInfoPages = RequiredPages;
        return "";
    }

    public string Color
    {
        get => _color;
        set
        {
            _color = value;
            Refresh();
        }
    }

    public override string ToString() => Text;

    public GraphNode(string text, Point position = null) : base(position, RenderLayer.HTML, Shapes.Circle)
    {
        Text = text;
        Info = new(this);
        DistanceInfo = new(this);
    }
}