using TaskFlow.Api.Domain;

namespace TaskFlow.Api.Application.Tasks.Sorting;

public interface ITaskSortStrategy
{
    TaskSort Key { get; }
    IOrderedQueryable<TaskItem> Apply(IQueryable<TaskItem> query);
}