using Blazor.Diagrams.Core.Models;
using GraphUtil.Components;
using Microsoft.Msagl.Core;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Routing;
using Microsoft.Msagl.Layout.Incremental;
using Microsoft.Msagl.Layout.LargeGraphLayout;
using Microsoft.Msagl.Layout.Layered;
using Microsoft.Msagl.Layout.MDS;
using Microsoft.Msagl.Miscellaneous;
using Microsoft.Msagl.Prototype.Ranking;
using AlgorithmBase = GraphUtil.Algorithms.AlgorithmBase;
using Ellipse = Microsoft.Msagl.Core.Geometry.Curves.Ellipse;
using Node = Microsoft.Msagl.Core.Layout.Node;
using Point = Microsoft.Msagl.Core.Geometry.Point;

namespace GraphUtil.Pages;

public partial class Index
{
    private enum Modal
    {
        None,
        Node,
        Edge,
        Algorithm,
        Import,
        AlgorithmDescription,
        EventHistory
    }

    private Modal CurrentModal = Modal.None;

    private string NewNodeName;
    
    private string StartNode;
    private string EndNode;
    private bool Bidirectional = false;
    private double? Weight;

    private string Template;
    private string LastTemplate;
    private string ImportString;

    private AlgorithmBase CurrentAlgorithm;

    private void OpenImportModal()
    {
        CurrentModal = Modal.Import;
    }
    
    private void OpenAddNodeModal()
    {
        CurrentModal = Modal.Node;
    }

    private void OpenAddEdgeModal()
    {
        CurrentModal = Modal.Edge;
    }
    
    private void OpenAlgorithmDescriptionModal()
    {
        CurrentModal = Modal.AlgorithmDescription;
    }

    private void OpenAlgorithmModal(AlgorithmBase algorithm)
    {
        CurrentAlgorithm = algorithm;
        CurrentAlgorithm.Reset();
        foreach(var argument in CurrentAlgorithm.Arguments)
            argument.Reset();
        CurrentModal = Modal.Algorithm;
    }

    private void OpenEventHistoryModal()
    {
        CurrentModal = Modal.EventHistory;
    }

    private void CloseImportModal()
    {
        CurrentModal = Modal.None;
        Template = "";
        LastTemplate = "";
        ImportString = "";
    }
    
    private void CloseAddNodeModal()
    {
        CurrentModal = Modal.None;
        NewNodeName = "";
    }

    private void CloseAddEdgeModal()
    {
        CurrentModal = Modal.None;
        StartNode = "";
        EndNode = "";
        Bidirectional = false;
        Weight = null;
    }

    private void CloseAlgorithmModal()
    {
        CurrentAlgorithm = null;
        CurrentModal = Modal.None;
    }

    private void CloseAlgorithmDescriptionModal()
    {
        CurrentModal = Modal.None;
    }

    private void CloseEventHistoryModal()
    {
        CurrentModal = Modal.None;
    }

    private void ExecuteAlgorithm()
    {
        if (!CurrentAlgorithm.ValidateArguments())
            return;
        ExecutingAlgorithmObj = CurrentAlgorithm;
        ExecutingAlgorithm = CurrentAlgorithm.Execute();
        ResetButton = true;
        CloseAlgorithmModal();
    }

    private GraphNode? AddNode(string name)
    {
        if (string.IsNullOrEmpty(name) || Nodes.ContainsKey(name))
            return null;
        var node = new GraphNode(name);
        _diagram.Nodes.Add(node);
        Nodes.Add(name, node);
        return node;
    }
    
    private void ConfirmNode()
    {
        AddNode(NewNodeName.Trim());
        CloseAddNodeModal();
    }

    private void AddEdge(EdgeInfo edge)
    {
        if (edge.StartNode == edge.EndNode || !Nodes.TryGetValue(edge.StartNode, out var startNode) ||
            !Nodes.TryGetValue(edge.EndNode, out var endNode) || startNode.AdjNodes.Any(adj => adj.Node == endNode) ||
            endNode.AdjNodes.Any(adj => adj.Node == startNode))
            return;
        if (edge.Weight != null && !Weighted)
        {
            Weighted = true;
            foreach (var _link in Links)
                _link.Visible = true;
        }
        var link = new WeightedLink(startNode, endNode, edge.Weight ?? 1.0, edge.Bidirectional);
        link.Visible = Weighted;
        startNode.AdjNodes.Add(new AdjacentNode
        {
            Link = link,
            Node = endNode
        });
        if (edge.Bidirectional)
            endNode.AdjNodes.Add(new AdjacentNode
            {
                Link = link,
                Node = startNode
            });
        else link.TargetMarker = LinkMarker.Arrow;
        Links.Add(link);
        _diagram.Links.Add(link);
    }
    
    private void ConfirmEdge()
    {
        AddEdge(new EdgeInfo
        {
            StartNode = StartNode,
            EndNode = EndNode,
            Bidirectional = Bidirectional,
            Weight = Weight
        });
        CloseAddEdgeModal();
    }

    private void UpdateTemplate(string template)
    {
        Template = template;
        if(TemplateGraphs.ContainsKey(Template))
            ImportString = TemplateGraphs[Template];
    }
    
