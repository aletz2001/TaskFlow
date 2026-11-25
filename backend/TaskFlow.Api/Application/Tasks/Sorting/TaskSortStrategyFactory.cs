namespace TaskFlow.Api.Application.Tasks.Sorting;

public sealed class TaskSortStrategyFactory(IEnumerable<ITaskSortStrategy> strategies)
{
    private readonly Dictionary<TaskSort, ITaskSortStrategy> _strategies =
        strategies.ToDictionary(s => s.Key);

    public ITaskSortStrategy Get(TaskSort sort) =>
        _strategies.TryGetValue(sort, out var strategy)
            ? strategy
            : throw new ArgumentOutOfRangeException(nameof(sort), sort, "Ordenamiento no soportado.");
}