using DijkstraShortestPath.UI.Algorithms;
using DijkstraShortestPath.UI.Models;
using DijkstraShortestPath.UI.Rendering;
using DijkstraShortestPath.UI.Services;
using System.Data;
using System.Windows;

namespace DijkstraShortestPath.UI;

public partial class MainWindow : Window
{
    private string[] _vertices = Array.Empty<string>();
    private double[,] _A = new double[0, 0];

    private GraphCanvasRenderer? _renderer;
    private DijkstraAnimator? _animator;

    public MainWindow()
    {
        InitializeComponent();

        _renderer = new GraphCanvasRenderer(GraphCanvas, text => StatusText.Text = text);
        _animator = new DijkstraAnimator(_renderer);

        GenerateMatrixUI(5);
        RefreshCombos();
    }

    private void Generate_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(NBox.Text.Trim(), out int n) || n <= 0 || n > 30)
        {
            StatusText.Text = "⚠️ n-ը պետք է լինի 1..30";
            return;
        }
        GenerateMatrixUI(n);
        StatusText.Text = "✅ Matrix UI ստեղծվեց";
    }

    private void GenerateMatrixUI(int n)
    {
        _vertices = VerticesService.BuildVertices(VerticesBox.Text, n);

        MatrixGrid.ItemsSource = MatrixUiService.CreateMatrixView(_vertices);
        VerticesBox.Text = string.Join(Environment.NewLine, _vertices);

        RefreshCombos();
    }

    private void RefreshCombos()
    {
        StartCombo.ItemsSource = _vertices;
        EndCombo.ItemsSource = _vertices;

        if (_vertices.Length > 0)
        {
            StartCombo.SelectedIndex = 0;
            EndCombo.SelectedIndex = Math.Min(1, _vertices.Length - 1);
        }
    }

    private void BuildGraph_Click(object sender, RoutedEventArgs e)
    {
        if (!TryReadInputs(out var model, out string error))
        {
            StatusText.Text = "⚠️ " + error;
            return;
        }

        _vertices = model.Vertices;
        _A = model.A;

        _renderer!.Draw(model);
        StatusText.Text = "✅ Graph նկարվեց Canvas-ում";
    }

    private bool TryReadInputs(out GraphModel model, out string error)
    {
        model = null!;
        error = "";

        if (!int.TryParse(NBox.Text.Trim(), out int n) || n <= 0)
        {
            error = "Սխալ n";
            return false;
        }

        var vertices = VerticesService.BuildVertices(VerticesBox.Text, n);

        if (MatrixGrid.ItemsSource is not DataView dv)
        {
            error = "MatrixGrid-ը դատարկ է";
            return false;
        }

        if (!MatrixUiService.TryReadMatrix(dv, n, out var A, out error))
            return false;

        model = new GraphModel(vertices, A);
        return true;
    }

    private async void Run_Click(object sender, RoutedEventArgs e)
    {
        if (_vertices.Length == 0 || _A.Length == 0)
        {
            StatusText.Text = "⚠️ Սկզբում Build Graph սեղմիր";
            return;
        }

        int s = StartCombo.SelectedIndex;
        int t = EndCombo.SelectedIndex;

        if (s < 0 || t < 0)
        {
            StatusText.Text = "⚠️ Ընտրիր Start/End";
            return;
        }

        await _animator!.Run(s, t, delayMs: 650);
    }
}