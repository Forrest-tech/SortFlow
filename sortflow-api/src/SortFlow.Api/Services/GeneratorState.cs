namespace SortFlow.Api.Services;

public sealed class GeneratorState : IGeneratorState
{
    public bool IsRunning { get; set; } = true;
}
