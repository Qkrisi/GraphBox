namespace GraphUtil.Algorithms.Arguments;

public class AlgorithmArgument<TArg> : AlgorithmArgumentBase
{
    private TArg? _Value;
    
    public TArg? Value
    {
        get => _Value;
        set
        {
            if(value != null)
                _Value = value;
        }
    }

    public override void Reset()
    {
        _Value = default;
    }

    public AlgorithmArgument(string name) : base(name)
    {
    }
}