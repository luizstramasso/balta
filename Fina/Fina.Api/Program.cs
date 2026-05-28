using Fina.Api;
using Fina.Api.Common.Api;
using Fina.Api.Endpoints;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddConfiguration();
        builder.AddDataContext();
        builder.AddCrossOrigin();
        builder.AddDocumentation();
        builder.AddHandlers();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
            app.ConfigureDevEnvironment();

        app.UseCors(ApiConfiguration.CorsPolicyName);
        app.MapEndpoints();

        app.Run();
    }
}