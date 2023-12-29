using Microsoft.Extensions.DependencyInjection;
using NetSchool.Services.Books.Books;

namespace NetSchool.Services.Books;

public static class Bootstrapper
{
    public static IServiceCollection AddBookService(this IServiceCollection services)
    {
        return services
            .AddSingleton<IBookService, BookService>();
    }
}