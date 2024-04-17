using GraphUtil.Algorithms.Arguments;
using GraphUtil.Algorithms.Arguments.ArgumentTypes;
using GraphUtil.Components;
using Newtonsoft.Json;
using Index = GraphUtil.Pages.Index;

namespace GraphUtil.Algorithms;

public class AStar : AlgorithmBase
{
    private class DistanceComparer : IComparer<NodeDistanceInfo>
    {
        public int Compare(NodeDistanceInfo x, NodeDistanceInfo y)
        {
            if (x.CombinedDistance < y.CombinedDistance ||
                (x.CombinedDistance == y.CombinedDistance && x.DistanceToEnd < y.DistanceToEnd))
                return -1;
            if (x.CombinedDistance == y.CombinedDistance && x.DistanceToEnd == y.DistanceToEnd)
                return 0;
            return 1;
        }
    }

    public override string Name => "A*";

    public override string ShortDescription => "Legrövidebb út megkeresése két pont között";

    public override string Description => """
                                          Dijkstra algoritmusának optimalizációja két konkrét pont közti legrövidebb (legkisebb súlyösszegű) út megtalálására. A súlyok itt sem lehetek negatívak!
                                          <br>
                                          Az legrövidebb út gyorsabb meghatározásához szükségünk van egy heurisztikus függvényre (<code>H(p)</code>), ami egy <strong>becslést</strong> ad arról, hogy az adott pont milyen távol van a célponttól. Ez a függvény nem állandó, a felhasználó szükségletei szerint lehet változtatni. Gyakori ilyen függvény a pont és célpont távolsága légvonalban (euklideszi, manhattan távolság).
                                          <br>
                                          Fontos, hogy a függvény ne becsüljön magasabb értéket, mint ammenyi valós távolság lesz! A becsült érték lesz a pont <code>H</code> értéke. (A célpont <code>H</code> értéke 0, hiszen távolsága önmagától 0.)
                                          <br>
                                          A pontok <code>G</code> értéke megegyezik a Dijkstra-algoritmusban levővel: a pont távolsága a kezdőponttól.
                                          <br>
                                          Minden pontnak van még egy <code>F</code> értéke is: a pont <code>G</code> és <code>H</code> értékének összege: <strong>F(p) = G(p) + H(p)</strong>. Ez a ponton áthaladó legrövidebb út becsült távolsága a kezdőpont és célpont között. Az <code>F</code> érték lesz az az érték, ami alapján kiválasztjuk a bejárandó potot (legkisebb még nem bejárt <code>F</code> értékű). A legrövidebb út a kezdő- és célpont között az aktuális legrövidebb út a célpont bejárásnak idejében* (a célpont <code>F</code> értéke megegyezik a <code>G</code> értékével, hiszen <code>H</code> értéke 0).
                                          <br>Az algoritmus minden más tekintetben megegyezik Dijkstra algoritmusával. Dijkstra-algoritmusa tekinthető az A* algoritmusnak, aminek a heurisztikus függvénye konstans <strong>H(p) = 0</strong>.
                                          <hr>
                                          <h2>*Miért?</h2>
                                          Az algoritmus kikötése, hogy a súlyok nem negatívak, egy pont H értéke pedig nem nagyobb a tényleges célponttól való távolságánál.
                                          <br>
                                          Ebből következik, hogy <code>F</code> értéke sem lesz nagyobb, mint a tényleges legrövidebb út tálvolsága a kezdő- és célpont között az aktuális ponton áthaladva. Így ugyanúgy, mint Dijkstra algoritmusánál, belátható, hogy ha lenne egy másik olyan csúcs, amin áthaladva az út a célpontig rövidebb, annak az <code>F</code> értéke is kisebb, tehát előbb fogjuk bejárni.
                                          <hr>
                                          <h2>Implementáció</h2>
                                          Az implementáció szinte megegyezik Dijkstra alguritmusának implementációjával, néhány különbséggel:
                                          <ul>
                                          <li>A prioritási sorba a csúcsot az <code>F</code> értéke alapján soroljuk be (kezdőpont <code>H</code> értéke, hiszen <code>G</code> értéke 0). Az algoritmust megállítjuk, amikor a célpontot járjuk be, az aktuális legrövidebb út a tényleges legrövidebb út (zöld csúcsok).</li>
                                          <li>A heurisztikus függvénytől függően a cél- és kezdőpontot kivéve nem biztos, hogy amikor egy csúcsot bejárunk, akkor az aktuális út abba a csúcsba a legrövidebb, tehát többször is bekerülhet a prioritási sorba. Ezért nem tároljuk el a csúcsok különböző állapotát. (Ebben az alkalmazásban szemléltetés céljából a fekete csúcs az éppen bejárt csúcs, a szürke csúcsok pedig az prioritási sorban lévő csúcsok)
                                          </ul>
                                          <br>
                                          Az algoritmus futási ideje függ a heurisztikus függvény futási idejétől. Ha a <code>H(p)</code> futási ideje konstans (<strong>O(1)</strong>, pl. a programban lévő euklideszi távolság, manhattan távolság), az A* algoritmus futási ideje megegyezik Dijkstra algoritmusának futási idejével: <strong>O(N + E * log N)</strong>, ahol N a csúcsok, E pedig az élek száma.
                                          <code class="psc">
                                              ACsillag(kezdőpont, célpont):<br>
                                                &emsp;Távolság := [-1 minden P csúcsra]<br>
                                                &emsp;Távolság(kezdőpont) := 0<br>
                                                &emsp;PrSorba(kezdőpont, H(kezdőpont))<br>
                                                &emsp;Amíg PrSorElemszám() > 0:<br>
                                                    &emsp;&emsp;P := PrSorból()<br>
                                                        &emsp;&emsp;Ha P = célpont:<br>
                                                            &emsp;&emsp;&emsp;Megáll<br>
                                                        &emsp;&emsp;Ha vége<br>
                                                        &emsp;&emsp;Ciklus minden (Q, Súly)-ra Szomszédok(P)-ben:<br>
                                                            &emsp;&emsp;&emsp;G := Távolság(P) + Súly<br>
                                                            &emsp;&emsp;&emsp;Ha Távolság(Q) = -1 vagy G < Távolság(Q):<br>
                                                                &emsp;&emsp;&emsp;&emsp;Távolság(Q) := G<br>
                                                                &emsp;&emsp;&emsp;&emsp;Honnan(Q) := P<br>
                                                                &emsp;&emsp;&emsp;&emsp;F := G + H(Q)<br>
                                                                &emsp;&emsp;&emsp;&emsp;PrSorba(Q, F)<br>
                                                            &emsp;&emsp;&emsp;Ha vége<br>
                                                        &emsp;&emsp;Ciklus vége<br>
                                                &emsp;Amíg vége<br>
                                              Vége
                                          </code>
                                          <hr>
                                          Az alkalmazásban az A* algoritmusnak megadható saját heurisztikus függvény JavaScript nyelven. A függvény két paramétere az aktuális csúcs és a célpont. Ezek <code>NodeInfo</code> objektumok, aminek 4 adatához lehet hozzáférni:
                                          <ul>
                                            <li><code>id</code> - A csúcs azonosítója</li>
                                            <li><code>g</code> - A csúcs <code>G</code> értéke (ha ismert (az aktuális csúcsnál), egyébként <code>null</code>)</li>
                                            <li><code>x</code> - A csúcs X koordinátája</li>
                                            <li><code>y</code> - A csúcs Y koordinátája</li>
                                          </ul>
                                          <hr>
                                          <strong>Felhasznált irodalom:</strong>
                                          <br>
                                          <a target="_blank" href="https://en.wikipedia.org/wiki/A*_search_algorithm">Wikipedia: A* search algorithm</a>
                                          """;

