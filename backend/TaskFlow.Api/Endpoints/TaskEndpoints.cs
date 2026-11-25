using Microsoft.AspNetCore.Http.HttpResults;
using TaskFlow.Api.Application.Tasks;
using TaskFlow.Api.Application.Tasks.Sorting;

namespace TaskFlow.Api.Endpoints;

public static class TaskEndpoints
{
    public static IEndpointRouteBuilder MapTaskEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tasks").WithTags("Tasks");

        group.MapGet("/", ListTasks);
        group.MapGet("/{id:guid}", GetTask);
        group.MapPost("/", CreateTask);
        group.MapPut("/{id:guid}", UpdateTask);
        group.MapPost("/{id:guid}/start", (Guid id, ITaskService service, CancellationToken ct) =>
            ChangeStatus(id, TaskStatusAction.Start, service, ct));
        group.MapPost("/{id:guid}/complete", (Guid id, ITaskService service, CancellationToken ct) =>
            ChangeStatus(id, TaskStatusAction.Complete, service, ct));
        group.MapPost("/{id:guid}/reopen", (Guid id, ITaskService service, CancellationToken ct) =>
            ChangeStatus(id, TaskStatusAction.Reopen, service, ct));
        group.MapDelete("/{id:guid}", DeleteTask);

        return app;
    }

    private static async Task<Ok<IReadOnlyList<TaskDto>>> ListTasks(
        [AsParameters] TaskFilter filter,
        TaskSort? sort,
        ITaskService service,
        CancellationToken ct)
    {
        var tasks = await service.ListAsync(filter, sort ?? TaskSort.Newest, ct);
        return TypedResults.Ok(tasks);
    }

    private static async Task<Results<Ok<TaskDto>, NotFound>> GetTask(
        Guid id, ITaskService service, CancellationToken ct) =>
        await service.GetByIdAsync(id, ct) is { } task
            ? TypedResults.Ok(task)
            : TypedResults.NotFound();

    private static async Task<Created<TaskDto>> CreateTask(
        CreateTaskRequest request, ITaskService service, CancellationToken ct)
    {
        var task = await service.CreateAsync(request, ct);
        return TypedResults.Created($"/api/tasks/{task.Id}", task);
    }

    private static async Task<Results<Ok<TaskDto>, NotFound>> UpdateTask(
        Guid id, UpdateTaskRequest request, ITaskService service, CancellationToken ct) =>
        await service.UpdateAsync(id, request, ct) is { } task
            ? TypedResults.Ok(task)
            : TypedResults.NotFound();

    private static async Task<Results<Ok<TaskDto>, NotFound>> ChangeStatus(
        Guid id, TaskStatusAction action, ITaskService service, CancellationToken ct) =>
        await service.ChangeStatusAsync(id, action, ct) is { } task
            ? TypedResults.Ok(task)
            : TypedResults.NotFound();

    private static async Task<Results<NoContent, NotFound>> DeleteTask(
        Guid id, ITaskService service, CancellationToken ct) =>
        await service.DeleteAsync(id, ct)
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
}