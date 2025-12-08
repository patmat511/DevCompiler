using DevCompiler.Domain.Interfaces;
using DevCompiler.Infrastructure.Compiler;
using DevCompiler.Infrastructure.Data;
using DevCompiler.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

namespace DevCompiler.Infrastructure;

public static class DependencyInjection
{
   public static IServiceCollection AddInfrastructure(
       this IServiceCollection services,
       IConfiguration configuration
       )
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
            ));

        // Repositories
        services.AddScoped<IRoomRepository, RoomReposiotry>();


        // Services
        services.AddScoped<ICompilerService, RoslynCompilerService>();
        // Jwt

        return services;
    }
}