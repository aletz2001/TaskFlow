using TaskFlow.Api.Application.Tasks.Sorting;

namespace TaskFlow.Api.Application.Tasks;

public interface ITaskService
{
    Task<IReadOnlyList<TaskDto>> ListAsync(TaskFilter filter, TaskSort sort, CancellationToken ct);
    Task<TaskDto?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken ct);
    Task<TaskDto?> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken ct);
    Task<TaskDto?> ChangeStatusAsync(Guid id, TaskStatusAction action, CancellationToken ct);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct);
}