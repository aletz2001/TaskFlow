using TaskFlow.Api.Domain;

namespace TaskFlow.Api.Application.Tasks;

public sealed record TaskFilter(
    TaskItemStatus? Status = null,
    TaskPriority? Priority = null,
    string? Search = null);