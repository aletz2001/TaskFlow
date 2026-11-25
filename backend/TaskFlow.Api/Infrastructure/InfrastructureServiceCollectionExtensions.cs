using Microsoft.EntityFrameworkCore;
using TaskFlow.Api.Application.Abstractions;
using TaskFlow.Api.Infrastructure.Persistence;

namespace TaskFlow.Api.Infrastructure;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {

        services.AddDbContext<AppDbContext>((serviceProvider, options) =>
        {
            var connectionString = serviceProvider.GetRequiredService<IConfiguration>()
                .GetConnectionString("TaskFlow")
                ?? throw new InvalidOperationException("Falta la cadena de conexión 'ConnectionStrings:TaskFlow'.");

            options.UseSqlite(connectionString);
        });


        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<ITaskRepository, TaskRepository>();

        return services;
    }
}