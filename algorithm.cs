using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.InputEncoding  = System.Text.Encoding.UTF8;

        Console.Write("Մուտքագրիր գագաթների քանակը (n)՝ ");
        int n = int.Parse(Console.ReadLine());

        string[] vertices = new string[n];
        for (int i = 0; i < n; i++)
        {
            Console.Write($"Գագաթ {i + 1}-ի անունը՝ ");
            vertices[i] = Console.ReadLine().Trim();
        }


        double[,] A = new double[n, n];
        Console.WriteLine("\nՄուտքագրիր կշիռների մատրիցան (0՝ եթե չկա ճանապարհ)․");

        for (int i = 0; i < n; i++)
        {
            Console.WriteLine($"\n{vertices[i]} → մյուս գագաթներ");
            for (int j = 0; j < n; j++)
            {
                Console.Write($"Կշիռ {vertices[i]} → {vertices[j]} = ");
                A[i, j] = double.Parse(Console.ReadLine());
            }
        }

        Console.WriteLine("\nԳագաթների ցանկը՝ " + string.Join(", ", vertices));

        string start, end;
        while (true)
        {
            Console.Write("Սկզբնակետը՝ ");
            start = Console.ReadLine().Trim();
            if (Array.IndexOf(vertices, start) == -1)
                Console.WriteLine("⚠️ Սխալ գագաթ");
            else break;
        }

        while (true)
        {
            Console.Write("Վերջնակետը՝ ");
            end = Console.ReadLine().Trim();
            if (Array.IndexOf(vertices, end) == -1)
                Console.WriteLine("⚠️ Սխալ գագաթ");
            else break;
        }

        int s = Array.IndexOf(vertices, start);
        int t = Array.IndexOf(vertices, end);

        double INF = double.PositiveInfinity;
        double[] dist = new double[n];
        int?[] prev = new int?[n];
        bool[] used = new bool[n];

        for (int i = 0; i < n; i++)
            dist[i] = INF;

        dist[s] = 0;

        for (int k = 0; k < n; k++)
        {
            int u = -1;
            double best = INF;

            for (int i = 0; i < n; i++)
                if (!used[i] && dist[i] < best)
                {
                    best = dist[i];
                    u = i;
                }

            if (u == -1) break;

            used[u] = true;

            for (int v = 0; v < n; v++)
            {
                if (A[u, v] > 0 && !used[v])
                {
                    double nd = dist[u] + A[u, v];
                    if (nd < dist[v])
                    {
                        dist[v] = nd;
                        prev[v] = u;
                    }
                }
            }
        }

        if (double.IsPositiveInfinity(dist[t]))
        {
            Console.WriteLine("❌ Ճանապարհ չկա");
            return;
        }

        List<string> path = new List<string>();
        for (int? cur = t; cur != null; cur = prev[cur.Value])
            path.Add(vertices[cur.Value]);

        path.Reverse();

        Console.WriteLine("\n🟢 Ամենակարճ ճանապարհը՝");
        Console.WriteLine(" ➤ " + string.Join(" ➤ ", path));
        Console.WriteLine($"📏 Ընդհանուր երկարությունը՝ {dist[t]:0.0}");
    }
}