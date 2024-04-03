using GraphUtil.Components;
using GraphUtil.Algorithms.Arguments;
using Index = GraphUtil.Pages.Index;

namespace GraphUtil.Algorithms;

public class Dijkstra : AlgorithmBase
{
    public override string Name => "Dijkstra";
    public override string FullName => "Dijkstra algoritmusa";

    public override string ShortDescription => "Legrövidebb út megkeresése egy kezdőponttól a többi pontig (negatív élek nem megengedettek)";

    public override string Description => """
                                          Alap stratégia: szélességi bejárás
                                          <br>
                                          Ez az algoritmus megadja egy súlyozott gráfban a legrövidebb távolságot és utat egy adott kezdőpont és minden másik (kezdőpontból elérhető) csúcs között. A legrövidebb út itt azt jelenti, hogy az út élei súlyának összege a lehető legkisebb. Az élek súlya nem lehet negatív!
                                          <br>
                                          Minden csúcsnak tároljuk el az aktuálisan ismert legrövidebb kezdőponttól hozzá vezető út hosszát (<code>G</code> érték). Alapállapotban ez az érték legyen minden csúcsnak végtelen, kivéve a kezdőpontnak, aminek 0 (hiszen önmagától a távolsága 0).
                                          <br>
                                          Amíg van olyan csúcs aminek a <code>G</code> értéke nem végtelen és még nem jártuk be, járjuk be ezek közül a legkisebb <code>G</code> értékűt (első lépésben a kezdőpont): legyen ez a P pont. ellenőrizzük le P minden még nem bejárt Q szomszédját.
                                          <br>
                                          Ilyenkor Q távolsága P-n keresztül P-nek a <code>G</code> értéke és a két csúcsot összekötő él súlyának összege. Ha ez az érték kisebb, mint Q aktuális <code>G</code> értéke, állítsuk be ezt az új <code>G</code> értéknek. Ilyenkor Q-t még nem jártuk be (P-t igen)!
                                          <br>
                                          A kezdőpont és bármely másik P csúcs közötti legrövidebb út az aktuális legrövidebb út P bejárásnak idejében*. Ha egy konkrét célponthoz keressük a legrövidebb utat, a célpont bejárásakor az algoritmust meg lehet állítani.
                                          <hr>
                                          <h2>*Miért?</h2>
                                          Az algoritmus kikötése, hogy egyik él súlya sem negatív. Mivel mindig a legkisebb még be nem járt <code>G</code> értékű csúcsot járjuk be, tudjuk, hogy nincs másik csúcs, aminek még nem ellenőriztük a szomszédait és <code>G</code> értéke (aktuális legjobb távolsága a kezdőponttól) kisebb.
                                          <br>
                                          Ebből következik, hogy minden más csúcsnak, ahonnan esetlegesen el lehetne még jutni az aktuális csúcshoz <code>G</code> értéke nagyobb vagy egyenlő.
                                          <br>
                                          És mivel az élek súlya nem negatív, az ezekből a csúcsokból továbbvezető utak hossza sem lehet kisebb ennél a <code>G</code>-nél.
                                          <hr>
                                          <h2>Implementáció</h2>
                                          Ahhoz, hogy gyorsan meg tudjuk keresni a legkisebb <code>G</code> értékű csúcsot, használjunk prioritási sort (a legtöbb programozási nyelv standard könyvtárában megtalálható). Ez az adatszerkezet logaritmikus időben (elem hozzáadásának és törlésének ideje, <strong>O(log N)</strong>) meg tudja adni a hozzáadott adatsor szélső értékét, míg manuálisan végigmenni egy listán lineáris lenne (<strong>O(N)</strong>).
                                          <br>
                                          Prioritási sor műveletei: <code>PrSorba</code>, <code>PrSorból</code>, <code>PrSorElemszám</code>
                                          <br>
                                          A kezdeti végtelen érték az implementációban lehet bármilyen érték, amiről meg tudjuk mondani, hogy az adott pontban még nem jártunk (null, -1, súlyok összege+1, maximum egészérték, stb.). Az alábbi implementációban <code>-1</code>-et használunk.
                                          <br>
                                          <code>Honnan</code> adja meg, hogy az adott csúcsba melyik csúcsból jutottunk el a hozzá vezető legrövidebb úton (piros élek).
                                          <br>
                                          <code>Állapot</code> mondja meg egy csúcs aktuális állapotát:
                                          <br>
                                          <ul>
                                            <li>0 - Alapeset, a csúcshoz még nem jutottunk el (fehér csúcsok)</li>
                                            <li>1 - A csúcsot már hozzáadtuk a prioritási sorhoz, de még nem jártuk be(szürke csúcsok)</li>
                                            <li>2 - A csúcsot már bejártuk (fekete csúcsok)</li>
                                          </ul>
                                          <br>
                                          Futási idő: <strong>O(N + E * log N)</strong>, ahol N a csúcsok, E pedig az élek száma.
                                          <br>
                                          <code class="psc">
                                          Dijkstra(kezdőpont):<br>
                                            &emsp;Távolság := [-1 minden P csúcsra]<br>
                                            &emsp;Távolság(kezdőpont) := 0<br>
                                            &emsp;PrSorba(kezdőpont)<br>
                                            &emsp;Állapot(kezdőpont) := 1<br>
                                            &emsp;Amíg PrSorElemszám() > 0:<br>
                                                &emsp;&emsp;P := PrSorból()<br>
                                                &emsp;&emsp;Ha Állapot(P) = 1:<br>
                                                    &emsp;&emsp;&emsp;Állapot(P) := 2<br>
                                                    &emsp;&emsp;&emsp;Ciklus minden (Q, Súly)-ra Szomszédok(P)-ben:<br>
                                                        &emsp;&emsp;&emsp;&emsp;Ha Állapot(Q) < 2:<br>
                                                            &emsp;&emsp;&emsp;&emsp;&emsp;Állapot(Q) := 1<br>
                                                            &emsp;&emsp;&emsp;&emsp;&emsp;G := Távolság(P) + Súly<br>
                                                            &emsp;&emsp;&emsp;&emsp;&emsp;Ha Távolság(Q) = -1 vagy G < Távolság(Q):<br>
                                                                &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;Távolság(Q) := G<br>
                                                                &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;Honnan(Q) := P<br>
                                                                &emsp;&emsp;&emsp;&emsp;&emsp;&emsp;PrSorba(Q, G)<br>
                                                            &emsp;&emsp;&emsp;&emsp;&emsp;Ha vége<br>
                                                        &emsp;&emsp;&emsp;&emsp;Ha vége<br>
                                                    &emsp;&emsp;&emsp;Ciklus vége<br>
                                                &emsp;&emsp;Ha vége<br>
                                            &emsp;Amíg vége<br>
                                          Vége
                                          </code>
                                          """;

