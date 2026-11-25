using TaskFlow.Api.Domain;

namespace TaskFlow.Api.Application.Tasks;

internal static class TaskMappings
{
    public static TaskDto ToDto(this TaskItem task, DateOnly today) => new(
        task.Id,
        task.Title,
        task.Description,
        task.Priority,
        task.Status,
        task.DueDate,
        task.CreatedAt,
        task.CompletedAt,
        task.IsOverdue(today));
}