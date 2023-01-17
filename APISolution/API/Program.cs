using System.Composition.Hosting.Core;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using RealTimeChat.API.Messages;
using RealTimeChat.API.Middleware;
using RealTimeChat.API.Startup;
using RealTimeChat.BusinessLogic.SignalR;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AppContextConnection") ?? throw new InvalidOperationException("Connection string 'AppContextConnection' not found.");

builder.Services.RegisterServices(connectionString);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors();
app.UseCookiePolicy();

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
                tasks.Add(cleaner.CleanAppSession());
                tasks.Add(cleaner.CleanBusinessLogic());

                Task.WaitAll(tasks.ToArray());
            }
               
        }
    }
    catch (Exception ex)
    {
        app.Logger.Log(LogLevel.Error, UserMessage.SessionRefreshError);
        return;
    }

    app.Logger.Log(LogLevel.Information, UserMessage.SessionRefreshSuccess);
}