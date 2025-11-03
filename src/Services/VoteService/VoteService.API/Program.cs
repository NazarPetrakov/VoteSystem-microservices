using Common.Extensions;
using Common.Middlewares;
using VoteService.API.Data;
using VoteService.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.
builder.Services
    .AddServices()
    .AddMSSqlDb<VoteDbContext>(
        configuration.GetConnectionString("DefaultConnection"))
    .AddCommonOptions(configuration)
    .AddMassTransitWithRabbitMq()
    .AddAppAuthentication(configuration);

builder.Services.AddControllers();

builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(c =>
{
    c.AllowAnyHeader().AllowCredentials().AllowAnyMethod().WithOrigins("http://localhost:4200");
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
