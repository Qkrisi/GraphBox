namespace GraphUtil.Pages;

public partial class Index
{
    private static readonly Dictionary<string, string> TemplateGraphs = new()
    {
        {"Szélességi/Mélységi bejárás", "1 2\n2 1\n2 3\n3 2\n3 4\n4 3\n3 5\n5 3\n2 6\n6 2\n6 7\n7 6\n1 8\n8 1\n8 9\n9 8\n9 10\n10 9\n9 11\n11 9\n1 6\n6 1\n8 4\n4 8"},
        {"Dijkstra/A*", "1 2 11\n2 1 11\n5 8 1\n8 5 1\n1 3 11\n3 1 11\n1 4 5\n4 1 5\n2 5 11\n5 2 11\n6 7 1\n7 6 1\n3 6 1\n6 3 1\n4 7 11\n7 4 11\n4 8 11\n8 4 11\n4 5 20\n5 4 20\n8 7 11\n7 8 11\n11 7 10\n7 11 10\n8 12 11\n12 8 11\n9 10 1\n10 9 1\n11 10 11\n10 11 11"},
        {"Bellman-Ford", "1 2 11\n5 8 -1\n1 3 -11\n1 4 1\n2 5 -11\n6 7 -1\n3 6 1\n4 7 -11\n4 8 -11\n4 5 -1\n8 7 11\n11 7 -10\n8 12 -11\n9 10 1\n11 10 11\n13 9 1\n13 11 1\n13 1 1\n8 2 1"},
        {"Dominátor halmaz", "3 2\n4 3\n1 3\n2 6\n2 4\n6 10\n6 11\n2 11\n5 7\n5 8\n5 4\n7 8\n13 12\n12 11"},
        {"Kosaraju", "1 2\n2 3\n3 1\n1 4\n4 5\n5 6\n6 4\n7 5\n9 7\n6 9\n9 5\n8 3\n2 10\n10 8\n1 14\n14 12\n14 11\n11 13\n13 12\n11 1"},
        {"Kruskal", "16 1 4\n1 16 4\n1 2 2\n2 1 2\n16 2 4\n2 16 4\n2 3 3\n3 2 3\n2 4 4\n4 2 4\n2 5 2\n5 2 2\n3 4 3\n4 3 3\n4 5 3\n5 4 3\n10 11 5\n11 10 5\n11 12 3\n12 11 3\n10 12 5\n12 10 5\n12 13 4\n13 12 4\n12 14 5\n14 12 5\n12 15 3\n15 12 3\n13 14 4\n14 13 4\n14 15 4\n15 14 4\n4 11 100\n11 4 100"},
        {"Elvágó élek, pontok", "1 2\n2 1\n2 3\n3 2\n3 4\n4 3\n2 4\n4 2\n2 5\n5 2\n5 6\n6 5\n6 7\n7 6\n5 7\n7 5\n6 8\n8 6\n5 8\n8 5\n1 9\n9 1\n9 10\n10 9\n9 11\n11 9\n9 12\n12 9\n12 13\n13 12\n9 13\n13 9\n13 14\n14 13\n1 14\n14 1"}
    };
}