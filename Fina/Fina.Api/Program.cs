using Fina.Api.Data;
using Microsoft.EntityFrameworkCore;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContextPool<AppDataContext>(
            opt => opt
                .UseNpgsql(@"Host=localhost;Port=54857;Database=FinaApp;Username=FinaApp;Password=PostFina@123;TrustServerCertificate=True")
                .UseSnakeCaseNamingConvention());

        var app = builder.Build();

        app.MapGet("/", () => "Hello World!");

        app.Run();
    }
}