    public override AlgorithmArgumentBase[] Arguments => new AlgorithmArgumentBase[] { StartNode, EndNode, HeuristicScript };

    private AlgorithmArgument<GraphNode> StartNode = new("Kezdőpont");
    private AlgorithmArgument<GraphNode> EndNode = new("Célpont");
    private ScriptArgument HeuristicScript = new(ScriptID.AStarHeuristics, "Heurisztika", "currentNode", "endNode");

    private readonly ScriptArgument EuclideanArgument;

    public override bool ValidateArguments()
    {
        if (StartNode.Value == null || EndNode.Value == null || StartNode.Value == EndNode.Value ||
            string.IsNullOrEmpty(HeuristicScript.FunctionName) ||
            HeuristicScript.FunctionName == "new" || HeuristicScript.FunctionName == "új" ||
            string.IsNullOrEmpty(HeuristicScript.ReturnExpression))
            return false;
        if (HeuristicScript.FunctionName == "euklidesz" || HeuristicScript.FunctionName == "manhattan" ||
            HeuristicScript.FunctionName == "euklideszSzazalek" || HeuristicScript.FunctionName == "manhattanSzazalek")
            return true;
        if (!Page.JS.Invoke<bool>("createAlgorithm", HeuristicScript.FullFunctionName, HeuristicScript.Compiled))
            return false;
        if (!ScriptArgument.PredefinedScripts[ScriptID.AStarHeuristics].ContainsKey(HeuristicScript.FunctionName))
            ScriptArgument.PredefinedScripts[ScriptID.AStarHeuristics]
                .Add(HeuristicScript.FunctionName, HeuristicScript.Finalize());
        Page.JS.Invoke<object?>("localStorage.setItem", "scripts", JsonConvert.SerializeObject(ScriptArgument.PredefinedScripts));
        return true;
    }

