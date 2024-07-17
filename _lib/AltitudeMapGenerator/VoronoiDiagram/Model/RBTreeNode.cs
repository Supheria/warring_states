namespace AltitudeMapGenerator.VoronoiDiagram.Model;

public class RBTreeNode<T>
{
    public T Data { get; internal set; }
    public RBTreeNode<T> Left { get; internal set; }
    public RBTreeNode<T> Right { get; internal set; }
    public RBTreeNode<T> Parent { get; internal set; }

    //cached ordered traversal
    public RBTreeNode<T> Previous { get; internal set; }
    public RBTreeNode<T> Next { get; internal set; }

    internal bool Red { get; set; }

    internal RBTreeNode()
    {

    }
}
