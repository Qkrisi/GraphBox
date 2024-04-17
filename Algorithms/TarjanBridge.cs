using GraphUtil.Components;
using System.Collections;

namespace GraphUtil.Algorithms;
using Index = GraphUtil.Pages.Index;

public class TarjanBridge : AlgorithmBase
{
    public override string Name => "Elvágó élek, pontok (Tarjan)";
    public override string FullName => "Elvágó élek, pontok (Tarjan algoritmusa alapján)";

    public override string ShortDescription => "Elvágó élek és pontok megkeresése";

    public override string Description => """
                                          Alap stratégia: mélységi bejárás
                                          <br>
                                          Ez az algoritmus megkeresi az elvágó éleket és pontokat az adott irányítatlan gárfban:
                                          <ul>
                                            <li>Elvágó él: olyan él, amelyet elhagyva a gráf már nem összefüggő.</li>
                                            <li>Elvágó pont: olyan pont, amelyre igaz, hogy a tőle induló éleket, illetve önmagát elhagyva a gráf már nem összefüggő.</li>
                                          </ul>
                                          Válasszunk ki egy bejáratlan pontot (kezdetben a gráf bármelyik pontja). Ahhoz, hogy egy élről el tudjuk dönteni, hogy elvágó él-e, tároljuk el minden pont belépési idejét (a mélységi bejárás hányadik lépésében jutottunk el hozzá), ez lesz a csúcs <code>T</code> értéke.
                                          <br>
                                          Az egy P csúcsból kiinduló éleket (kivéve ahonnan P-hez eljutottunk) 2 kategóriába soroljuk:
                                          <ul>
                                            <li>Előremutató él (faél): olyan él, aminek másik végpontját még nem jártuk be (P gyereke lesz) - sárga él ellenőrzéskor, fekete utána</li>
                                            <li>Visszamutató él: olyan él, aminek másik végpontját már bejártuk (kivéve ha ezen az élen jutottunk el P-hez) - piros élek</li>
                                          </ul>
                                          Egy csúcsnak az <code>L</code> értéke a tőle induló részfából "legmagasabbra" visszamutató él végpontjának <code>T</code> értéke, a következő három érték minimuma:
                                          <ul>
                                            <li>A csúcs <code>T</code> értéke</li>
                                            <li>A visszamutató élek végpontjainak <code>T</code> értékeinek minimuma</li>
                                            <li>Az előremutató élek végpontjainak <code>L</code> értékeinek minimuma</li>
                                          </ul>
                                          Egy P-ből induló Q-ba vezető előremutató él elvágó, ha <strong>T(P) < L(Q)</strong>.*
                                          <br>
                                          Egy P csúcs elvágó, ha bármelyik tőle induló Q-ba előremutató élre igaz, hogy <strong>T(P) ≤ L(Q)</strong>.* Ezalól kivételt képez a mélységi feszítőfa gyökere (ahonnan indítottuk a bejárást). Ez a csúcs akkor elvágó, ha a mélységi feszítőfában több, mint 1 közvetlen gyereke van.*
                                          <br>
                                          Ezt a folyamatot ismételjük addig, amíg van még nem bejárt pont (nem összefüggő gráf minden komponensében keressük meg az elvágó éleket és pontokat).
                                          <hr>
                                          <h2>*Miért</h2>
                                          Belátható, hogy él elvágó, ha a végpontjából induló fa (a mélységi bejárás által létrehozott feszítőfa) egyik pontjának sincs az él P kezdőpontjára, vagy nála "magasabbra" (valamelyik (nem csak közvetlen) szülője) visszamutató éle. Ilyenkor ugyanis P és a fa bármelyik éle közötti összes út tartalmazza ezt az élt, elhagyva nem lesz út a csúcsok között.
                                          <br>
                                          Egy csúcsnak az <code>L</code> értéke a tőle induló részfából "legmagasabbra" visszamutató él végpontjának <code>T</code> értéke, mert összetétele ezen három érték minimuma:
                                          <ul>
                                            <li>A csúcs <code>T</code> értéke - akkor a minimum, ha a csúcs fokszáma (hozzá kapcsolódó élek száma) 1. Ez az él nem lehet visszamutató, mert innen érkeztünk el a ponthoz (ha a csúcs a mélységi bejárás kezdőpontja, ez az él előremutató).</li>
                                            <li>A visszamutató élek végpontjainak <code>T</code> értékeinek minimuma - A csúcsból közvetlenül induló visszamutató éleinek végpontjai közül a "legmagasabban" lévő.</li>
                                            <li>Az előremutató élek végpontjainak <code>L</code> értékeinek minimuma - A csúcs (nem csak közvetlen) gyerekeiből induló visszamutató élek végpontjai közül "legmagasabban" lévő.</li>
                                          </ul>
                                          Mélységi bejárás menetéből adódik, hogy ha egy P pontból induló részfának egy másik Q pont része, akkor <strong>T(P) < T(Q)</strong>. Azért keressük a visszamutató élek minimum értékét, mert ha mutat vissza egy él olyan pontba, ami P részfájának (P kihagyásával) egy csúcsa, akkor annak a csúcsnak <code>T</code> értéke biztos, hogy nem nagyobb, mint a P csúcs <code>T</code> értéke.
                                          <br>
                                          Ebből következik, hogy ha egy előremutató él Q végpontjának <code>L</code> értéke nagyobb, mint P kezdőpontjának <code>T</code> értéke, akkor a Q végpontból induló részfa egyik csúcsából induló visszamutató élek végpontjainak <code>T</code> értéke sem kisebb, vagy egyenlő, mint <code>T(P)</code>, tehát egyik ilyen visszamutató él sem vezet P-be, vagy nála "magasabbra", tehát az él elvágó.
                                          <br>
                                          Hasonlóképp, ha egy P pontból induló részfa minden csúcsának visszamutató éleinek végpontjának <code>T</code> értéke nem kisebb, mint <code>T(P)</code>, akkor P pont elvágó, mert ha a mindegyik ilyen visszamutató él végpontjának <code>T</code> értéke nagyobb, mint <code>T(P)</code>, akkor a P pont előremutató élei elvágóak, P pont törlésével pedig töröltük őket. Ha pedig van visszamutató él, aminek végpontjának <code>T</code> értéke megegyezik <code>T(P)</code>, akkor ez az él, illetve a P-ből induló ehhez a részfához vezető él nem elvágó, viszont P törlésével töröljük mindkettőt, tehát a részfa "elszakad" a gráftól.
                                          <br>
                                          Ezalól kivétel a mélységi bejárás kezdőpontja (nála kisebb <code>T</code> értékű pontra nem mutathat visszamutató él), ez akkor elvágó pont, ha a mélységi fában több, mint egy gyereke van, hiszen ilyenkor ez a két részfa biztos, hogy nem lessz összekapcsolva e pont törlése után, mert a mélységi bejárásnak vissza kellett lépnie a kezdőpontba ahhoz, hogy eljusson be tudja járni a másik részfát.
                                          <hr>
                                          <h2>Implementáció</h2>
                                          Az idő mérésére használjunk egy globális <code>Idő</code> változót, amit a mélységi bejárás minden lépésében megnövelünk 1-gyel.
                                          <br>
                                          Ha egy (P, Q) él elvágó, állítsuk be <code>ElvágóÉl(P, Q)</code> értékét igazra (zöld élek). Hasonlóan ha egy P pont elvágó, állítsuk be <code>ElvágóPont(P)</code> értékét igazra (zöld csúcsok).
                                          <br>
                                          Hasonlóan a mélységi bejáráshoz, <code>Állapot</code> adja meg az adott csúcs aktuális állapotát:
                                          <ul>
                                            <li>0 - Alapeset, a csúcshoz még nem jutottunk el (fehér csúcsok)</li>
                                            <li>1 - A csúcshoz már eljutottunk, de még nem jártuk be teljesen a tőle induló részfát, még nem léptünk ki belőle (szürke csúcsok)</li>
                                            <li>2 - A csúcsot és a tőle induló részfát bejártuk, kiléptünk belőle (fekete csúcsok - kivéve ha a pont elvágó, amikor is zöld)</li>
                                          </ul>
                                          Futási idő: <strong>O(N + E)</strong>, ahol N a csúcsok, E pedig az élek száma.
                                          <code class="psc">
                                          Elvágó():<br>
                                            &emsp;Idő := 0<br>
                                            &emsp;Ciklus minden P-re Pontok()-ban:<br>
                                                &emsp;&emsp;Ha Állapot(P) = 0:<br>
                                                    &emsp;&emsp;&emsp;Bejár(P, -1)<br>
                                                &emsp;&emsp;Ha vége<br>
                                            &emsp;Ciklus vége<br>
                                          Vége<br>
                                          <br>
                                          Bejár(P, Szülő):<br>
                                            &emsp;Állapot(P) := 1<br>
                                            &emsp;Idő := Idő + 1<br>
                                            &emsp;T(P) := Idő<br>
                                            &emsp;L(P) := Idő<br>
                                            &emsp;Gyerekek := 0<br>
                                            &emsp;Ciklus minden Q-ra Szomszédok(P)-ben:<br>
                                                &emsp;&emsp;Ha nem Q = Szülő:<br>
                                                    &emsp;&emsp;&emsp;Ha Állapot(Q) > 0:&emsp;//Visszamutató<br>
                                                        &emsp;&emsp;&emsp;&emsp;L(P) := Min(L(P), T(Q))<br>
                                                    &emsp;&emsp;&emsp;Különben:&emsp;//Előremutató<br>
                                                        &emsp;&emsp;&emsp;&emsp;Gyerekek := Gyerekek + 1<br>
                                                        &emsp;&emsp;&emsp;&emsp;Bejár(Q, P)<br>
                                                        &emsp;&emsp;&emsp;&emsp;L(P) = Min(L(P), L(Q))<br>
                                                        &emsp;&emsp;&emsp;&emsp;Ha T(P) < L(Q):<br>
                                                            &emsp;&emsp;&emsp;&emsp;&emsp;ElvágóÉl(P, Q) := IGAZ<br>
                                                        &emsp;&emsp;&emsp;&emsp;Ha vége<br>
                                                        &emsp;&emsp;&emsp;&emsp;Ha (nem Szülő = -1) vagy T(P) <= L(Q):<br>
                                                            &emsp;&emsp;&emsp;&emsp;&emsp;ElvágóPont(P) := IGAZ<br>
                                                        &emsp;&emsp;&emsp;&emsp;Ha vége<br>
                                                    &emsp;&emsp;&emsp;Ha vége<br>
                                                &emsp;&emsp;Ha vége<br>
                                            &emsp;Ciklus vége<br>
                                            &emsp;Ha Szülő = -1 és Gyerekek > 1:<br>
                                                &emsp;&emsp;ElvágóPont(P) := IGAZ<br>
                                            &emsp;Ha vége<br>
                                            &emsp;Állapot(P) := 2<br>
                                          Vége
                                          </code>
                                          <hr>
                                          <strong>Felhasznált irodalom:</strong>
                                          <br>
                                          <a target="_blank" href="http://tehetseg.inf.elte.hu/szakkor/2023/grafok-3.pptx">ELTE: Gráfok, Gráfalgoritmusok III.</a>
                                          <br>
                                          <a target="_blank" href="https://cp-algorithms.com/graph/bridge-searching.html">CP-Algorithms: Finding bridges in a graph in O(N + M)</a>
                                          <br>
                                          <a target="_blank" href="https://cp-algorithms.com/graph/cutpoints.html">CP-Algorithms: Finding articulation points in a graph in O(N + M)</a>
                                          """;

