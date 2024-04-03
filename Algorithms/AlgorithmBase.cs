using System.Collections;
using GraphUtil.Algorithms.Arguments;
using Microsoft.AspNetCore.Components;
using Index = GraphUtil.Pages.Index;

namespace GraphUtil.Algorithms;

public enum StopLevel
{
    Start,
    Always,
    Skippable
}

public abstract class AlgorithmBase
{
    protected readonly Index Page;
    
    public abstract string Name { get; }
    public virtual string FullName => Name;
    public virtual string ShortDescription => "";
    public virtual string Description => "";
    public List<string> EventHistory { get; } = new();

    public string CurrentEvent
    {
        get => EventHistory.Count == 0 ? "" : EventHistory[^1];
        set => EventHistory.Add(value);
    }

    public virtual AlgorithmArgumentBase[] Arguments => Array.Empty<AlgorithmArgumentBase>();

    public virtual bool ValidateArguments() => true;

    public virtual void Reset()
    {
        EventHistory.Clear();
        CurrentEvent = "START";
    }
    
    public abstract IEnumerator<StopLevel> Execute();
    
    protected IEnumerator<StopLevel> AlgorithmWrapper(IEnumerator algorithm)
    {
        var routineList = new List<IEnumerator> { algorithm };
        while (routineList.Count > 0)
        {
            var routine = routineList[^1];
            if (!routine.MoveNext())
            {
                routineList.RemoveAt(routineList.Count - 1);
                continue;
            }
            
            switch (routine.Current)
            {
                case StopLevel stopLevel:
                    yield return stopLevel;
                    break;
                case IEnumerator innerRoutine:
                    routineList.Add(innerRoutine);
                    continue;
            }
        }
    }

    public AlgorithmBase(Index page)
    {
        Page = page;
        CurrentEvent = "START";
    }
}