    public override AlgorithmArgumentBase[] Arguments => new AlgorithmArgumentBase[] { StartNode };

    private AlgorithmArgument<GraphNode> StartNode = new("Kezdőpont");

    public override bool ValidateArguments() => StartNode.Value != null;

    public override IEnumerator<StopLevel> Execute()
    {
        var queue = new PrintablePriorityQueue<GraphNode, double>();
        StartNode.Value.DistanceInfo.DistanceFromStart = 0.0;
        StartNode.Value.Color = NodeColor.GRAY;
        queue.Enqueue(StartNode.Value, 0.0);
        CurrentEvent = $"Kezdőpont ({StartNode.Value.Text}) hozzáadása a prioritási sorhoz: {queue.Text}";
        yield return StopLevel.Skippable;
        while (queue.TryDequeue(out var p, out var distance))
        {
            if (distance > p.DistanceInfo.DistanceFromStart)
            {
                CurrentEvent =
                    $"{p.Text} (G={distance}) kivétele a prioritási sorból és átugrása (már találtunk jobb utat). Prioritási sor: {queue.Text}";
                yield return StopLevel.Skippable;
                continue;
            }
            p.Color = NodeColor.BLACK;
            CurrentEvent = $"{p.Text} (G={distance}) bejárása és kivétele a prioritási sorból: {queue.Text}";
            yield return StopLevel.Always;
            foreach (var adjNode in p.AdjNodes)
            {
                if(adjNode.Node.Color == NodeColor.BLACK)
                    continue;
                adjNode.Node.Color = NodeColor.GRAY;
                var dist2 = distance + adjNode.Link.Weight;
                if (adjNode.Node.DistanceInfo.DistanceFromStart == null || dist2 < adjNode.Node.DistanceInfo.DistanceFromStart)
                {
                    adjNode.Node.DistanceInfo.DistanceFromStart = dist2;
                    adjNode.Link.Color = NodeColor.RED;
                    if (adjNode.Node.SourceLink != null)
                        adjNode.Node.SourceLink.Color = NodeColor.BLACK;
                    adjNode.Node.SourceLink = adjNode.Link;
                    queue.Enqueue(adjNode.Node, dist2);
                    CurrentEvent = $"{adjNode.Node.Text} (G={dist2}) hozzáadása a prioritási sorhoz: {queue.Text}";
                }
                else CurrentEvent = $"{adjNode.Node.Text} (G={dist2}) G értéke nem jobb, mint az aktuális ({adjNode.Node.DistanceInfo.DistanceFromStart}), nem adjuk hozzá a prioritási sorhoz. Prioritási sor: {queue.Text}";
                yield return StopLevel.Skippable;
            }
            yield return StopLevel.Always;
        }
    }

    public Dijkstra(Index page) : base(page)
    {
    }
}