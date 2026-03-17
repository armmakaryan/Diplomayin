using DijkstraShortestPath.UI.Models;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DijkstraShortestPath.UI.Rendering;

internal sealed class GraphCanvasRenderer : IGraphRenderer
{
    private readonly Canvas _canvas;
    private GraphModel? _model;

    private readonly Dictionary<int, Ellipse> _nodeEllipse = new();
    private readonly Dictionary<int, TextBlock> _nodeLabel = new();
    private readonly Dictionary<(int u, int v), Line> _edgeLine = new();
    private readonly Dictionary<(int u, int v), TextBlock> _edgeWeight = new();

    private readonly Action<string> _setStatus;

    public GraphCanvasRenderer(Canvas canvas, Action<string> setStatus)
    {
        _canvas = canvas;
        _setStatus = setStatus;
    }

    public int VertexCount => _model?.N ?? 0;

    public string VertexName(int i) => _model!.Vertices[i];

    public double Weight(int u, int v) => _model!.A[u, v];

    public void Draw(GraphModel model)
    {
        _model = model;

        _canvas.Children.Clear();
        _nodeEllipse.Clear();
        _nodeLabel.Clear();
        _edgeLine.Clear();
        _edgeWeight.Clear();

        int n = model.N;
        if (n == 0) return;

        double cx = _canvas.ActualWidth > 10 ? _canvas.ActualWidth / 2 : 350;
        double cy = _canvas.ActualHeight > 10 ? _canvas.ActualHeight / 2 : 250;
        double radius = Math.Min(cx, cy) - 60;
        if (radius < 120) radius = 200;

        var pos = new (double x, double y)[n];
        for (int i = 0; i < n; i++)
        {
            double angle = 2 * Math.PI * i / n - Math.PI / 2;
            pos[i] = (cx + radius * Math.Cos(angle), cy + radius * Math.Sin(angle));
        }

        for (int u = 0; u < n; u++)
        {
            for (int v = 0; v < n; v++)
            {
                if (model.A[u, v] <= 0 || u == v) continue;

                var line = new Line
                {
                    X1 = pos[u].x,
                    Y1 = pos[u].y,
                    X2 = pos[v].x,
                    Y2 = pos[v].y,
                    Stroke = Brushes.LightGray,
                    StrokeThickness = 2
                };
                _canvas.Children.Add(line);
                _edgeLine[(u, v)] = line;

                var tb = new TextBlock
                {
                    Text = model.A[u, v].ToString("0.##"),
                    Foreground = Brushes.Gray,
                    Background = Brushes.White
                };
                Canvas.SetLeft(tb, (pos[u].x + pos[v].x) / 2);
                Canvas.SetTop(tb, (pos[u].y + pos[v].y) / 2);
                _canvas.Children.Add(tb);
                _edgeWeight[(u, v)] = tb;
            }
        }

        for (int i = 0; i < n; i++)
        {
            var e = new Ellipse
            {
                Width = 46,
                Height = 46,
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };
            Canvas.SetLeft(e, pos[i].x - 23);
            Canvas.SetTop(e, pos[i].y - 23);
            _canvas.Children.Add(e);
            _nodeEllipse[i] = e;

            var label = new TextBlock
            {
                Text = model.Vertices[i],
                FontWeight = System.Windows.FontWeights.Bold,
                Foreground = Brushes.Black
            };
            Canvas.SetLeft(label, pos[i].x - 10);
            Canvas.SetTop(label, pos[i].y - 10);
            _canvas.Children.Add(label);
            _nodeLabel[i] = label;
        }
    }

    public void ResetVisuals()
    {
        foreach (var kv in _nodeEllipse)
        {
            kv.Value.Fill = Brushes.White;
            kv.Value.Stroke = Brushes.Black;
        }
        foreach (var kv in _edgeLine)
        {
            kv.Value.Stroke = Brushes.LightGray;
            kv.Value.StrokeThickness = 2;
        }
    }

    public void HighlightNode(int i, Brush fill)
    {
        if (_nodeEllipse.TryGetValue(i, out var e))
        {
            e.Fill = fill;
            e.Stroke = Brushes.Black;
        }
    }

    public void HighlightEdge(int u, int v, Brush stroke, double thickness)
    {
        if (_edgeLine.TryGetValue((u, v), out var line))
        {
            line.Stroke = stroke;
            line.StrokeThickness = thickness;
        }
    }

    public void SetStatus(string text) => _setStatus(text);
}
