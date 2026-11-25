using TaskFlow.Api.Domain;

namespace TaskFlow.Api.Application.Tasks.Sorting;

public sealed class NewestFirstSort : ITaskSortStrategy
{
    public TaskSort Key => TaskSort.Newest;

    public IOrderedQueryable<TaskItem> Apply(IQueryable<TaskItem> query) =>
        query.OrderByDescending(t => t.CreatedAt)
             .ThenBy(t => t.Id);
}