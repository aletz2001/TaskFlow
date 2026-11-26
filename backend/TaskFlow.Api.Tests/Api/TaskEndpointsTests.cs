using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using TaskFlow.Api.Application.Tasks;
using TaskFlow.Api.Domain;

namespace TaskFlow.Api.Tests.Api;

public sealed class TaskEndpointsTests(TaskFlowApiFactory factory) : IClassFixture<TaskFlowApiFactory>
{

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateThenGet_ReturnsTheCreatedTask()
    {
        var request = new CreateTaskRequest("Repasar React", "Hooks y estado", TaskPriority.High, null);

        var createResponse = await _client.PostAsJsonAsync("/api/tasks", request, JsonOptions);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var created = await createResponse.Content.ReadFromJsonAsync<TaskDto>(JsonOptions);
        Assert.NotNull(created);
        Assert.Equal($"/api/tasks/{created.Id}", createResponse.Headers.Location?.OriginalString);

        var fetched = await _client.GetFromJsonAsync<TaskDto>($"/api/tasks/{created.Id}", JsonOptions);
        Assert.Equal("Repasar React", fetched?.Title);
        Assert.Equal(TaskPriority.High, fetched?.Priority);
    }

    [Fact]
    public async Task Create_WithBlankTitle_ReturnsProblemDetails400()
    {
        var request = new CreateTaskRequest("   ", null, TaskPriority.Low, null);

        var response = await _client.PostAsJsonAsync("/api/tasks", request, JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>(JsonOptions);
        Assert.Equal("El título es obligatorio.", problem?.Detail);
    }

    [Fact]
    public async Task Complete_CalledTwice_SecondCallReturns400()
    {
        var created = await CreateTaskAsync("Completar dos veces");

        var first = await _client.PostAsync($"/api/tasks/{created.Id}/complete", content: null);
        var second = await _client.PostAsync($"/api/tasks/{created.Id}/complete", content: null);

        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
    }

    [Fact]
    public async Task Get_WithUnknownId_Returns404()
    {
        var response = await _client.GetAsync($"/api/tasks/{Guid.NewGuid()}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_RemovesTheTask()
    {
        var created = await CreateTaskAsync("Para borrar");

        var deleteResponse = await _client.DeleteAsync($"/api/tasks/{created.Id}");
        var getResponse = await _client.GetAsync($"/api/tasks/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private async Task<TaskDto> CreateTaskAsync(string title)
    {
        var request = new CreateTaskRequest(title, null, TaskPriority.Medium, null);
        var response = await _client.PostAsJsonAsync("/api/tasks", request, JsonOptions);
        response.EnsureSuccessStatusCode();

        var task = await response.Content.ReadFromJsonAsync<TaskDto>(JsonOptions);
        return task ?? throw new InvalidOperationException("La API no devolvió la tarea creada.");
    }
}