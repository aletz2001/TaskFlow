using TaskFlow.Api.Domain;

namespace TaskFlow.Api.Application.Tasks.Sorting;

public sealed class PriorityFirstSort : ITaskSortStrategy
{
    public TaskSort Key => TaskSort.Priority;

    public IOrderedQueryable<TaskItem> Apply(IQueryable<TaskItem> query) =>
        query.OrderByDescending(t => t.Priority)
             .ThenBy(t => t.DueDate == null)
             .ThenBy(t => t.DueDate)
             .ThenBy(t => t.Id);
}