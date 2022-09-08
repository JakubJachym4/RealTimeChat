using RazorPagesUI.Library.Api;
using RazorPagesUI.Library.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddRazorPages().AddRazorPagesOptions(options =>
{
    options.Conventions.AddPageRoute("/Login", "");
});

builder.Services.AddSingleton<IApiHelper, ApiHelper>();
builder.Services.AddSingleton<ILoggedInUser, LoggedInUser>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();



app.Run();
