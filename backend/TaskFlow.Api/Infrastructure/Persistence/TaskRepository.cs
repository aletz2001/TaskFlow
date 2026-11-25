using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Application.Abstractions;
using TaskFlow.Api.Application.Tasks;
using TaskFlow.Api.Domain;

namespace TaskFlow.Api.Infrastructure.Persistence;

internal sealed class TaskRepository(AppDbContext db) : ITaskRepository
{
    public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken ct) =>
        db.Tasks.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<IReadOnlyList<TaskItem>> ListAsync(TaskFilter filter, CancellationToken ct)
    {
        IQueryable<TaskItem> query = db.Tasks.AsNoTracking();

        if (filter.Status is { } status)
            query = query.Where(t => t.Status == status);

        if (filter.Priority is { } priority)
            query = query.Where(t => t.Priority == priority);

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            var pattern = $"%{filter.Search.Trim()}%";
            query = query.Where(t =>
                EF.Functions.Like(t.Title, pattern) ||
                EF.Functions.Like(t.Description, pattern));
        }

        return await query
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(ct);   
    }

    public void Add(TaskItem task) => db.Tasks.Add(task);

    public void Remove(TaskItem task) => db.Tasks.Remove(task);
}