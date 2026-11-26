using TaskFlow.Api.Application.Abstractions;
using TaskFlow.Api.Application.Tasks;
using TaskFlow.Api.Application.Tasks.Sorting;
using TaskFlow.Api.Domain;

namespace TaskFlow.Api.Tests.Fakes;


internal sealed class InMemoryTaskRepository : ITaskRepository
{
    private readonly List<TaskItem> _tasks = [];

    public IReadOnlyList<TaskItem> Tasks => _tasks;

    public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(_tasks.FirstOrDefault(t => t.Id == id));

    public Task<IReadOnlyList<TaskItem>> ListAsync(TaskFilter filter, ITaskSortStrategy sort, CancellationToken ct)
    {
        var query = _tasks.AsQueryable();

        if (filter.Status is { } status)
            query = query.Where(t => t.Status == status);

        if (filter.Priority is { } priority)
            query = query.Where(t => t.Priority == priority);

        IReadOnlyList<TaskItem> result = sort.Apply(query).ToList();
        return Task.FromResult(result);
    }

    public void Add(TaskItem task) => _tasks.Add(task);

    public void Remove(TaskItem task) => _tasks.Remove(task);
}