namespace GraphUtil;

public class PrintablePriorityQueue<TElement, TPriority> : PriorityQueue<TElement, TPriority>
{
    public string Text
    {
        get
        {
            var result = "";
            var first = true;
            foreach (var item in UnorderedItems)
            {
                if (!first)
                    result += "; ";
                first = false;
                result += $"({item.Element?.ToString()}, {item.Priority?.ToString()})";
            }

            if (TryPeek(out var element, out var priority))
            {
                var str = $"({element?.ToString()}, {priority?.ToString()})";
                result = result.ReplaceFirst(str, $"<strong>{str}</strong>");
            }
            return result;
        }
    }

    public PrintablePriorityQueue(IComparer<TPriority>? comparer) : base(comparer)
    {
    }
    
    public PrintablePriorityQueue() : base()
    {
    }
}