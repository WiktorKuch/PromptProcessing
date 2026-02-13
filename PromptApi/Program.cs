using MassTransit;
using Microsoft.EntityFrameworkCore;
using PromptApi.Consumers;
using PromptApi.Infrastructure;
using PromptApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()   
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION")
        ?? builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("DB_CONNECTION missing");

    options.UseNpgsql(connectionString);
});




builder.Services.AddScoped<IOpenAiService, OpenAiService>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<PromptConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("prompt-queue", e =>
        {
            e.ConfigureConsumer<PromptConsumer>(context);
        });
    });
});


var app = builder.Build();

//Configure the HTTP request pipeline.

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();


app.UseAuthorization();

app.MapControllers();


app.Run();
