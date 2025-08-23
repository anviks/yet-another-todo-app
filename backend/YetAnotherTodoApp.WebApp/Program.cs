using System.Text.Json.Serialization;
using YetAnotherTodoApp.Core.Contracts.Repositories;
using YetAnotherTodoApp.Core.Services;
using YetAnotherTodoApp.Data.Context;
using YetAnotherTodoApp.Data.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Hubs;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
          throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TodoContext>(options =>
{
    options.EnableSensitiveDataLogging();
    options.UseNpgsql(connectionString);
});

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<TodoContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddRazorPages();
builder.Services.AddControllers()
    .AddJsonOptions(options => { options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles; });

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<ITodoRepository, TodoRepository>();
// builder.Services.AddScoped<IUserRepository, UserRepository>();
//
builder.Services.AddScoped<TodoService, TodoService>();
// builder.Services.AddScoped<UserService, UserService>();
//
// builder.Services.AddSingleton<IGameStore, InMemoryGameStore>();
//
// builder.Services.Configure<UserLimitsConfig>(builder.Configuration.GetSection("UserLimits"));
// builder.Services.Configure<EmailConfig>(builder.Configuration.GetSection("Email"));

// builder.Services.AddSession(options =>
// {
//     // options.Cookie.Name = "YourSessionCookieName";
//     options.IdleTimeout = TimeSpan.FromMinutes(20); // Set the session timeout duration
// });

var origins = builder.Configuration
    .GetSection("AllowedOrigins")
    .GetChildren()
    .Select(child => child.Value!)
    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCors", policyBuilder =>
    {
        policyBuilder
            .WithOrigins(origins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddSignalR();

// builder.Services.AddAuthentication("UnoToken")
//     .AddScheme<AuthenticationSchemeOptions, UnoTokenAuthenticationHandler>("UnoToken", null);

builder.Services.AddAuthorization();

// builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseSession();

app.UseCors("AllowCors");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllers();

app.MapHub<TodoHub>("/hubs/todo");

app.Run();