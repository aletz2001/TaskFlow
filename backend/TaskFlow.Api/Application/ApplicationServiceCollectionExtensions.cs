using Microsoft.Extensions.DependencyInjection.Extensions;
using TaskFlow.Api.Application.Tasks;
using TaskFlow.Api.Application.Tasks.Sorting;

namespace TaskFlow.Api.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {

        services.AddSingleton<ITaskSortStrategy, NewestFirstSort>();
        services.AddSingleton<ITaskSortStrategy, PriorityFirstSort>();
        services.AddSingleton<ITaskSortStrategy, DueDateFirstSort>();
        services.AddSingleton<TaskSortStrategyFactory>();

        services.TryAddSingleton(TimeProvider.System);

        services.AddScoped<ITaskService, TaskService>();

        return services;
    }
}