namespace DijkstraShortestPath.UI.Models;


public sealed class GraphModel
{
    public string[] Vertices { get; }
    public double[,] A { get; }

    public int N => Vertices.Length;

    public GraphModel(string[] vertices, double[,] a)
    {
        Vertices = vertices ?? Array.Empty<string>();
        A = a ?? new double[0, 0];

        if (Vertices.Length == 0) throw new ArgumentException("Vertices empty");
        if (A.GetLength(0) != Vertices.Length || A.GetLength(1) != Vertices.Length)
            throw new ArgumentException("Matrix size must match vertices count");
    }
}
