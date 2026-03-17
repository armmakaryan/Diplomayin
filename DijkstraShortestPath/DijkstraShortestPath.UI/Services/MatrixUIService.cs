using System.Data;
using System.Globalization;

namespace DijkstraShortestPath.UI.Services;


public static class MatrixUiService
{
    public static DataView CreateMatrixView(string[] vertices)
    {
        int n = vertices.Length;

        DataTable dt = new DataTable();
        for (int j = 0; j < n; j++)
            dt.Columns.Add(vertices[j], typeof(string));

        for (int i = 0; i < n; i++)
        {
            var row = dt.NewRow();
            for (int j = 0; j < n; j++) row[j] = "0";
            dt.Rows.Add(row);
        }

        return dt.DefaultView;
    }

    public static bool TryReadMatrix(DataView dv, int n, out double[,] A, out string error)
    {
        A = new double[0, 0];
        error = "";

        if (dv == null)
        {
            error = "MatrixGrid-ը դատարկ է";
            return false;
        }

        if (dv.Table.Rows.Count != n || dv.Table.Columns.Count != n)
        {
            error = "Matrix չափերը չեն համապատասխանում n-ին";
            return false;
        }

        A = new double[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                string s = (dv.Table.Rows[i][j]?.ToString() ?? "0").Trim().Replace(',', '.');

                if (!double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double w))
                {
                    error = $"Matrix[{i},{j}] սխալ թիվ է";
                    return false;
                }
                if (w < 0)
                {
                    error = "Քաշերը պետք է լինեն >= 0";
                    return false;
                }

                A[i, j] = w;
            }
        }

        return true;
    }
}
