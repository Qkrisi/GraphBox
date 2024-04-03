namespace GraphUtil.Algorithms.Arguments;

public abstract class AlgorithmArgumentBase
{
    public readonly string Name;

    public abstract void Reset();

    public AlgorithmArgumentBase(string name)
    {
        Name = name;
    }
}