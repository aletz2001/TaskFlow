using TaskFlow.Api.Application.Abstractions;
using TaskFlow.Api.Application.Tasks.Sorting;
using TaskFlow.Api.Domain;

namespace TaskFlow.Api.Application.Tasks;

public sealed class TaskService(
    ITaskRepository repository,
    IUnitOfWork unitOfWork,
    TaskSortStrategyFactory sortStrategies,
    TimeProvider timeProvider) : ITaskService
{
    public async Task<IReadOnlyList<TaskDto>> ListAsync(TaskFilter filter, TaskSort sort, CancellationToken ct)
    {
        var tasks = await repository.ListAsync(filter, sortStrategies.Get(sort), ct);
        var today = Today();
        return [.. tasks.Select(t => t.ToDto(today))];
    }

    public async Task<TaskDto?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var task = await repository.GetByIdAsync(id, ct);
        return task?.ToDto(Today());
    }

    public async Task<TaskDto> CreateAsync(CreateTaskRequest request, CancellationToken ct)
    {
        var task = TaskItem.Create(
            request.Title,
            request.Description,
            request.Priority,
            request.DueDate,
            timeProvider.GetUtcNow());

        repository.Add(task);
        await unitOfWork.SaveChangesAsync(ct);

        return task.ToDto(Today());
    }

    public async Task<TaskDto?> UpdateAsync(Guid id, UpdateTaskRequest request, CancellationToken ct)
    {
        var task = await repository.GetByIdAsync(id, ct);
        if (task is null)
            return null;

        task.UpdateDetails(request.Title, request.Description, request.Priority, request.DueDate);
        await unitOfWork.SaveChangesAsync(ct);

        return task.ToDto(Today());
    }

    public async Task<TaskDto?> ChangeStatusAsync(Guid id, TaskStatusAction action, CancellationToken ct)
    {
        var task = await repository.GetByIdAsync(id, ct);
        if (task is null)
            return null;

        switch (action)
        {
            case TaskStatusAction.Start:
                task.Start();
                break;
            case TaskStatusAction.Complete:
                task.Complete(timeProvider.GetUtcNow());
                break;
            case TaskStatusAction.Reopen:
                task.Reopen();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(action), action, "Acción no soportada.");
        }

        await unitOfWork.SaveChangesAsync(ct);
        return task.ToDto(Today());
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct)
    {
        var task = await repository.GetByIdAsync(id, ct);
        if (task is null)
            return false;

        repository.Remove(task);
        await unitOfWork.SaveChangesAsync(ct);
        return true;
    }

    private DateOnly Today() => DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
}