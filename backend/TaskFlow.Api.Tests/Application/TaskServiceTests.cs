using Microsoft.Extensions.Time.Testing;
using TaskFlow.Api.Application.Tasks;
using TaskFlow.Api.Application.Tasks.Sorting;
using TaskFlow.Api.Domain;
using TaskFlow.Api.Tests.Fakes;

namespace TaskFlow.Api.Tests.Application;

public sealed class TaskServiceTests
{
    private readonly InMemoryTaskRepository _repository = new();
    private readonly FakeUnitOfWork _unitOfWork = new();
    private readonly FakeTimeProvider _time = new(new DateTimeOffset(2026, 11, 26, 15, 0, 0, TimeSpan.Zero));
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        var sortStrategies = new TaskSortStrategyFactory(
            [new NewestFirstSort(), new PriorityFirstSort(), new DueDateFirstSort()]);

        _service = new TaskService(_repository, _unitOfWork, sortStrategies, _time);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_AddsTaskAndSavesOnce()
    {
        var request = new CreateTaskRequest("Estudiar DI", null, TaskPriority.High, null);

        var dto = await _service.CreateAsync(request, CancellationToken.None);

        var stored = Assert.Single(_repository.Tasks);  
        Assert.Equal(stored.Id, dto.Id);
        Assert.Equal(_time.GetUtcNow(), dto.CreatedAt);     
        Assert.Equal(1, _unitOfWork.SaveCount);         
    }

    [Fact]
    public async Task CreateAsync_WithBlankTitle_ThrowsAndDoesNotSave()
    {
        var request = new CreateTaskRequest("", null, TaskPriority.Low, null);

        await Assert.ThrowsAsync<DomainException>(() =>
            _service.CreateAsync(request, CancellationToken.None));

        Assert.Empty(_repository.Tasks);
        Assert.Equal(0, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task ChangeStatusAsync_WhenTaskDoesNotExist_ReturnsNullAndDoesNotSave()
    {
        var result = await _service.ChangeStatusAsync(Guid.NewGuid(), TaskStatusAction.Complete, CancellationToken.None);

        Assert.Null(result);
        Assert.Equal(0, _unitOfWork.SaveCount);
    }

    [Fact]
    public async Task ChangeStatusAsync_Complete_UsesCurrentTime()
    {
        var created = await _service.CreateAsync(
            new CreateTaskRequest("Entregar proyecto", null, TaskPriority.High, null), CancellationToken.None);
        _time.Advance(TimeSpan.FromHours(3));   

        var completed = await _service.ChangeStatusAsync(created.Id, TaskStatusAction.Complete, CancellationToken.None);

        Assert.NotNull(completed);
        Assert.Equal(TaskItemStatus.Completed, completed.Status);
        Assert.Equal(_time.GetUtcNow(), completed.CompletedAt);
    }

    [Fact]
    public async Task GetByIdAsync_AfterDueDatePasses_MarksTaskAsOverdue()
    {
        var created = await _service.CreateAsync(
            new CreateTaskRequest("Pagar la luz", null, TaskPriority.High, new DateOnly(2026, 11, 27)),
            CancellationToken.None);
        Assert.False(created.IsOverdue);

        _time.Advance(TimeSpan.FromDays(2));

        var later = await _service.GetByIdAsync(created.Id, CancellationToken.None);
        Assert.NotNull(later);
        Assert.True(later.IsOverdue);
    }

    [Fact]
    public async Task ListAsync_SortedByPriority_ReturnsHighestPriorityFirst()
    {
        await _service.CreateAsync(new CreateTaskRequest("Baja", null, TaskPriority.Low, null), CancellationToken.None);
        await _service.CreateAsync(new CreateTaskRequest("Alta", null, TaskPriority.High, null), CancellationToken.None);
        await _service.CreateAsync(new CreateTaskRequest("Media", null, TaskPriority.Medium, null), CancellationToken.None);

        var result = await _service.ListAsync(new TaskFilter(), TaskSort.Priority, CancellationToken.None);

        Assert.Equal(new[] { "Alta", "Media", "Baja" }, result.Select(t => t.Title));
    }
}