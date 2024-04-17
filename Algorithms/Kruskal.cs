using GraphUtil.Components;
using Index = GraphUtil.Pages.Index;

namespace GraphUtil.Algorithms;

public class Kruskal : AlgorithmBase
{
    public override string Name => "Kruskal";
    public override string FullName => "Kruskal algoritmusa";

    public override string ShortDescription => "Minimális költségű feszítőfa megkeresése";

    public override string Description => """
                                          Az algoritmus megkeresi egy súlyozott gráf minimális költségű feszítőfáját (feszítőerdőt, ha a gráf nem összefüggő): Azt a feszítőfát, amelyeben az élek súlyának összege a lehető legkisebb.
                                          <br>
                                          Kezdetben tekintsünk minden csúcsra egy izolált pontként ("minden élt töröljünk ki"). Egy izolált pont önmagában egy fagráf. Menjünk végig minden élen súlyuk szerint növekvő sorrendben. Ha az él két különböző fához tartozó csúcsot köz össze, "húzzuk meg az élt", innentől ez a két pont egy fához tartozik.
                                          <br>
                                          Miután megnéztünk minden élt, a meghúzott élek adják a minimális költségű feszítőfát*.
                                          <hr>
                                          <h2>*Miért?</h2>
                                          Minden izolált pont egy fát képez. Két pontot csak akkor kötünk össze, ha különböző fához tartoznak, tehát nincs út közöttük. Egy éllel kötöttük össze őket, tehát a két csúcs (és a két fa összes többi csúcsa között) csak 1 út van (irányítottságtól eltekintve), tehát a kapott részgráf is fa. Mivel feszítőfát szeretnénk, N-1 élt húzunk meg (N csúcsú fagráf élszáma).
                                          <br>
                                          Mivel az éleket súlyuk szerint növekvő sorrendben ellenőrizzük, biztos hogy amikor két fát összekötünk, nincs másik olyan él, ami felhasználásával a létrejövő fa súlyösszege kisebb lenne.
                                          <hr>
                                          <h2>Implementáció</h2>
                                          Kruskal algoritmusának implementációja előtt szükségünk van egy algoritmusra amivel gyorsan tudjuk megmondani egy pontról, hogy melyik fához tartozik, illetve gyorsan tudunk két fát egyesíten: Unió-Holvan (Diszjunkt halmazfelbontás).
                                          <br>
                                          Minden fára tekintsünk gyökeres faként. Alapállapotban minden fa egy izolált pont, ilyenkor mindegyik pont (fa) gyökere önmaga. Amikor két fát egyesítünk, az egyik fa P gyökerét hozzákapcsoljuk a másik fa Q gyökeréhez: <code>Szülő(P) := Q</code>
                                          <br>
                                          A <code>Holvan(P)</code> függvény megmondja, hogy a P pont melyik fához tartozik (visszatérési érték a fa gyökere). Ezt úgy tudjuk megtalálni, hogy mindig egy szinttel "feljebb" (az aktuális pont szülője) megyünk amíg az aktuális pont szülője nem önmaga.
                                          <br>
                                          Mivel a fa struktúrájára nincs szükségünk, csak a benne lévő pontokra, ellaposíthatjuk a fát: minden bejárt pontot hozzácsatolunk közvetlenül a fa gyökeréhez. Ezzel gyorsíthatjuk <code>Holvan</code> futási idejét a későbbi hívásoknál.
                                          <code class="psc">
                                          Holvan(P):<br>
                                            &emsp;Ha Szülő(P) = P:<br>
                                                &emsp;&emsp;Eredmény P<br>
                                            &emsp;Ha vége<br>
                                            &emsp;Szülő(P) := Holvan(Szülő(P))<br>
                                            &emsp;Eredmény Szülő(P)<br>
                                          Vége
                                          </code>
                                          <code>Unió(P, Q)</code> egyesíti P és Q fáját. Az egyik fának a gyökerét hozzákapcsolja a másik fa gyökeréhez. Gyorsíthatók <code>Unió</code> későbbi hívásai, ha heurisztikusan eldöntjük, hogy melyik fát kapcsoljuk a másikhoz - azzal járunk a legjobban, ha a "nagyobb" fához kapcsoljuk a "kisebbet". Erre két gyakori módszer van:
                                          <h3>Egyesítés méret alapján</h3>
                                          Ilyenkor (különböző csúcsszámok esetén) a kisebb csúcsszámú fát kapcsoljuk a nagyobb csúcsszámúhoz (<code>Méret(P)</code> adja meg a P gyökerű fa csúcsszámát, kezdetben minden izolált pontra 1):
                                          <code class="psc">
                                          Unió(P, Q):<br>
                                            &emsp;PSzülő := Holvan(P)<br>
                                            &emsp;QSzülő := Holvan(Q)<br>
                                            &emsp;Ha nem PSzülő = QSzülő:<br>
                                                &emsp;&emsp;Ha Méret(PSzülő) > Méret(QSzülő):<br>
                                                    &emsp;&emsp;&emsp;Méret(PSzülő) := Méret(PSzülő) + Méret(QSzülő)<br>
                                                    &emsp;&emsp;&emsp;Szülő(QSzülő) := PSzülő<br>
                                                &emsp;&emsp;Különben<br>
                                                    &emsp;&emsp;&emsp;Méret(QSzülő) := Méret(QSzülő) + Méret(PSzülő)<br>
                                                    &emsp;&emsp;&emsp;Szülő(PSzülő) := QSzülő<br>
                                                &emsp;&emsp;Ha vége<br>
                                            &emsp;Ha vége<br>
                                          Vége
                                          </code>
                                          <h3>Egyesítés mélység alapján</h3>
                                          Ilyenkor (különböző mélységek esetén) a kisebb mélységű fát kapcsoljuk a nagyobb mélységűhöz (<code>Mélsyég(P)</code> adja meg a P gyökerű fa mélységét, kezdetben minden izolált pontra 0).
                                          <br>
                                          Csak akkor kell a mélységet megnövelni (1-gyel), ha két egyenlő mélységű fát kapcsoltunk egymáshoz, mert ellenkező esetben a maximális kisebb fát kapcsoltunk a a gyökérhez, mint fájának mélysége, tehát a maximális mélység változatlan marad.
                                          <br>
                                          Fontos megjegyezni, hogy itt a mélység csak egy felső határt jelent, ugyanis a <code>Holvan</code> függvényben megírt ellaposítás miatt a tényleges mélység változhat.
                                          <code class="psc">
                                          Unió(P, Q):<br>
                                            &emsp;PSzülő := Holvan(P)<br>
                                            &emsp;QSzülő := Holvan(Q)<br>
                                            &emsp;Ha nem PSzülő = QSzülő:<br>
                                                &emsp;&emsp;Ha Mélység(PSzülő) > Mélység(QSzülő):<br>
                                                    &emsp;&emsp;&emsp;Szülő(QSzülő) := PSzülő<br>
                                                &emsp;&emsp;Különben ha Mélység(PSzülő) < Mélység(QSzülő):<br>
                                                    &emsp;&emsp;&emsp;Szülő(PSzülő) := QSzülő<br>
                                                &emsp;&emsp;Különben:<br>
                                                    &emsp;&emsp;&emsp;Szülő(PSzülő) := QSzülő<br>
                                                    &emsp;&emsp;&emsp;Mélység(QSzülő) := Mélység(QSzülő) + 1<br>
                                                &emsp;&emsp;Ha vége<br>
                                            &emsp;Ha vége<br>
                                          Vége
                                          </code>
                                          Mindkét megoldásnak megegyezik az optimalizált futási ideje, a felhasználi szükségleteinek megfelelően választhatja ki, melyiket alkalmazza.
                                          <h3>Kruskal algoritmusának implentációja</h3>
                                          A <code>Honnan</code> függvény használatával meg tudjuk állapítani, hogy két csúcs különböző fákhoz tartoznak-e, ha igen, akkor egyesítjük <cude>Unió</code>-val, az aktuális él <code>Felhasznált</code> értékét pedig igazra állítjuk (piros élek).
                                          <br>
                                          <code>SorbarendezettÉlek()</code> a súlyuk alapján növekvő sorrendbe rendezett éleket adja vissza.
                                          <br>
                                          Futási idő: <strong>O(E * log N)</strong>, ahol E az élek, N pedig a csúcsok száma.
                                          <code class="psc">
                                          Kruskal():<br>
                                            &emsp;Ciklus minden P-re Pontok()-ban:<br>
                                                &emsp;&emsp;Szülő(P) := P<br>
                                                &emsp;&emsp;Méret(P) := 1<br>
                                                &emsp;&emsp;Mélység(P) := 0<br>
                                            &emsp;Ciklus vége<br>
                                            &emsp;Ciklus minden (P, Q)-ra SorbarendezettÉlek()-ben:<br>
                                                &emsp;&emsp;PSzülő := Holvan(P)<br>
                                                &emsp;&emsp;QSzülő := Holvan(Q)<br>
                                                &emsp;&emsp;Ha nem PSzülő = QSzülő:<br>
                                                    &emsp;&emsp;&emsp;Felhasznált(P, Q) := IGAZ<br>
                                                    &emsp;&emsp;&emsp;Unió(PSzülő, QSzülő)<br>
                                                &emsp;&emsp;Ha vége<br>
                                            &emsp;Ciklus vége<br>
                                          Vége
                                          </code>
                                          <hr>
                                          <strong>Felhasznált irodalom:</strong>
                                          <br>
                                          <a target="_blank" href="http://tehetseg.inf.elte.hu/szakkor/2023/grafok-3.pptx">ELTE: Gráfok, Gráfalgoritmusok III.</a>
                                          <br>
                                          <a target="_blank" href="https://en.wikipedia.org/wiki/Kruskal%27s_algorithm">Wikipedia: Kruskal's algorithm</a>
                                          <br>
                                          <a target="_blank" href="https://en.wikipedia.org/wiki/Disjoint-set_data_structure">Wikipedia: Disjoint-set data structure</a>
                                          <br>
                                          <a target="_blank" href="https://cp-algorithms.com/data_structures/disjoint_set_union.html">CP-Algorithms: Disjoint Set Union</a>
                                          """;

