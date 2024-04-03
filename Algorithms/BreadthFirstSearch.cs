using System.Collections;
using GraphUtil.Algorithms.Arguments;
using GraphUtil.Components;
using Index = GraphUtil.Pages.Index;

namespace GraphUtil.Algorithms;

public class BreadthFirstSearch : AlgorithmBase
{
    public override string Name => "Szélességi bejárás";

    public override string Description => """
                                          Alapvető gráfbejárási stratégia. Először bejárjuk a kezdőpont szomsédait ("1-es szintű szomszédok"), majd azoknak a szomszédait ("2-es szintű szomszédok"), és így tovább, amíg a gráf minden (kezdőpontból elérhető) pontjába el-nem jutottunk.
                                          <br>
                                          Minden pontot csak egyszer járunk be (ha nem körmentes a gráf, átugorjuk a már bejárt pontokat).
                                          <br>
                                          Gyökeres fa esetén (kezdőpont a gyökér) látható, hogy a csúcsokat "soronként" (szélesség) járjuk be.
                                          <br>
                                          A szélességi bejárás az adott gráfban (komponensben, ha a gráf nem összefüggő) egy feszítőfát hoz létre (bejárt élek - piros). Súlyozatlan gráf esetén ebben a fában az út a kezdőpont és egy másik P pont között megegyezik a gráfban az e két pont közti legrövidebb úttal*.
                                          <br>
                                          <hr>
                                          <h3>*Miért?</h3>
                                          Súlyozatlan gráfban minden él "egyenlő". Ebből következik, hogy (feltéve, hogy a kezdő- és végpont nem egyezik meg) a legjobb eset, ha a végpont a kezdőpont 1-es szintű z között van. Ha ez nem teljesül, a második legjobb eset, ha a végpont a kezdőpont 2-es szintű szomszédai között van (1 köztes csúcs), és így tovább.
                                          <br>
                                          A szélességi bejárás pont ezen stratégia alapján járja be a gráfot, tehát belátható, hogy a feszítőfában az út a kezdőpont és egy másik P pont között megegyzik a két pont közti legrövidebb úttal a gráfban.
                                          <br>
                                          <hr>
                                          <h3>Implementáció</h3>
                                          A bejárandó csúcsokat adjuk hozzá egy sorhoz (first in, first out), ebben a sorrendben járjuk be őket. (Sorba, Sorból)
                                          <br>
                                          <code>Állapot</code> mondja meg egy csúcs aktuális állapotát:
                                          <br>
                                          <ul>
                                            <li>0 - Alapeset, a csúcshoz még nem jutottunk el (fehér csúcsok)</li>
                                            <li>1 - A csúcsot már hozzáadtuk a sorhoz, még nem jártuk be a szomszédait (szürke csúcsok)</li>
                                            <li>2 - A csúcsot már kivettük a sorból, bejárjuk (vagy már bejártuk) a szomszédait (fekete csúcsok)</li>
                                          </ul>
                                          Az 1-es állapot csak a szemléltetést segíti, az algoritmus szempontjából helyettesíthető a 2-es állapottal (így akár Állapot értéke lehet logikai érték is, ami megmondja, hogy az adott csúcshoz el jutottunk-e már, vagy nem).
                                          <br>
                                          <code>Honnan</code> mondja meg, hogy az adott csúcsba melyik csúcsból léptünk be.
                                          <br>
                                          Futási idő: <strong>O(N + E)</strong>, ahol N a csúcsok, E pedig az élek száma.
                                          <br>
                                          <code class="psc">
                                            SzélességiBejárás(kezdőpont):<br>
                                                &emsp;Sorba(kezdőpont)<br>
                                                &emsp;Állapot(kezdőpont) := 1<br>
                                                &emsp;Ciklus amíg SorHossz() > 0<br>
                                                   &emsp;&emsp;P := Sorból()<br>
                                                   &emsp;&emsp;Ciklus minden Q-ra Szomszédok(P)-ben<br>
                                                        &emsp;&emsp;&emsp;Ha Állapot(Q) = 0<br>
                                                            &emsp;&emsp;&emsp;&emsp;Honnan(Q) := P<br>
                                                            &emsp;&emsp;&emsp;&emsp;Sorba(Q)<br>
                                                            &emsp;&emsp;&emsp;&emsp;Állapot(Q) := 1<br>
                                                        &emsp;&emsp;&emsp;Ha vége<br>
                                                   &emsp;&emsp;Ciklus vége<br>
                                                   &emsp;&emsp;Állapot(P) := 2<br>
                                                &emsp;Ciklus vége<br>
                                            Vége
                                          </code>
                                          """;

    public override AlgorithmArgumentBase[] Arguments => new AlgorithmArgumentBase[] { StartNode };

    private AlgorithmArgument<GraphNode> StartNode = new("Kezdőpont");

    public override bool ValidateArguments() => StartNode.Value != null;

    public override IEnumerator<StopLevel> Execute()
    {
        var queueText = "";
        StartNode.Value.Color = NodeColor.GRAY;
        CurrentEvent = $"Kezdőpont ({StartNode.Value.Text}) hozzáadása a sorhoz: {StartNode.Value.Text}";
        yield return StopLevel.Skippable;
        foreach (var node in BFSNodes(StartNode.Value, text => queueText = text))
        {
            if (node.Color == NodeColor.WHITE)
            {
                node.Color = NodeColor.GRAY;
                CurrentEvent = $"{node.Text} hozzáadaása a sorhoz: {queueText}";
                yield return StopLevel.Skippable;
                continue;
            }
            node.Color = NodeColor.BLACK;
            CurrentEvent = $"{node.Text} bejárása és kivétele a sorból: {queueText}";
            yield return StopLevel.Always;
        }
        StartNode.Value.Color = NodeColor.BLACK;
    }

    public static IEnumerable<GraphNode> BFSNodes(GraphNode startNode, Action<string>? updateQueueText = null)
    {
        var queue = new Queue<GraphNode>();
        queue.Enqueue(startNode);
        while (queue.Count > 0)
        {
            var p = queue.Dequeue();
            updateQueueText?.Invoke(queue.GetText());
            yield return p;
            foreach (var q in p.AdjNodes)
            {
                if (q.Node.Color != NodeColor.WHITE && q.Node.Color != NodeColor.GREEN)     //Bellman Ford algorithm sets visited nodes to green
                    continue;
                queue.Enqueue(q.Node);
                q.Node.SourceLink?.SetColor(NodeColor.BLACK);
                q.Link.SetColor(NodeColor.RED);
                updateQueueText?.Invoke(queue.GetText());
                yield return q.Node;
            }
        }
    }

    public BreadthFirstSearch(Index page) : base(page)
    {
    }
}