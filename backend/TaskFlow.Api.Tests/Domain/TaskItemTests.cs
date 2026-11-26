using System.Globalization;
using TaskFlow.Api.Domain;

namespace TaskFlow.Api.Tests.Domain;

public sealed class TaskItemTests
{

    private static readonly DateTimeOffset Now = new(2026, 11, 26, 15, 0, 0, TimeSpan.Zero);

    private static TaskItem CreateValidTask(DateOnly? dueDate = null) =>
        TaskItem.Create("Repasar", null, TaskPriority.Medium, dueDate, Now);

    [Fact]
    public void Create_WithValidData_StartsAsPendingAndNormalizesText()
    {

        var task = TaskItem.Create("  Repasar  ", "   ", TaskPriority.High, null, Now);

        Assert.Equal("Repasar", task.Title);       
        Assert.Null(task.Description);                   
        Assert.Equal(TaskItemStatus.Pending, task.Status);
        Assert.Equal(Now, task.CreatedAt);
        Assert.NotEqual(Guid.Empty, task.Id);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankTitle_ThrowsDomainException(string title)
    {
        var exception = Assert.Throws<DomainException>(() =>
            TaskItem.Create(title, null, TaskPriority.Low, null, Now));

        Assert.Equal("El título es obligatorio.", exception.Message);
    }

    [Fact]
    public void Create_WithTitleTooLong_ThrowsDomainException()
    {
        var title = new string('a', TaskItem.TitleMaxLength + 1);

        Assert.Throws<DomainException>(() =>
            TaskItem.Create(title, null, TaskPriority.Low, null, Now));
    }

    [Fact]
    public void Create_WithUndefinedPriority_ThrowsDomainException()
    {
        Assert.Throws<DomainException>(() =>
            TaskItem.Create("Tarea", null, (TaskPriority)0, null, Now));
    }

    [Fact]
    public void Start_WhenPending_ChangesToInProgress()
    {
        var task = CreateValidTask();

        task.Start();

        Assert.Equal(TaskItemStatus.InProgress, task.Status);
    }

    [Fact]
    public void Start_WhenAlreadyInProgress_ThrowsDomainException()
    {
        var task = CreateValidTask();
        task.Start();

        Assert.Throws<DomainException>(task.Start);
    }

    [Fact]
    public void Complete_SetsStatusAndCompletedAt()
    {
        var task = CreateValidTask();
        var completedAt = Now.AddHours(2);

        task.Complete(completedAt);

        Assert.Equal(TaskItemStatus.Completed, task.Status);
        Assert.Equal(completedAt, task.CompletedAt);
    }

    [Fact]
    public void Complete_WhenAlreadyCompleted_ThrowsDomainException()
    {
        var task = CreateValidTask();
        task.Complete(Now);

        var exception = Assert.Throws<DomainException>(() => task.Complete(Now));

        Assert.Equal("La tarea ya está completada.", exception.Message);
    }

    [Fact]
    public void Reopen_WhenCompleted_ReturnsToPendingAndClearsCompletedAt()
    {
        var task = CreateValidTask();
        task.Complete(Now);

        task.Reopen();

        Assert.Equal(TaskItemStatus.Pending, task.Status);
        Assert.Null(task.CompletedAt);
    }

    [Fact]
    public void UpdateDetails_WhenCompleted_ThrowsDomainException()
    {
        var task = CreateValidTask();
        task.Complete(Now);

        Assert.Throws<DomainException>(() =>
            task.UpdateDetails("Otro título", null, TaskPriority.High, null));
    }

    [Theory]
    [InlineData("2026-11-25", false, true)]   
    [InlineData("2026-11-26", false, false)] 
    [InlineData("2026-11-25", true, false)]  
    public void IsOverdue_DependsOnDueDateAndStatus(string dueDate, bool completed, bool expected)
    {
        var task = CreateValidTask(DateOnly.Parse(dueDate, CultureInfo.InvariantCulture));
        if (completed)
            task.Complete(Now);

        var isOverdue = task.IsOverdue(new DateOnly(2026, 11, 26));

        Assert.Equal(expected, isOverdue);
    }

    [Fact]
    public void IsOverdue_WithoutDueDate_IsFalse()
    {
        var task = CreateValidTask(dueDate: null);

        Assert.False(task.IsOverdue(new DateOnly(2030, 1, 1)));
    }
}