    private int Time;

    private IEnumerator<StopLevel> GetBridges(GraphNode node, GraphNode? parent)
    {
        node.Color = NodeColor.GRAY;
        node.Disc = node.Low = ++Time;
        CurrentEvent = $"{node.Text} csúcsba belépés (T=(L=){node.Disc}), éleinek ellenőrzése";
        yield return StopLevel.Skippable;
        var children = 0;
        foreach (var adj in node.AdjNodes)
        {
            var adjNode = adj.Node;
            if (adjNode == parent)
                continue;
            var origLow = node.Low;
            if (adjNode.Color != NodeColor.WHITE)
            {
                adj.Link.Color = NodeColor.RED;
                node.Low = Math.Min(node.Low, adjNode.Disc);
                CurrentEvent =
                    $"{node.Text} -> {adjNode.Text} él visszamutatú, L({node.Text}) átállítása L({node.Text})={origLow} és T({adjNode.Text})={adjNode.Disc} minimumára ({node.Low})";
                yield return StopLevel.Skippable;
            }
            else
            {
                children++;
                adj.Link.Color = NodeColor.YELLOW;
                CurrentEvent = $"{node.Text} -> {adjNode.Text} él előremutató, lépjünk be a végpontba ({adjNode.Text})";
                yield return StopLevel.Skippable;
                adj.Link.Color = NodeColor.BLACK;
                var iter = GetBridges(adjNode, node);
                while (iter.MoveNext())
                    yield return iter.Current;
                node.Low = Math.Min(node.Low, adjNode.Low);
                CurrentEvent =
                    $"Kiléptünk {adjNode.Text} pontból, visszajutottunk {node.Text} pontba. L értéke legyen L({node.Text})={origLow} és L({adjNode.Text})={adjNode.Low} minimuma ({node.Low})";
                yield return StopLevel.Skippable;
                if (adjNode.Low > node.Disc)
                {
                    adj.Link.Color = NodeColor.GREEN;
                    CurrentEvent =
                        $"{node.Text} T értéke ({node.Disc}) kisebb, mint {adjNode.Text} L értéke ({adjNode.Low}), tehát az él a két pont között elvágó";
                    yield return StopLevel.Always;
                }

                if (parent != null && adjNode.Low >= node.Disc)
                {
                    node.Color = NodeColor.GREEN;
                    CurrentEvent =
                        $"{node.Text} T értéke ({node.Disc}) nem nagyobb, mint {adjNode.Text} L értéke ({adjNode.Low}) (és {node.Text} nem a bejárási fa gyökere), tehát {node.Text} pont elvágó";
                    yield return StopLevel.Always;
                }
            }
        }
        
        node.ShowLow = true;

        if (parent == null && children > 1)
        {
            CurrentEvent =
                $"{node.Text} a bejárási fa gyökere, és több, mint 1 közvetlen gyereke van a fában, tehát elvágó";
            node.Color = NodeColor.GREEN;
            yield return StopLevel.Always;
        }

        if(node.Color != NodeColor.GREEN)
            node.Color = NodeColor.BLACK;
        CurrentEvent = $"Megkaptuk {node.Text} csúcs végleges L értékét ({node.Low}), kiléphetünk belőle";
        yield return StopLevel.Always;
    }
    
    private IEnumerator ExecuteAlg()
    {
        if (Page.Nodes.Count == 0)
            yield break;
        foreach (var node in Page.Nodes.Values)
        {
            if(node.Color != NodeColor.WHITE)
                continue;
            yield return GetBridges(node,  null);
        }
    }

    public override IEnumerator<StopLevel> Execute() => AlgorithmWrapper(ExecuteAlg());

    public override void Reset()
    {
        base.Reset();
        Time = 0;
    }

    public TarjanBridge(Index page) : base(page)
    {
    }
}