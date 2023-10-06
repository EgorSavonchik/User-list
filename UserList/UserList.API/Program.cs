
using Microsoft.EntityFrameworkCore;
using UserList.API.Data;
using UserList.API.Services.RoleService;
using UserList.API.Services.UserService;
using UserList.API.Util.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// подключение сервисов
builder.Services.AddScoped<UserValidator>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();




// настройка и подключение базы данных
var connStr = builder.Configuration.GetConnectionString("Default");
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(connStr)
    .Options;

builder.Services.AddScoped((s) => new AppDbContext(options));
builder.Services.AddDbContext<AppDbContext>();
//

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

await DbInitializer.SeedData(app);


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
