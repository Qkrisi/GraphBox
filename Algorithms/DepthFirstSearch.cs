using System.Collections;
using GraphUtil.Algorithms.Arguments;
using GraphUtil.Components;
using Index = GraphUtil.Pages.Index;

namespace GraphUtil.Algorithms;

public class DepthFirstSearch : AlgorithmBase
{
    public override string Name => "Mélységi bejárás";

    public override string Description => """
                                          Alapvető gráfbejárási stratégia. A vizsgált pontból egyből továbblépünk az egyik (még nem bejárt) szomszédjába, majd ezt folytatjuk addig, amíg "zsákutcába" nem jutunk.
                                          <br>
                                          Ilyenkor visszamegyünk a legközelebbi olyan csúcsig, aminek van még nem bejárt szomszédja, amelyen tovább tudunk menni.
                                          <br>
                                          Gyökeres fa esetén látható, hogy a gráfot áganként ("mélységében") járjuk be.
                                          <br>
                                          A mélységi bejárás az adott gráfban (komponensben, ha a gráf nem összefüggő) egy feszítőfát hoz létre (bejárt élek - piros). 
                                          <hr>
                                          <h2>Implementáció</h2>
                                          Mélységi bejárást rekurzívan tudjuk implementálni. A rekurzív eljárás a gráf egyik csúcsának bejárása: a csúcs minden még nem bejárt szomszédjára meghívja önmagát a szomszéddal aktuális csúcsként.
                                          <br>
                                          <code>Állapot</code> mondja meg egy csúcs aktuális állapotát:
                                          <br>
                                          <ul>
                                            <li>0 - Alapeset, a csúcshoz még nem jutottunk el (fehér csúcsok)</li>
                                            <li>1 - A csúcshoz már eljutottunk, de még nem jártuk be minden tőle induló ágat, még nem léptünk ki belőle (szürke csúcsok)</li>
                                            <li>2 - A csúcsot és a tőle induló ágakat is bejártuk, kiléptünk belőle (fekete csúcsok)</li>
                                          </ul>
                                          Az 1-es állapot csak a szemléltetést segíti, az algoritmus szempontjából helyettesíthető a 2-es állapottal (így akár Állapot értéke lehet logikai érték is, ami megmondja, hogy az adott csúcshoz el jutottunk-e már, vagy nem).
                                          <br>
                                          <code>Honnan</code> mondja meg, hogy az adott csúcsba melyik csúcsból léptünk be.
                                          <br>
                                          <code>MélységiBejárás</code> első hívása a kezdőponttal történik.
                                          <br>
                                          Futási idő: <strong>O(N + E)</strong>, ahol N a csúcsok, E pedig az élek száma.
                                          <br>
                                          <code class="psc">
                                          MélységiBejárás(pont):<br>
                                            &emsp;Állapot(pont) := 1<br>
                                            &emsp;Ciklus minden Q-ra Szomszédok(pont)-ban:<br>
                                                &emsp;&emsp;Ha Állapot(Q) = 0<br>
                                                    &emsp;&emsp;&emsp;Honnan(Q) := pont<br>
                                                    &emsp;&emsp;&emsp;MélységiBejárás(Q)<br>
                                                &emsp;&emsp;Ha vége<br>
                                            &emsp;Ciklus vége<br>
                                            &emsp;Állapot(pont) := 2<br>
                                          Vége
                                          </code>
                                          <hr>
                                          <strong>Felhasznált irodalom:</strong>
                                          <br>
                                          <a target="_blank" href="http://tehetseg.inf.elte.hu/szakkor/2023/grafok-1.pptx">ELTE: Gráfok, Gráfalgoritmusok I.</a>
                                          """;

    public override AlgorithmArgumentBase[] Arguments => new AlgorithmArgumentBase[] { StartNode };

    private AlgorithmArgument<GraphNode> StartNode = new("Kezdőpont");

    public override bool ValidateArguments() => StartNode.Value != null;

    public IEnumerator ExecuteInner(GraphNode node)
    {
        node.Color = NodeColor.GRAY;
        CurrentEvent = $"Belépés pontba: {node.Text}";
        yield return StopLevel.Always;
        foreach (var q in node.AdjNodes)
        {
            if (q.Node.Color != NodeColor.WHITE)
                continue;
            q.Link.SetColor(NodeColor.RED);
            yield return ExecuteInner(q.Node);
        }

        node.Color = NodeColor.BLACK;
        CurrentEvent = $"Kilépés pontból: {node.Text}";
        yield return StopLevel.Always;
    }

    public override IEnumerator<StopLevel> Execute() => AlgorithmWrapper(ExecuteInner(StartNode.Value));

    public DepthFirstSearch(Index page) : base(page)
    {
    }
}