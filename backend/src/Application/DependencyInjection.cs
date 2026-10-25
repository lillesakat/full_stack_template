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
        services.AddScoped<IStaffGroupService, StaffGroupService>();
        services.AddScoped<StaffGroupValidator>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<UserValidator>();
        services.AddScoped<IEmploymentService, EmploymentService>();
        services.AddScoped<EmploymentValidator>();
        services.AddScoped<IStaffService, StaffService>();

        return services;
    }
}
