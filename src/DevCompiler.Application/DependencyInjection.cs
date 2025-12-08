using DevCompiler.Application.Interfaces;
using DevCompiler.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DevCompiler.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IRoomService, RoomService>();
       
        return services;
    }
}