    private void ConfirmImport()
    {
        var edgeList = new List<EdgeInfo>();
        var nodeList = new List<string>();
        var edges = ImportString.Split('\n');
        foreach (var edge in edges)
        {
            if (edge.StartsWith('#'))
            {
                var isolateds = edge.ReplaceFirst("#", "");
                foreach (var isolatedNode in isolateds.Trim().Split(' '))
                {
                    var iNodeName = isolatedNode.Trim();
                    if(!string.IsNullOrEmpty(iNodeName))
                        nodeList.Add(iNodeName);
                }
                continue;
            }
            var splitted = edge.Trim().Split(' ');
            if (splitted.Length < 2)
                continue;
            var node1 = splitted[0].Trim();
            var node2 = splitted[1].Trim();
            if (node1 == node2 || string.IsNullOrEmpty(node1) || string.IsNullOrEmpty(node2))
                continue;
            nodeList.Add(node1);
            nodeList.Add(node2);
            foreach (var e in edgeList)
            {
                if (e.StartNode == node1 && e.EndNode == node2)
                    continue;
                if (e.StartNode == node2 && e.EndNode == node1)
                    e.Bidirectional = true;
            }
            var edgeInfo = new EdgeInfo
            {
                StartNode = node1,
                EndNode = node2,
                Bidirectional = false
            };
            if (splitted.Length > 2 && int.TryParse(splitted[2], out var weight))
                edgeInfo.Weight = weight;
            edgeList.Add(edgeInfo);
        }

        /*var columnCount = (int)Math.Sqrt(nodeList.Count);
        var row = 0;
        var column = 0;
        foreach (var node in nodeList)
        {
            var _node = AddNode(node);
            if (_node == null)
                continue;
            if (++column > columnCount)
            {
                row++;
                column = 0;
            }
            _node.Position = new Point(column * 150, row * 150);
        }*/

        /*var nodePositions = new Dictionary<string, NodePosition>();
        var positionLinks = new List<PositionLink>();
        foreach (var node in nodeList)
        {
            var _node = AddNode(node);
            if (_node != null)
                nodePositions.Add(node, new NodePosition
                {
                    Node = _node
                });
            else if (Nodes.TryGetValue(node, out var existingNode))
                nodePositions.TryAdd(node, new NodePosition
                {
                    Node = existingNode
                });
        }

        foreach (var edge in edgeList)
            positionLinks.Add(new PositionLink
            {
                Source = nodePositions[edge.StartNode],
                Target = nodePositions[edge.EndNode]
            });

        var nodePositionsList = nodePositions.Values.ToList();
        var layoutGenerator = new LayoutGenerator(nodePositionsList, positionLinks);
        layoutGenerator.Layout();
        var maxXDelta = 0.0;
        var maxYDelta = 0.0;
        foreach (var nodePosition in nodePositionsList)
        {
            if (nodePosition.X < 0 && -1 * nodePosition.X > maxXDelta)
                maxXDelta = -1 * nodePosition.X;
            if (nodePosition.Y < 0 && -1 * nodePosition.Y > maxYDelta)
                maxYDelta = -1 * nodePosition.Y;
        }
        foreach(var nodePosition in nodePositionsList)
            nodePosition.Node.Position = new Point(nodePosition.X + maxXDelta, nodePosition.Y + maxYDelta);

        //AddNode("A").Position = new Point(1700, 800);*/

        var msNodes = new Dictionary<string, Node>();
        var graph = new GeometryGraph();
        foreach (var node in nodeList)
        {
            var _node = AddNode(node);
            if (_node != null)
            {
                var msNode = new Node(new Ellipse(100, 100, new Point(0, 0)));
                msNodes[node] = msNode;
                graph.Nodes.Add(msNode);
            }
        }

        foreach (var edge in edgeList)
        {
            graph.Edges.Add(new(msNodes[edge.StartNode], msNodes[edge.EndNode]));
        }

        var settings = new MdsLayoutSettings();
        //settings.EdgeRoutingSettings.EdgeRoutingMode = EdgeRoutingMode.SugiyamaSplines;


        Dictionary<string, Point> nodePositions;
        try
        {
            LayoutHelpers.CalculateLayout(graph, settings, new CancelToken());
            nodePositions = msNodes.ToDictionary(pair => pair.Key, pair => pair.Value.Center);
        }
        catch
        {
            var rnd = new Random();
            nodePositions = Nodes.ToDictionary(pair => pair.Key,
                pair => new Point(rnd.Next(0, 1000), rnd.Next(0, 1000)));
        }

        var maxXDelta = 0.0;
        var maxYDelta = 0.0;
        foreach (var nodePosition in nodePositions.Values)
        {
            if (nodePosition.X < 0 && -1 * nodePosition.X > maxXDelta)
                maxXDelta = -1 * nodePosition.X;
            if (nodePosition.Y < 0 && -1 * nodePosition.Y > maxYDelta)
                maxYDelta = -1 * nodePosition.Y;
        }
        
        foreach (var pair in nodePositions)
            Nodes[pair.Key].Position =
                new Blazor.Diagrams.Core.Geometry.Point(pair.Value.X + maxXDelta + 100, pair.Value.Y + maxYDelta + 100);
        
        edgeList.ForEach(AddEdge);
        CloseImportModal();
    }
}