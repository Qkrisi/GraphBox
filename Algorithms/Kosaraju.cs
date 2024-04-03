using System.Collections;
using GraphUtil.Components;
using Index = GraphUtil.Pages.Index;

namespace GraphUtil.Algorithms;

public class Kosaraju : AlgorithmBase
{
    public override string Name => "Kosaraju";
    public override string FullName => "Kosaraju algoritmusa";

    public override string ShortDescription => "Erősen összefüggő komponensek megkeresése";

    public override string Description => """
                                          Ez az algoritmus felbontja a gráoft erősen összefüggő komponensekre: a legnagyobb olyan részgráfok, amelyekre igaz, hogy bennük bármely két pont között van út mindkét irányban.
                                          <br>
                                          Egy csúcs <code>C</code> értéke adja meg, hogy melyik komponenshez tartozik.
                                          <br>
                                          Az algoritmus hasonlít a dominátor halmaz keresési algoritmusra. írjuk fel a pontokat mélységi kilépési sorrendben: Válasszunk ki egy még nem bejárt pontot. Indítsunk el ebből a pontból kiindulva egy mélységi bejárást. Amikor egy pontból kilépünk (amikor minden bejártunk minden tőle induló ágat - "feketére színezzük"), adjuk hozzá a kilépési sorhoz. Ezt a folyamatot ismételjük addig, amíg van még nem bejárt csúcs.
                                          <br>
                                          Ezután tekintsünk minden csúcsot ismét bejáratlannak, majd induljunk el a kilépési sorban visszafelé. Minden P csúcsra csináljuk a következőt:
                                          <ul>
                                            <li>Ha P-t még nem jártuk be, adjunk hozzá a jelenlegi komponensazonosítóhoz 1-et (új komponenst nyitunk), majd indítsunk el tőle egy mélységi bejárást <strong>a gráf transzponáltjában:</strong> az aktuális pontból minden olyan Q pontba megyünk tovább, ami<strong>ből</strong> vezet közvetlen él P-be. Minden csúcs, amihez ezzel a bejárással eljutunk ugyanabba a komponensbe tartozik* (jelenlegi komponensazonosító).</li>
                                            <li>Ha P-t már bejártuk, átugorjuk.</li>
                                          </ul>
                                          <hr>
                                          <h2>*Miért</h2>
                                          A kilépési sorba A pont később kerül be, mint B pont, ha A-ból van út B-be, de B-ből nincs út A-ba, mert:
                                          <ul>
                                            <li>Ha A-t előbb járjuk be, akkor mivel van út A-ból B-be, ugyanebben a mélységi bejárásban eljutunk B-be, ahonnan előbb fogunk kilépni, tehát B előbb kerül be a sorba.</li>
                                            <li>Ha B-t előbb járjuk be, akkor mivel nincs út B-ből A-ba, A-t csak egy következő mélységi bejárásban fogjuk bejárni, tehát A később fog bekerülni a sorba.</li>
                                          </ul>
                                          Ha két csúcs között mindkét irányban van út, akkor sorrendjuk mindegy (bemenet sorrendjétől, implementációtól függ), hiszen bármelyiket kiválasztva el tudunk jutni a másikba.
                                          <br>
                                          Mivel mindig az utolsó (visszafelé első) még nem bejárt csúcsnál indítjuk a bejárást, tudjuk, hogy minden más még nem bejárt csúcs előbb került be a sorba. Mivel a gráf transzponáltját járjuk be, gyakorlatilag visszafelé megyünk az éleken. Tehát P pontból kezdve, minden Q pontról amit így bejárunk, elmondható, hogy vezet belőle út P-be. De mivel Q a sorba előbb került be, kell, hogy vezessen út P-ből Q-ba is, különben Q-nak később kellett volna a sorba kerülnie (feljebb beláttuk). Tehát mindkét irányban van köztük út és ebből kifolyólag bármelyik két így bejárt pont között is, tehát azonos erősen összefüggő komponenshez tartoznak.
                                          <hr>
                                          <h2>Implementáció</h2>
                                          Az implementáció nagyrész megegyezik a dominátor halmaz keresési algoritmussal. Egyetlen különbség, hogy <code>Bejárás2</code>-ben a transzponált szomszédokba lépünk be, illetve a csúcsokhoz hozzárendeljük a megfelelő komponens azonosítóját.
                                          <br>
                                          A csúcsokat a kilépéskor helyezzük egy verembe (last in, first out), így a csúcs amit utoljára adtunk hozzá, fog először kikerülni (megfordított kilépési sorrend).
                                          <br>
                                          Veremhez műveletei: <code>Veremre</code>, <code>Veremről</code>, <code>VeremElemszám</code>
                                          <br>
                                          <code>Állapot</code> mondja meg egy csúcs aktuális állapotát:
                                          <br>
                                          <ul>
                                            <li>0 - Alapeset, a csúcshoz még nem jutottunk el (fehér csúcsok)</li>
                                            <li>1 - A csúcshoz már eljutottunk, de még nem léptünk ki belőle (szürke csúcsok)</li>
                                            <li>2 - A csúcsból már kiléptünk (fekete csúcsok)</li>
                                            <li>3 - A csúcsot bejártuk másodszor is a kilépési sor alapján, hozzáadtuk egy komponenshez (sárga csúcsok)</li>
                                          </ul>
                                          <br>
                                          Futási idő: Csúcslista esetén <strong>O(N + E)</strong>, ahol N a csúcsok, E pedig az élek száma. Csúcsmátrix esetén <strong>O(N<sup>2</sup>)</strong>, ahol N a csúcsok száma.
                                          <br>
                                          <code class="psc">
                                          Kosaraju():<br>
                                            &emsp;Ciklus minden P-re Csúcsok()-ban:<br>
                                                &emsp;&emsp;Ha Állapot(P) = 0:<br>
                                                    &emsp; &emsp;&emsp;Bejár1(P)<br>
                                                &emsp;&emsp;Ha vége<br>
                                            &emsp;Ciklus vége<br>
                                            &emsp;C := 0<br>
                                            &emsp;Amíg VeremElemszám() > 0:<br>
                                                &emsp;&emsp;P := Veremről()<br>
                                                &emsp;&emsp;Ha Állapot(P) = 2:<br>
                                                    &emsp;&emsp;&emsp;C := C + 1<br>
                                                    &emsp;&emsp;&emsp;Bejár2(P, C)<br>
                                                &emsp;&emsp;Ha vége<br>
                                            &emsp;Amíg vége<br>
                                          Vége<br>
                                          <br>
                                          Bejár1(P):<br>
                                            &emsp;Állapot(P) := 1<br>
                                            &emsp;Ciklus minden Q-ra Szomszédok(P)-ben:<br>
                                                &emsp;&emsp;Ha Állapot(Q) = 0:<br>
                                                    &emsp;&emsp;&emsp;Bejár1(Q)<br>
                                                &emsp;&emsp;Ha vége<br>
                                            &emsp;Ciklus vége<br>
                                            &emsp;Állapot(P) := 2<br>
                                            &emsp;Veremre(P)<br>
                                          Vége<br>
                                          <br>
                                          Bejár2(P, C):<br>
                                            &emsp;Állapot(P) := 3<br>
                                            &emsp;Komponens(P) := C<br>
                                            &emsp;Ciklus minden Q-ra TranszponáltSzomszédok(P)-ben:<br>
                                                &emsp;&emsp;Ha Állapot(Q) = 2:<br>
                                                    &emsp;&emsp;&emsp;Bejár2(Q, C)<br>
                                                &emsp;&emsp;Ha vége<br>
                                            &emsp;Ciklus vége<br>
                                          Vége
                                          </code>
                                          """;