    private Func<NodeInfo, NodeInfo, double>? HeuristicsFunction;

    private double heuristics_euclidean(NodeInfo currentNode, NodeInfo endNode)
    {
        var dx = endNode.X - currentNode.X;
        var dy = endNode.Y - currentNode.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    private double heuristics_manhattan(NodeInfo currentNode, NodeInfo endNode) =>
        Math.Abs(endNode.X - currentNode.X) + Math.Abs(endNode.Y - currentNode.Y);

    private double heuristics_euclidean_percent(NodeInfo currentNode, NodeInfo endNode)
    {
        return heuristics_euclidean(currentNode, endNode) / 100.0;
    }

    private double heuristics_manhattan_percent(NodeInfo currentNode, NodeInfo endNode)
    {
        return heuristics_manhattan(currentNode, endNode) / 100.0;
    }

    private double ExecuteHeuristics(NodeInfo currentNode, NodeInfo endNode)
    {
        if(HeuristicsFunction != null)
            return HeuristicsFunction(currentNode, endNode);
        try
        {
            return Page.JS.Invoke<double>(HeuristicScript.FullFunctionName, currentNode, endNode);
        }
        catch (Exception e)
        {
            Page.JS.Invoke<object?>("console.error",
                $"Hiba történt a heurisztikus függvény futtatása közben\n{e.GetType().Name} {e.Message}");
            return 0.0;
        }
    }

    public override IEnumerator<StopLevel> Execute()
    {
        HeuristicsFunction = HeuristicScript.FunctionName switch
        {
            "euklidesz" => heuristics_euclidean,
            "manhattan" => heuristics_manhattan,
            "euklideszSzazalek" => heuristics_euclidean_percent,
            "manhattanSzazalek" => heuristics_manhattan_percent,
            _ => null
        };
        var queue = new PrintablePriorityQueue<GraphNode, NodeDistanceInfo>(new DistanceComparer());
        StartNode.Value.DistanceInfo.DistanceFromStart = 0.0;
        StartNode.Value.DistanceInfo.DistanceToEnd = ExecuteHeuristics(StartNode.Value.Info, EndNode.Value.Info);
        StartNode.Value.InQueue++;
        queue.Enqueue(StartNode.Value, StartNode.Value.DistanceInfo.Copy());
        CurrentEvent = $"Kezdőpont ({StartNode.Value.Text}) hozzáadása a prioritási sorhoz: {queue.Text}";
        yield return StopLevel.Skippable;
        while (queue.TryDequeue(out var currentNode, out var currentDistance))
        {
            if (currentDistance.DistanceFromStart > currentNode.DistanceInfo.DistanceFromStart)
            {
                CurrentEvent =
                    $"{currentNode.Text} (F={currentDistance}) kivétele a prioritási sorból és átugrása (már találtunk jobb utat). Prioritási sor: {queue.Text}";
                yield return StopLevel.Skippable;
                continue;
            }
            if (currentNode == EndNode.Value)
            {
                currentNode.Color = NodeColor.GREEN;
                CurrentEvent = $"Elérkeztünk a célponthoz ({EndNode.Value.Text}) a prioritási sorban";
                yield return StopLevel.Always;
                do
                {
                    currentNode.Color = NodeColor.GREEN;
                    currentNode = currentNode.SourceNode;
                } while (currentNode != StartNode.Value);
                StartNode.Value.Color = NodeColor.GREEN;
                yield break;
            }
            currentNode.Color = NodeColor.BLACK;
            CurrentEvent = $"{currentNode.Text} (F={currentDistance}) bejárása és kivétele a prioritási sorból: {queue.Text}";
            yield return StopLevel.Always;
            foreach (var adjNode in currentNode.AdjNodes)
            {
                var distanceFromStart = currentNode.DistanceInfo.DistanceFromStart + adjNode.Link.Weight;
                if (adjNode.Node.DistanceInfo.DistanceFromStart == null ||
                    distanceFromStart < adjNode.Node.DistanceInfo.DistanceFromStart)
                {
                    adjNode.Link.Color = NodeColor.RED;
                    if (adjNode.Node.SourceLink != null)
                        adjNode.Node.SourceLink.Color = NodeColor.BLACK;
                    adjNode.Node.SourceNode = currentNode;
                    adjNode.Node.SourceLink = adjNode.Link;
                    adjNode.Node.DistanceInfo.DistanceFromStart = distanceFromStart;
                    adjNode.Node.DistanceInfo.DistanceToEnd = ExecuteHeuristics(adjNode.Node.Info, EndNode.Value.Info);
                    adjNode.Node.Color = NodeColor.GRAY;
                    adjNode.Node.InQueue++;
                    queue.Enqueue(adjNode.Node, adjNode.Node.DistanceInfo.Copy());
                    CurrentEvent =
                        $"{adjNode.Node.Text} (F={adjNode.Node.DistanceInfo}) hozzáadása a prioritási sorhoz: {queue.Text}";
                }
                else CurrentEvent = $"{adjNode.Node.Text} (F={adjNode.Node}) G értéke ({distanceFromStart}) nem jobb, mint az aktuális ({adjNode.Node.DistanceInfo.DistanceFromStart}), nem adjuk hozzá a prioritási sorhoz. Prioritási sor: {queue.Text}";
                yield return StopLevel.Skippable;
            }
            currentNode.Color = --currentNode.InQueue == 0 ? NodeColor.WHITE : NodeColor.GRAY;
        }
    }

    public override void Reset()
    {
        base.Reset();
        HeuristicScript.CopyFrom(EuclideanArgument);
    }

    public AStar(Index page) : base(page)
    {
        EuclideanArgument = new ScriptArgument(ScriptID.AStarHeuristics, "Heurisztika", "currentNode", "endNode")
        {
            FunctionName = "euklidesz",
            FunctionBody = "let dx = endNode.x - currentNode.x\nlet dy = endNode.y - currentNode.y",
            ReturnExpression = "Math.sqrt(dx * dx + dy * dy)"
        }.Finalize();
        HeuristicScript.CopyFrom(EuclideanArgument);
        if (!ScriptArgument.PredefinedScripts[ScriptID.AStarHeuristics].ContainsKey("euklidesz"))
            ScriptArgument.PredefinedScripts[ScriptID.AStarHeuristics].Add("euklidesz", EuclideanArgument.Finalize());
        
        if (!ScriptArgument.PredefinedScripts[ScriptID.AStarHeuristics].ContainsKey("manhattan"))
            ScriptArgument.PredefinedScripts[ScriptID.AStarHeuristics].Add("manhattan",
                new ScriptArgument(ScriptID.AStarHeuristics, "Heurisztika", "currentNode", "endNode")
                {
                    FunctionName = "manhattan",
                    FunctionBody = "let dx = endNode.x - currentNode.x\nlet dy = endNode.y - currentNode.y",
                    ReturnExpression = "Math.abs(dx) + Math.abs(dy)"
                }.Finalize());
        
        if(!ScriptArgument.PredefinedScripts[ScriptID.AStarHeuristics].ContainsKey("euklideszSzazalek"))
            ScriptArgument.PredefinedScripts[ScriptID.AStarHeuristics].Add("euklideszSzazalek", new ScriptArgument(ScriptID.AStarHeuristics, "Heurisztika", "currentNode", "endNode")
            {
                FunctionName = "euklideszSzazalek",
                FunctionBody = "let dx = endNode.x - currentNode.x\nlet dy = endNode.y - currentNode.y",
                ReturnExpression = "Math.sqrt(dx * dx + dy * dy) / 100.0"
            }.Finalize());
        
        if(!ScriptArgument.PredefinedScripts[ScriptID.AStarHeuristics].ContainsKey("manhattanSzazalek"))
            ScriptArgument.PredefinedScripts[ScriptID.AStarHeuristics].Add("manhattanSzazalek", new ScriptArgument(ScriptID.AStarHeuristics, "Heurisztika", "currentNode", "endNode")
            {
                FunctionName = "manhattanSzazalek",
                FunctionBody = "let dx = endNode.x - currentNode.x\nlet dy = endNode.y - currentNode.y",
                ReturnExpression = "(Math.abs(dx) + Math.abs(dy)) / 100.0"
            }.Finalize());
    }
}