namespace DijkstraShortestPath.UI.Rendering;

using System.Windows.Media;

public interface IGraphRenderer
{
    void ResetVisuals();
    void HighlightNode(int i, Brush fill);
    void HighlightEdge(int u, int v, Brush stroke, double thickness);
    void SetStatus(string text);
    string VertexName(int i);
    int VertexCount { get; }
    double Weight(int u, int v);
}