    private GraphNode Find(GraphNode node)
    {
        if (node.SourceNode == node)
            return node;
        node.SourceNode = Find(node.SourceNode);
        return node.SourceNode;
    }

    private void Union(GraphNode aRoot, GraphNode bRoot)
    {
        if (aRoot.Rank < bRoot.Rank)
            aRoot.SourceNode = bRoot;
        else if(aRoot.Rank > bRoot.Rank)
            bRoot.SourceNode = aRoot;
        else
        {
            bRoot.SourceNode = aRoot;
            aRoot.Rank++;
        }
    }

    public override IEnumerator<StopLevel> Execute()
    {
        foreach (var node in Page.Nodes.Values)
        {
            node.SourceNode = node;
            node.Rank = 0;
        }

        var i = 0;
        foreach(var edge in Page.Links.OrderBy(e => e.Weight))
        {
            edge.Color = NodeColor.GREEN;
            var a = (GraphNode)edge.SourceNode;
            var b = (GraphNode)edge.TargetNode;
            CurrentEvent = $"Ellenőrizzük {a.Text} és {b.Text} közti élt (Súly: {edge.Weight}).";
            yield return StopLevel.Skippable;
            var aRoot = Find(a);
            var bRoot = Find(b);
            if (aRoot != bRoot)
            {
                edge.Color = NodeColor.RED;
                Union(aRoot, bRoot);
                CurrentEvent =
                    $"{a.Text} (gyökér: {aRoot.Text}) és {b.Text} (gyökér: {bRoot.Text}) külön fákhoz tartoznak, az élt megtartjuk.";
            }
            else
            {
                edge.Color = NodeColor.BLACK;
                CurrentEvent =
                    $"{a.Text} és {b.Text} azonos fához tartoznak (gyökér: {aRoot.Text}), az élt nem tartjuk meg.";
            }
            yield return StopLevel.Always;
        }
    }

    public Kruskal(Index page) : base(page)
    {
    }
}