using TaskFlow.Api.Domain;

namespace TaskFlow.Api.Application.Tasks.Sorting;

public sealed class DueDateFirstSort : ITaskSortStrategy
{
    public TaskSort Key => TaskSort.DueDate;

    public IOrderedQueryable<TaskItem> Apply(IQueryable<TaskItem> query) =>
        query.OrderBy(t => t.DueDate == null)
             .ThenBy(t => t.DueDate)
             .ThenByDescending(t => t.Priority)
             .ThenBy(t => t.Id);
}