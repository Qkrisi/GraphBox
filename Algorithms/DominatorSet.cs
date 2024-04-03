using System.Collections;
using GraphUtil.Components;
using Index = GraphUtil.Pages.Index;

namespace GraphUtil.Algorithms;

public class DominatorSet : AlgorithmBase
{
    public override string Name => "Dominátor halmaz";

    public override string ShortDescription => "Legkisebb ponthalmaz, amely elemei valameikéből elindulva a gráf minden pontja bejárható";

    public override string Description => """
                                          Alap stratégia: mélységi bejárás
                                          <br>
                                          Ez az algoritmus megkeresi a legkisebb olyan D ponthalmazt, amire igaz, hogy a gráf bármelyik pontjába el lehet jutni D valamelyik pontjából elindulva.
                                          <br>
                                          A halmaz megkereséséhez írjuk fel a pontokat mélységi kilépési sorrendben: Válasszunk ki egy még nem bejárt pontot. Indítsunk el ebből a pontból kiindulva egy mélységi bejárást. Amikor egy pontból kilépünk (amikor minden bejártunk minden tőle induló ágat - "feketére színezzük"), adjuk hozzá a kilépési sorhoz. Ezt a folyamatot ismételjük addig, amíg van még nem bejárt csúcs.
                                          <br>
                                          Ezután tekintsünk minden csúcsot ismét bejáratlannak, majd induljunk el a kilépési sorban visszafelé. Minden P csúcsra csináljuk a következőt:
                                          <ul>
                                            <li>Ha P-t még nem jártuk be, adjuk hozzá a dominátor halmazhoz, majd indítsunk el tőle egy mélységi bejárást (itt lehetne akár szélességi is, csak azt szeretnénk tudni, hogy mely csúcsokba lehet innen eljutni). Tekintsünk bejártnak minden csúcsot, amihez a bejárással eljutunk.</li>
                                            <li>Ha P-t már bejártuk, átugorjuk.</li>
                                          </ul>
                                          Miután végigértünk a kilépési soron, megkaptuk a dominátor halmazt* (zöld csúcsok).
                                          <hr>
                                          <h2>*Miért?</h2>
                                          A kilépési sorba A pont később kerül be, mint B pont, ha A-ból van út B-be, de B-ből nincs út A-ba, mert:
                                          <ul>
                                            <li>Ha A-t előbb járjuk be, akkor mivel van út A-ból B-be, ugyanebben a mélységi bejárásban eljutunk B-be, ahonnan előbb fogunk kilépni, tehát B előbb kerül be a sorba.</li>
                                            <li>Ha B-t előbb járjuk be, akkor mivel nincs út B-ből A-ba, A-t csak egy következő mélységi bejárásban fogjuk bejárni, tehát A később fog bekerülni a sorba.</li>
                                          </ul>
                                          Ha két csúcs között mindkét irányban van út, akkor sorrendjuk mindegy (bemenet sorrendjétől, implementációtól függ), hiszen bármelyiket kiválasztva el tudunk jutni a másikba.
                                          <br>
                                          Ebből következik, hogy ha visszafelé indulunk el a sorban, akkor ha eljutunk (nem bejárás alatt) egy még nem bejárt P ponthoz (legutolsó pont először), akkor nincs olyan másik Q pont, amiből el lehetne jutni P-be, de P-ből Q-ba nem, hiszen ha lenne, akkor a sorban visszafelé haladva Q-hoz jutottunk volkna el előbb, és az onnan indított bejárással eljutottunk volna P-be.
                                          <br>
                                          Ezért adjuk hozzá P-t a dominátor halmazhoz, mert tudjuk, hogy minden másik Q pont, ahonnan el lehet jutni P-be, szintén elérhető P-ből, viszont minden más olyan R pont ahova P-ből el lehet jutni, de R-ből P-be nem, később van a sorban (visszafelé), tehát még nincs bejárva és a P-ből indított bejárással el fogunk hozzájuk jutni.
                                          <br>
                                          A kilépési sor alapján való bejárásokra tekinthetünk úgy, mintha kitörölné a bejárt pontokat a sorból, így mindig a sor "maradékának" utolsó pontját adjuk hozzá a dominátor halmazhoz (és tőle indítjuk az új bejárást).
                                          <hr>
                                          <h2>Implementáció</h2>
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
                                            <li>3 - A csúcsot bejártuk másodszor is a kilépési sor alapján (piros csúcsok)</li>
                                          </ul>
                                          <br>
                                          Futási idő: Csúcslista esetén <strong>O(N + E)</strong>, ahol N a csúcsok, E pedig az élek száma. Csúcsmátrix esetén <strong>O(N<sup>2</sup>)</strong>, ahol N a csúcsok száma.
                                          <br>
                                          <code class="psc">
                                          DominátorHalmaz():<br>
                                            &emsp;Ciklus minden P-re Csúcsok()-ban:<br>
                                                &emsp;&emsp;Ha Állapot(P) = 0:<br>
                                                    &emsp; &emsp;&emsp;Bejár1(P)<br>
                                                &emsp;&emsp;Ha vége<br>
                                            &emsp;Ciklus vége<br>
                                            &emsp;Amíg VeremElemszám() > 0:<br>
                                                &emsp;&emsp;P := Veremről()<br>
                                                &emsp;&emsp;Ha Állapot(P) = 2:<br>
                                                    &emsp;&emsp;&emsp;Dominátor(P) := IGAZ<br>
                                                    &emsp;&emsp;&emsp;Bejár2(P)<br>
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
                                          Bejár2(P):<br>
                                            &emsp;Állapot(P) := 3<br>
                                            &emsp;Ciklus minden Q-ra Szomszédok(P)-ben:<br>
                                                &emsp;&emsp;Ha Állapot(Q) = 2:<br>
                                                    &emsp;&emsp;&emsp;Bejár2(Q)<br>
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
        stack.Push(node);
        CurrentEvent = $"Kilépési sor létrehozása - kilépés {node.Text} pontból. Kilépési sor: {stack.GetText(true)}";
        yield return StopLevel.Always;
    }

    private IEnumerator Traverse2(GraphNode node, string color, Stack<GraphNode> stack)
    {
        node.Color = color;
        if (color == NodeColor.GREEN)
        {
            CurrentEvent = $"Kilépési sor utolsó eleme ({node.Text}) dominátor halmazbeli. Kilépési sor: {stack.GetText(true)}";
            yield return StopLevel.Always;
        }
        else
        {
            CurrentEvent =
                $"Eljutottunk {node.Text} ponthoz egy dominátor halmazbeli pontból. Kilépési sor: {stack.GetText(true)}";
            yield return StopLevel.Skippable;
        }
        foreach (var adjNode in node.AdjNodes)
        {
            if (adjNode.Node.Color == NodeColor.BLACK)
                yield return Traverse2(adjNode.Node, NodeColor.RED, stack);
        }
    }

    private IEnumerator ExecuteInner()
    {
        var stack = new Stack<GraphNode>();
        foreach (var node in Page.Nodes.Values)
        {
            if (node.Color == NodeColor.WHITE)
                yield return Traverse1(stack, node);
        }
        while(stack.Count > 0)
        {
            var p = stack.Pop();
            if (p.Color == NodeColor.BLACK)
                yield return Traverse2(p, NodeColor.GREEN, stack);
            else
            {
                CurrentEvent =
                    $"Kilépési utolsó elemét ({p.Text}) már bejártuk, átugorjuk. Kilépési sor: {stack.GetText(true)}";
                yield return StopLevel.Skippable;
            }
        }
    }

    public override IEnumerator<StopLevel> Execute() => AlgorithmWrapper(ExecuteInner());

    public DominatorSet(Index page) : base(page)
    {
    }
}