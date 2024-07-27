using DestinyLimo.Common.DB; // Add this using directive for DapperContext
using DestinyLimo.Repositories; // Add this using directive for IRepositoryManager
using DestinyLimo.Repositories.impl; // Add this using directive for RepositoryManager

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// read configuration
var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

// setup DI for DapperContext
builder.Services.AddSingleton<DapperContext>(provider =>
{
    var connectionString = configuration.GetConnectionString("SqlConnection") ?? throw new Exception("Connection string is null.");
    return new DapperContext(connectionString);
});

// add common services
builder.Services.AddScoped<IRepositoryManager, RepositoryManager>();

// add controllers
builder.Services.AddControllers();

app.Run();