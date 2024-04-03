using Newtonsoft.Json;

namespace GraphUtil.Algorithms.Arguments.ArgumentTypes;

public static class ScriptID
{
    public const string AStarHeuristics = "AStar_heuristics";
}

public class ScriptArgument : AlgorithmArgumentBase
{
    [JsonIgnore]
    public static Dictionary<string, Dictionary<string, ScriptArgument>> PredefinedScripts = new()
    {
        {ScriptID.AStarHeuristics, new()}
    };

    public readonly string ID;
    public readonly string[] Arguments;
    
    public string FunctionName;
    public string FunctionBody;
    public string ReturnExpression;

    [JsonIgnore]
    public string FullFunctionName => $"{ID}_{FunctionName}";

    [JsonIgnore]
    public string Compiled =>
        $"function {FullFunctionName}({string.Join(", ", Arguments)}) {{\n{FunctionBody}\nreturn {ReturnExpression}\n}}";

    public bool Final { get; set; }
    
    public override void Reset()
    {
        FunctionName = FunctionBody = ReturnExpression = "";
        Final = false;
    }

    public void CopyFrom(ScriptArgument other)
    {
        FunctionName = other.FunctionName;
        FunctionBody = other.FunctionBody;
        ReturnExpression = other.ReturnExpression;
        Final = other.Final;
    }

    public ScriptArgument Finalize()
    {
        return new ScriptArgument(ID, Name, Arguments)
        {
            FunctionName = FunctionName,
            FunctionBody = FunctionBody,
            ReturnExpression = ReturnExpression,
            Final = true
        };
    }

    public ScriptArgument(string id, string name, params string[] arguments) : base(name)
    {
        ID = id;
        Arguments = arguments;
    }
}