    private IEnumerator Traverse1(Stack<GraphNode> stack, GraphNode node)
    {
        node.Color = NodeColor.GRAY;
        CurrentEvent = $"Kilépési sor létrehozása - belépés {node.Text} pontba. Kilépési sor: {stack.GetText(true)}";
        yield return StopLevel.Skippable;
        foreach (var adjNode in node.AdjNodes)
        {
            if (adjNode.Node.Color == NodeColor.WHITE)
                yield return Traverse1(stack, adjNode.Node);
        }
        node.Color = NodeColor.BLACK;
        CurrentEvent = $"Kilépési sor létrehozása - kilépés {node.Text} pontból. Kilépési sor: {stack.GetText(true)}";
        yield return StopLevel.Always;
        stack.Push(node);
    }
    
    private IEnumerator Traverse2(GraphNode node, string color, int component, bool first, Stack<GraphNode> stack)
    {
        node.Color = color;
        node.Component = component;
        if (first)
        {
            CurrentEvent = $"Kilépési sor utolsó eleme ({node.Text}) új komponenst (C={component}) kezd. Kilépési sor: {stack.GetText(true)}";
            yield return StopLevel.Always;
        }
        else
        {
            CurrentEvent =
                $"Eljutottunk {node.Text} ponthoz a transzponált gráfban a {component}. komponens kezdőelemétől => C={component}. Kilépési sor: {stack.GetText(true)}";
            yield return StopLevel.Skippable;
        }

        foreach (var _node in Page.Nodes.Values)
        {
            if (_node == node || _node.Color != NodeColor.BLACK || !_node.AdjNodes.Any(adj => adj.Node == node))
                continue;
            yield return Traverse2(_node, NodeColor.YELLOW, component, false, stack);
        }
    }
    
    private IEnumerator ExecuteInner()
    {
        var stack = new Stack<GraphNode>();
        int component = 0;
        foreach (var node in Page.Nodes.Values)
        {
            if (node.Color == NodeColor.WHITE)
                yield return Traverse1(stack, node);
        }
        while(stack.Count > 0)
        {
            var p = stack.Pop();
            if (p.Color == NodeColor.BLACK)
                yield return Traverse2(p, NodeColor.YELLOW, ++component, true, stack);
            else
            {
                CurrentEvent =
                    $"Kilépési utolsó elemét ({p.Text}) már bejártuk, átugorjuk. Kilépési sor: {stack.GetText(true)}";
                yield return StopLevel.Skippable;
            }
        }
    }

    public override IEnumerator<StopLevel> Execute() => AlgorithmWrapper(ExecuteInner());

    public Kosaraju(Index page) : base(page)
    {
    }
}