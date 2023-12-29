using NetSchool.Api.Settings;
using NetSchool.Context.Seeder;
using NetSchool.Services.Actions;
using NetSchool.Services.Books;
using NetSchool.Services.Logger;
using NetSchool.Services.RabbitMq;
using NetSchool.Services.Settings;

namespace NetSchool.Api;

public static class Bootstrapper
{
    public static IServiceCollection RegisterServices(this IServiceCollection service,
        IConfiguration configuration = null)
    {
        service
            .AddMainSettings()
            .AddLogSettings()
            .AddSwaggerSettings()
            .AddAppLogger()
            .AddDbSeeder()
            .AddApiSpecialSettings()
            .AddBookService()
            .AddRabbitMq()
            .AddActions()
            ;

        return service;
    }
}