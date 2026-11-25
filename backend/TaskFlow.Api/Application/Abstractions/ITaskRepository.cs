using TaskFlow.Api.Application.Tasks;
using TaskFlow.Api.Domain;

namespace TaskFlow.Api.Application.Abstractions;

public interface ITaskRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyList<TaskItem>> ListAsync(TaskFilter filter, CancellationToken ct);
    void Add(TaskItem task);
    void Remove(TaskItem task);
}