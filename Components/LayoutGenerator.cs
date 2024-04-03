namespace GraphUtil.Components;

public class NodePosition
{
    public GraphNode Node;
    public double X;
    public double Y;
    public double XForce;
    public double YForce;
}

public class PositionLink
{
    public NodePosition Source;
    public NodePosition Target;
}

public class LayoutGenerator
{
    private const double RepulsiveForceFactor = 4000.0;
    private const double AttractiveForceFactor = 0.25;
    private const double DampingFactor = 15;
    private const int MaxIterations = 1000;

    private readonly List<NodePosition> nodes;
    private readonly List<PositionLink> edges;
    private readonly Random random;

    public LayoutGenerator(List<NodePosition> nodes, List<PositionLink> edges)
    {
        this.nodes = nodes;
        this.edges = edges;
        random = new Random();
    }

    public void Layout()
    {
        InitializePositions();

        for (int i = 0; i < MaxIterations; i++)
        {
            CalculateForces();
            UpdatePositions();
            ApplyDamping();
        }
    }

    private void InitializePositions()
    {
        foreach (var node in nodes)
        {
            node.X = random.NextDouble() * 1700;
            node.Y = random.NextDouble() * 800;
        }
    }

    private void CalculateForces()
    {
        foreach (var node in nodes)
        {
            node.XForce = 0.0;
            node.YForce = 0.0;
        }

        foreach (var edge in edges)
        {
            var dx = edge.Target.X - edge.Source.X;
            var dy = edge.Target.Y - edge.Source.Y;

            var distance = Math.Sqrt(dx * dx + dy * dy);

            var attractiveForce = distance * AttractiveForceFactor;

            edge.Source.XForce += attractiveForce * dx / distance;
            edge.Source.YForce += attractiveForce * dy / distance;

            edge.Target.XForce -= attractiveForce * dx / distance;
            edge.Target.YForce -= attractiveForce * dy / distance;
        }

        foreach (var node1 in nodes)
        {
            foreach (var node2 in nodes)
            {
                if (node1 != node2)
                {
                    var dx = node2.X - node1.X;
                    var dy = node2.Y - node1.Y;

                    var distance = Math.Sqrt(dx * dx + dy * dy);

                    var repulsiveForce = RepulsiveForceFactor / distance;

                    node1.XForce -= repulsiveForce * dx / distance;
                    node1.YForce -= repulsiveForce * dy / distance;
                }
            }
        }
    }

    private void UpdatePositions()
    {
        foreach (var node in nodes)
        {
            node.X += node.XForce;
            node.Y += node.YForce;
        }
    }

    private void ApplyDamping()
    {
        foreach (var node in nodes)
        {
            node.XForce *= DampingFactor;
            node.YForce *= DampingFactor;
        }
    }
}