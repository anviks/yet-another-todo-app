using YetAnotherTodoApp.Core.Contracts.Repositories;
using YetAnotherTodoApp.Core.Services;
using YetAnotherTodoApp.Data.Context;
using YetAnotherTodoApp.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using WebApp.Hubs;
using WebApp.Mapping;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TodoContext>(options => { options.UseNpgsql(connectionString); });

builder.Services.AddControllers();

builder.Services.AddScoped<ITodoRepository, TodoRepository>();

builder.Services.AddScoped<TodoService, TodoService>();

var origins = builder.Configuration
    .GetSection("AllowedOrigins")
    .Get<string[]>()!;

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

builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile).Assembly);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseCors("AllowCors");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app.MapHub<TodoHub>("/hubs/todo");

app.Run();