namespace DlaTest;

public class Edge
{
    public Node StartNode { get; }

    public Node EndNode { get; }

    public double Weight { get; }

    public Edge(Node startNode,  Node endNode)
    {
        StartNode = startNode;
        EndNode = endNode;
        var (x1, y1) = (startNode.X, startNode.Y);
        var (x2, y2) = (endNode.X, endNode.Y);
        var x = Math.Pow(x1 - x2, 2);
        var y = Math.Pow(y1 - y2, 2);
        Weight = Math.Pow(x + y, 0.5);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StartNode.Id, EndNode.Id);
    }
}
public class Node(double x, double y)
{
    public int Id => GetHashCode();

    public double X { get; } = x;

    public double Y { get; } = y;

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}
public class NodeItem
{
    public bool Used { get; set; }
    public List<int> Nodes { get; } = [];
    public int NodeId { get; set; }
    public int Index { get; set; }
    public double Weight { get; set; }
}

public class Route
{
    public List<Edge> Edges { get; set; } = [];
}


public class DijkstraRouter
{
    static double[,] Graph { get; set; } = new double[0, 0];

    /// <summary>
    /// 所有的边
    /// </summary>
    static List<Edge> Edges { get; set; } = [];

    /// <summary>
    /// 所有的点
    /// </summary>
    static List<Node> Nodes { get; set; } = [];

    static List<NodeItem> NodeItems { get; set; } = [];

    static bool IsRouting { get; set; }

    public static Route? GetRoute(int startPointID, int endPointID)
    {
        if (IsRouting)
            throw new InvalidOperationException($"can't route.router busy");
        IsRouting = true;
        Node? sNode = null;
        Node? dNode = null;
        try
        {
            if ((sNode = Nodes.FirstOrDefault(x => x.Id == startPointID)) == null
                || (dNode = Nodes.FirstOrDefault(x => x.Id == endPointID)) == null)
                throw new ArgumentNullException("can't found target points.");
            NodeItems.FirstOrDefault(x => x.NodeId == startPointID).Used = true;
            NodeItems.ForEach(x =>
            {
                x.Weight = GetRowArray(Graph, Nodes.IndexOf(sNode))[x.Index];
                x.Nodes.Add(startPointID);
            });
            while (NodeItems.Any(x => !x.Used))
            {
                var item = GetUnUsedAndMinNodeItem();
                if (item == null)
                    break;

                item.Used = true;
                var tempRow = GetRowArray(Graph, item.Index);
                foreach (var nodeItem in NodeItems)
                {
                    if (nodeItem.Weight > tempRow[nodeItem.Index] + item.Weight)
                    {
                        nodeItem.Weight = tempRow[nodeItem.Index] + item.Weight;
                        nodeItem.Nodes.Clear();
                        nodeItem.Nodes.AddRange(item.Nodes);
                        nodeItem.Nodes.Add(item.NodeId);
                    }
                }
            }
            var desNodeitem = NodeItems.FirstOrDefault(x => x.NodeId == endPointID);
            if (desNodeitem.Used && desNodeitem.Weight < double.MaxValue)
            {
                var edges = new List<Edge>();
                foreach (var index in Enumerable.Range(0, desNodeitem.Nodes.Count - 1))
                {
                    edges.Add(Edges.FirstOrDefault(x => x.StartNode.Id == desNodeitem.Nodes[index] && x.EndNode.Id == desNodeitem.Nodes[index + 1]));
                }
                edges.Add(Edges.FirstOrDefault(x => x.StartNode.Id == desNodeitem.Nodes.Last() && x.EndNode.Id == endPointID));
                return new Route()
                {
                    Edges = edges
                };
            }
            return null;
        }
        catch (Exception ex)
        {
            //_logger.LogError(ex.ToString());
            //_logger.LogInformation($"startPoint:{startPointID}-endpoint:{endPointID} route faild.");
            throw;
        }
        finally
        {
            NodeItems.ForEach(x =>
            {
                x.Used = false;
                x.Nodes.Clear();
            });
            IsRouting = false;
        }
    }

    private static NodeItem GetUnUsedAndMinNodeItem()
    {
        return NodeItems.Where(x => !x.Used && x.Weight != double.MaxValue).OrderBy(x => x.Weight).FirstOrDefault();
    }

    private static double[] GetRowArray(double[,] source, int row)
    {
        double[] result = new double[source.GetLength(1)];
        foreach (var index in Enumerable.Range(0, result.Length))
        {
            result[index] = source[row, index];
        }

        return result;
    }

    public static void Initialize(IEnumerable<Edge> edges, IEnumerable<Node> nodes)
    {
        Edges = edges.ToList();
        Nodes = nodes.ToList();
        NodeItems = [];
        Graph = new double[Nodes.Count, Nodes.Count];
        foreach (var row in Enumerable.Range(0, Nodes.Count))
        {
            var rowNode = Nodes[row];
            foreach (var colnum in Enumerable.Range(0, Nodes.Count))
            {
                if (row == colnum)
                {
                    Graph[row, colnum] = 0;
                    continue;
                }
                var edge = Edges.FirstOrDefault(x =>
                        x.StartNode.Id == rowNode.Id && x.EndNode.Id == Nodes[colnum].Id);
                Graph[row, colnum] = edge == null ? double.MaxValue : edge.Weight;
            }

            NodeItems.Add(new NodeItem()
            {
                NodeId = Nodes[row].Id,
                Index = row,
                Weight = double.MaxValue
            });
        }
    }
}
