using TaskFlow.Api.Domain;

namespace TaskFlow.Api.Application.Tasks;

public sealed record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    TaskPriority Priority,
    TaskItemStatus Status,
    DateOnly? DueDate,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt,
    bool IsOverdue);

public sealed record CreateTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateOnly? DueDate);

public sealed record UpdateTaskRequest(
    string Title,
    string? Description,
    TaskPriority Priority,
    DateOnly? DueDate);