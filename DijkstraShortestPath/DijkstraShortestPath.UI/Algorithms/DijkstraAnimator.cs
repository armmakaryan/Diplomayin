using DijkstraShortestPath.UI.Rendering;
using System.Windows.Media;

namespace DijkstraShortestPath.UI.Algorithms;

public sealed class DijkstraAnimator
{
    private readonly IGraphRenderer _r;

    public DijkstraAnimator(IGraphRenderer renderer)
    {
        _r = renderer;
    }

    public async Task Run(int s, int t, int delayMs)
    {
        _r.ResetVisuals();

        int n = _r.VertexCount;
        double INF = double.PositiveInfinity;

        double[] dist = Enumerable.Repeat(INF, n).ToArray();
        int?[] prev = new int?[n];
        bool[] used = new bool[n];

        dist[s] = 0;

        _r.HighlightNode(s, Brushes.DodgerBlue);
        _r.SetStatus($"Start = {_r.VertexName(s)}, End = {_r.VertexName(t)}");
        await Task.Delay(delayMs);

        for (int step = 0; step < n; step++)
        {
            int u = -1;
            double best = INF;

            for (int i = 0; i < n; i++)
            {
                if (!used[i] && dist[i] < best)
                {
                    best = dist[i];
                    u = i;
                }
            }

            if (u == -1) break;

            used[u] = true;
            _r.HighlightNode(u, Brushes.Orange);
            _r.SetStatus($"Ընտրվեց u = {_r.VertexName(u)} (dist={FormatDist(dist[u])})");
            await Task.Delay(delayMs);

            for (int v = 0; v < n; v++)
            {
                double w = _r.Weight(u, v);
                if (w <= 0 || used[v] || u == v) continue;

                _r.HighlightEdge(u, v, Brushes.MediumPurple, 4);
                await Task.Delay(delayMs / 2);

                double nd = dist[u] + w;
                if (nd < dist[v])
                {
                    dist[v] = nd;
                    prev[v] = u;

                    _r.HighlightNode(v, Brushes.LightGreen);
                    _r.SetStatus($"Թուլացում՝ {_r.VertexName(u)}→{_r.VertexName(v)} , dist[{_r.VertexName(v)}]={FormatDist(dist[v])}");
                }
                else
                {
                    _r.SetStatus($"Չի բարելավվում՝ {_r.VertexName(u)}→{_r.VertexName(v)}");
                }

                await Task.Delay(delayMs);
                _r.HighlightEdge(u, v, Brushes.LightGray, 2);
            }

            _r.HighlightNode(u, Brushes.Gray);
            await Task.Delay(delayMs / 2);

            if (u == t) break;
        }

        if (double.IsPositiveInfinity(dist[t]))
        {
            _r.SetStatus("❌ Ճանապարհ չկա");
            _r.HighlightNode(t, Brushes.Red);
            return;
        }

        var path = new List<int>();
        for (int? cur = t; cur != null; cur = prev[cur.Value]) path.Add(cur.Value);
        path.Reverse();

        _r.SetStatus($"✅ Ամենակարճը՝ {string.Join(" → ", path.Select(_r.VertexName))} | Length={dist[t]:0.##}");

        for (int i = 0; i < path.Count; i++)
        {
            _r.HighlightNode(path[i], Brushes.Gold);
            if (i + 1 < path.Count) _r.HighlightEdge(path[i], path[i + 1], Brushes.Gold, 5);
            await Task.Delay(delayMs);
        }
    }

    private static string FormatDist(double d) =>
        double.IsPositiveInfinity(d) ? "INF" : d.ToString("0.##");
}