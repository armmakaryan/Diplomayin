namespace DijkstraShortestPath.UI.Services;

public static class VerticesService
{
    public static string[] BuildVertices(string a, int n)
    {
        var lines = (a ?? "")
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
            .Select(x => x.Trim())
            .Where(x => x.Length > 0)
            .ToList();

        while (lines.Count < n)
            lines.Add(((char)('A' + (lines.Count % 26))).ToString());

        return lines.Take(n).ToArray();
    }
}