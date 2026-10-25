using Application.Interfaces;
using Application.Services;
using Application.Validators;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPeriodService, PeriodService>();
        services.AddScoped<PeriodValidator>();
        services.AddScoped<IDepartmentService, DepartmentService>();
        services.AddScoped<DepartmentValidator>();

        return services;
    }
}
