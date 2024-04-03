using GraphUtil.Algorithms.Arguments;
using GraphUtil.Components;
using Index = GraphUtil.Pages.Index;

namespace GraphUtil.Algorithms;

public class BellmanFord : AlgorithmBase
{
    public override string Name => "Bellman-Ford";
    public override string FullName => "Bellman-Ford algoritmus";

    public override string ShortDescription =>
        "Legrövidebb út megkeresése egy kezdőponttól a többi pontig (negatív élek megengedettek)";

    public override string Description => """
                                          Hasonlóan Dijkstra algoritmusához, a Bellman-Ford algoritmus súlyozott gráfban keresi meg a legrövidebb (legkisebb súlyösszegű) utat egy adott kezdőpont és a gráf többi pontja között. Itt a súlyok lehetnek negatívak, viszont az algoritmus futási ideje rosszabb Dijkstra algoritmusánál.
                                          <br>
                                          Probléma a negatív súlyokkal: negatív körök. Ha van a gráfban egy olyan kör, amelynek súlyösszege negatív, akkor (feltéve, hogy a kör elérhető a kezdőpontból) minden olyan csúcsnak, ami elérhető ebből a körből, kezdőponttól való legrövidebb távolsága negatív végtelen, hiszen ezen a körön bármennyiszer végig lehet menni, a távolság egyre kisebb lesz. Negatív súlyú irányítatlan él önmagában is képez negatív kört.
                                          <br>
                                          Hasonlóan Dijkstra algoritmusához, a Bellman-Ford algoritmus is az élek "relaxációjára" épül: ellenőrizzük, hogy egy adott él felhasználásával találtunk-e egy rövidebb utat az él végpontjába, mint az eddig ismert legrövidebb út abba a pontba. Azonban míg Dijkstra algoritmusa csak az aktuálisan legjobb még nem bejárt csúcsból induló éleket ellenőrzi, a Bellman Ford algoritmus minden lépésben megteszi ezt minden éllel. Irányítatlan éleknek mindkét irányát ellenőrizzük.
                                          <br>
                                          Egy P csúcs <code>G</code> értéke az aktuálisan ismert legrövidebb út hossza a kezdőpontból P-be. Kezdetben minden csúcsnak <code>G</code> értéke végtelen, kivéve a kezdőpontnak, aminek 0.
                                          <br>
                                          P-ből Q-ba mutató él S súllyal megad egy jobb utat, ha <strong>G(P) + S < G(Q)</strong>. Ilyenkor Q-nak <code>G</code> értékét átírjathzk <strong>G(P) + S</strong>-re, szülőpontját (ahonnan eljutottunk hozzá) pedig P-re.
                                          <br>
                                          Ezt a folyamatot megismételjük <strong>N</strong>-szer (N a csúcsok száma). Ha az utolsó lépésben még találtunk jobb utat bármelyik élnél, akkor ott van egy negatív kör.
                                          <br>
                                          Miután ezt elvégeztük, ismerjük minden csúcshoz a legrövidebb utat a kezdőpontból* (zöld csúcsok, a fehéren maradt csúcsokhoz nem lehet eljutni a kezdőpontból).
                                          <hr>
                                          <h2>*Miért</h2>
                                          Ha a legrövidebb út a kezdőpont és egy másik P pont között nem tartalmaz negatív kört, akkor nem tartalmaz egyáltalán kört, hiszen ha végigmennénk egy nemnegatív körön, akkor ugyanabba a csúcsba jutunk vissza úgy, hogy az út nem lett rövidebb. Ezért ennek az útnak maximális élszáma <strong>N-1</strong> (N csúcsú fagráf éleinek száma).
                                          <br>
                                          Tehát ha a relaxációt <strong>N-1</strong>-szer hajtuk végre minden élen, biztos, hogy bejártuk a kezdőponttól minden másik csúcsba vezető legrövidebb utat. Ha még egyszer végrehajtuk a relaxációt és ekkor is találtunk egy jobb utat valamely ponthoz, akkor biztos, hogy azon az úton van egy kör, aminek a súlyösszege a fenti állítás miatt biztos, hogy negatív, tehát minden belőle elérhető csúcs <code>G</code> értéke -∞.
                                          <hr>
                                          <h2>Implementáció</h2>
                                          <code>VanTávolság</code> adja meg, hogy az adott pontba már eljutottunk-e (kezdetben hamis - kezdeti végtelen távolság)
                                          <br>
                                          <code>Távolság</code> adja meg az adott pont <code>G</code> értékét, <code>Honnan</code> adja meg, hogy az adott pontba melyik másik pontból jutottunk el a legrövidebb úton.
                                          <br>
                                          Ha találtunk egy olyan P pontot ami negatív körből elérhető (N. lépésben jobb utat találtunk hozzá), megkeresünk egy pontot a körön: elindulunk P-től visszafelé (<strong>P<sub>következő</sub> = Honnan(P)</strong>), és amikor P<sub>következő</sub> egy olyan pont lenne, ahol már jártunk, P a negatív kör egy pontja.
                                          <br>
                                          <code>KörMegad(P)</code> keresi meg annak negatív kör pontjait (piros csúcsok), aminek P része (kihagyható, ha nincs a pontos kör ismeretére szükség).
                                          <br>
                                          <code>NegatívBejár(P)</code> egy mélységi bejárás, minden P-ből elérhető Q pontra beállítja <code>NegatívVégtelen(Q)</code> értékét igazra (sárga csúcsok, kivéve körön).
                                          <br>
                                          Futási idő: <strong>O(N * E)</strong>, ahol N a csúcsok, E pedig az élek száma.
                                          <br>
                                          <code class="psc">
                                            BellmanFord(kezdőpont):<br>
                                                &emsp;VanTávolság = [HAMIS minden P-re]<br>
                                                &emsp;VanTávolság(kezdőpont) := IGAZ<br>
                                                &emsp;Távolság(kezdőpont) := 0<br>
                                                &emsp;Ismétlés N-1-szer:<br>
                                                    &emsp;&emsp;Ciklus minden (P, Q, Súly)-ra Élek()-ben:<br>
                                                        &emsp;&emsp;&emsp;Ha VanTávolság(P):<br>
                                                            &emsp;&emsp;&emsp;&emsp;G := Távolság(P) + Súly<br>
                                                            &emsp;&emsp;&emsp;&emsp;Ha nem VanTávolság(Q) vagy G < Távolság(Q):<br>
                                                                &emsp;&emsp;&emsp;&emsp;&emsp;VanTávolság(Q) := IGAZ<br>
                                                                &emsp;&emsp;&emsp;&emsp;&emsp;Távolság(Q) := G<br>
                                                                &emsp;&emsp;&emsp;&emsp;&emsp;Honnan(Q) := P<br>
                                                            &emsp;&emsp;&emsp;&emsp;Ha vége<br>
                                                        &emsp;&emsp;&emsp;Ha vége<br>
                                                   &emsp;&emsp; Ciklus vége<br>
                                                &emsp;Ismétlés vége<br>
                                                &emsp;Ciklus minden (P, Q, Súly)-ra Élek()-ben:<br>
                                                        &emsp;&emsp;Ha VanTávolság(P):<br>
                                                            &emsp;&emsp;&emsp;Ha Távolság(P) + Súly < Távolság(Q):<br>
                                                                &emsp;&emsp;&emsp;&emsp;Honnan(Q) := P<br>
                                                                &emsp;&emsp;&emsp;&emsp;Bejárt := [HAMIS minden P-re]<br>
                                                                &emsp;&emsp;&emsp;&emsp;Bejárt(Q) := IGAZ<br>
                                                                &emsp;&emsp;&emsp;&emsp;Amíg nem Bejárt(P):<br>
                                                                    &emsp;&emsp;&emsp;&emsp;&emsp;Bejárt(P) := IGAZ<br>
                                                                    &emsp;&emsp;&emsp;&emsp;&emsp;P := Honnan(P)<br>
                                                                &emsp;&emsp;&emsp;&emsp;Amíg vége<br>
                                                                &emsp;&emsp;&emsp;&emsp;Körökhöz(KörMegad(P))<br>
                                                                &emsp;&emsp;&emsp;&emsp;NegatívBejár(P)<br>
                                                            &emsp;&emsp;&emsp;Ha vége<br>
                                                        &emsp;&emsp;Ha vége<br>
                                                &emsp;Ciklus vége<br>
                                            Vége<br>
                                            <br>
                                            KörMegad(P):<br>
                                              &emsp;Kör := [P]<br>
                                              &emsp;Q := Honnan(P)<br>
                                              &emsp;Amíg nem Q = P:<br>
                                                  &emsp;&emsp;Kör := Kör + [Q]<br>
                                                  &emsp;&emsp;Q := Honnan(Q)<br>
                                              &emsp;Amíg vége<br>
                                              &emsp;Eredmény Kör<br>
                                            Vége<br>
                                            <br>
                                            NegatívBejár(P):<br>
                                                &emsp;NegatívVégtelen(P) := IGAZ<br>
                                                &emsp;Ciklus minden Q-ra Szomszédok(P)-ben:<br>
                                                    &emsp;&emsp;Ha nem NegatívVégtelen(Q):<br>
                                                        &emsp;&emsp;&emsp;NegatívBejár(Q)<br>
                                                    &emsp;&emsp;Ha vége<br>
                                                &emsp;Ciklus vége<br>
                                            Vége
                                          </code>
                                          """;

