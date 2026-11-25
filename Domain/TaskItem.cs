namespace TaskFlow.Api.Domain

public sealed class TaskItem
{
    public const int TitleMaxLenght = 100;
    public const int DescriptionMaxLenght = 500;

    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TaskPriority Priority { get; private set; }
    public TaskItemStatus Status { get; private set; }
    public DateOnly? DueDate { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    private TaskItem() { }

    public static TaskItem Create(
        string title,
        string? description,
        TaskPriority priority,
        DateOnly? dueDate,
        DateTimeOffset now)
    {
        var task = new TaskItem
        {
            Id = Guid.CreateVersion7(now),
            Status = TaskItemStatus.Pending,
            CreatedAt = now
        };
        task.SetDeatils(title, description, priority, dueDate);
        return task;
    }

    public void UpdateDetails(string title, string? description, TaskPriority priority, DateOnly? dueDate)
    {
        if (Status == TaskItemStatus.Completed)
            throw new DomainException("No se puede editar una tarea completada.");

        SetDetails(title, description, priority, dueDate);
    }
    public void Start()
    {
        if (Status != TaskItemStatus.Pending)
            throw new DomainException("Solo se puede iniciar una tarea pendiente.");

        Status = TaskItemStatus.InProgress;
    }
    public void Complete(DateTimeOffset now)
    {
        if (Status == TaskItemStatus.Completed)
            throw new DomainException("La tarea ya está completada.");

        Status = TaskItemStatus.Completed;
        CompletedAt = now;
    }
    public void Reopen()
    {
        if (Status != TaskItemStatus.Completed)
            throw new DomainException("Solo se puede reabrir una tarea completada.");

        Status = TaskItemStatus.Pending;
        CompletedAt = null;
    }

    public bool IsOverdue(DateOnly today) =>
        Status != TaskItemStatus.Completed && DueDate < today;

    private void SetDetails(string title, string? description, TaskPriority priority, DateOnly? dueDate)
    {
        Title = NormalizeTitle(title);
        Description = NormalizeDescription(description);
        Priority = Enum.IsDefined(priority)
            ? priority
            : throw new DomainException("La prioridad no es válida.");
        DueDate = dueDate;
    }

    private static string NormalizeTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new DomainException("El título es obligatorio.");

        var trimmed = title.Trim();
        if (trimmed.Length > TitleMaxLength)
            throw new DomainException($"El título no puede superar los {TitleMaxLength} caracteres.");

        return trimmed;
    }

    private static string? NormalizeDescription(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return null;

        var trimmed = description.Trim();
        if (trimmed.Length > DescriptionMaxLength)
            throw new DomainException($"La descripción no puede superar los {DescriptionMaxLength} caracteres.");

        return trimmed;
    }
}