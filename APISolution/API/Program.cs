using Microsoft.AspNetCore.DataProtection;
using RealTimeChat.API.LifeCycle;
using RealTimeChat.API.Messages;
using RealTimeChat.API.Startup;
using RealTimeChat.BusinessLogic.SignalR;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection().SetApplicationName("APP");

var connectionString = builder.Configuration.GetConnectionString("AppContextConnection") ?? throw new InvalidOperationException("Connection string 'AppContextConnection' not found.");

builder.Services.RegisterServices(connectionString);

// Serilog logger configuration
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(@"logs\log.txt", 
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: LogEventLevel.Warning)
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseAuthentication();
app.UseAuthorization();
app.UseCors("CORS");
app.UseHttpsRedirection();





app.MapControllers();
app.MapHub<WebChatHub>("/chat");

app.Lifetime.ApplicationStopping.Register(CleanApp);

app.Run();


//APP KEY EVENTS
void CleanApp()
{
    app.Logger.Log(LogLevel.Information, UserMessage.AppStopping);
    List<Task> tasks = new List<Task>();

    try
    {
        using (var scope = app.Services.CreateScope())
        using (var cleaner = scope.ServiceProvider.GetService<AppCleaner>())
        {

            if (cleaner != null)
            {
                tasks.Add(cleaner.CleanApp());

                Task.WaitAll(tasks.ToArray());
            }
               
        }
    }
    catch (Exception ex)
    {
        app.Logger.Log(LogLevel.Critical, UserMessage.SessionRefreshError);
        return;
    }

    app.Logger.Log(LogLevel.Information, UserMessage.SessionRefreshSuccess);
}