    public override AlgorithmArgumentBase[] Arguments => new AlgorithmArgumentBase[] { StartNode };

    private AlgorithmArgument<GraphNode> StartNode = new("Kezdőpont");

    public override bool ValidateArguments() => StartNode.Value != null;

    public override IEnumerator<StopLevel> Execute()
    {
        StartNode.Value.DistanceInfo.DistanceFromStart = 0;
        StartNode.Value.Color = NodeColor.GREEN;
        for (int i = 0; i < Page.Nodes.Count - 1; i++)
        {
            foreach (var edge in Page.Links)
            {
                var u = (GraphNode)edge.SourceNode;
                var v = (GraphNode)edge.TargetNode;
                var checkOtherDirection = true;
                CheckEdge:
                var c = edge.Color;
                edge.SetColor(NodeColor.GREEN);
                CurrentEvent = $"Relaxáció #{i + 1}: {u.Text} és {v.Text} közti él ellenőrzése";
                yield return StopLevel.Skippable;
                bool success = false;
                if (u.DistanceInfo.DistanceFromStart != null)
                {
                    var d = u.DistanceInfo.DistanceFromStart + edge.Weight;
                    if (v.DistanceInfo.DistanceFromStart == null || d < v.DistanceInfo.DistanceFromStart)
                    {
                        v.SourceLink?.SetColor(NodeColor.BLACK);
                        v.DistanceInfo.DistanceFromStart = d;
                        v.Color = NodeColor.GREEN;
                        v.SourceNode = u;
                        v.SourceLink = edge;
                        c = NodeColor.RED;
                        edge.SetColor(NodeColor.RED);
                        success = true;
                        CurrentEvent =
                            $"Relaxáció #{i + 1}: Találtunk egy jobb utat {v.Text}-be (G={d}), használjuk fel az élt.";
                    }
                }

                if (!success)
                    CurrentEvent = $"Relaxáció #{i + 1}: Az él nem ad meg egy jobb utat {v.Text}-be, átugorjuk.";
                yield return StopLevel.Skippable;
                edge.SetColor(c);
                if (checkOtherDirection && edge.Bidirectional)
                {
                    (u, v) = (v, u);
                    checkOtherDirection = false;
                    goto CheckEdge;
                }
                
            }

            CurrentEvent = $"Relaxáció #{i + 1} vége";
            yield return StopLevel.Always;
        }

        foreach (var edge in Page.Links)
        {
            var u = (GraphNode)edge.SourceNode;
            var v = (GraphNode)edge.TargetNode;
            var checkOtherDirection = true;
            CheckEdge:
            if (u.DistanceInfo.DistanceFromStart != null && v.DistanceInfo.DistanceFromStart == null || u.DistanceInfo.DistanceFromStart + edge.Weight < v.DistanceInfo.DistanceFromStart)
            {
                if (u.DistanceInfo.NegInf)
                    continue;
                v.SourceNode = u;
                v.SourceLink = edge;
                var visited = Page.Nodes.ToDictionary(p => p.Key, _ => false);
                visited[v.Text] = true;
                while (!visited[u.Text])
                {
                    visited[u.Text] = true;
                    u = u.SourceNode;
                }

                var negativeCycle = new List<GraphNode>
                {
                    u
                };
                v = u.SourceNode;
                while (v != u)
                {
                    v.DistanceInfo.NegInf = true;
                    v.Color = NodeColor.RED;
                    negativeCycle.Add(v);
                    CurrentEvent = $"{v.Text} negatív kör csúcsa";
                    v = v.SourceNode;
                    yield return StopLevel.Skippable;
                }
                v.DistanceInfo.NegInf = true;
                v.Color = NodeColor.RED;
                CurrentEvent = $"{v.Text} negatív kör csúcsa - megtaláltuk a negatív kört";
                yield return StopLevel.Always;
                negativeCycle.Reverse();
                foreach (var cycleNode in negativeCycle)
                {
                    foreach (var node in BreadthFirstSearch.BFSNodes(cycleNode))
                    {
                        if (node.Color != NodeColor.RED)
                        {
                            node.DistanceInfo.NegInf = true;
                            node.Color = NodeColor.YELLOW;
                        }
                    }
                }
                CurrentEvent = $"A negatív körből elérhető pontok G értéke -\u221e";
                yield return StopLevel.Always;
            }

            if (checkOtherDirection && edge.Bidirectional)
            {
                (u, v) = (v, u);
                checkOtherDirection = false;
                goto CheckEdge;
            }
        }
    }

    public BellmanFord(Index page) : base(page)
    {
    }
}