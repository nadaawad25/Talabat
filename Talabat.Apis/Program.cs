using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using Talabat.Apis.Errors;
using Talabat.Apis.Extentions;
using Talabat.Apis.Helpers;
using Talabat.Apis.Middlewares;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Repositories.Contract;
using Talabat.Repository.Data.Contexts;
using Talabat.Repository.Identity;
using Talabat.Repository.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

var builder = WebApplication.CreateBuilder(args);

#region Configuire Service

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IConnectionMultiplexer>(
    options =>
    {
        var Connection = builder.Configuration.GetConnectionString("RedisConnection");
        return ConnectionMultiplexer.Connect(Connection);
    });
builder.Services.AddDbContext<AppIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection"));
});
     
builder.Services.AddApplicationServices();
builder.Services.IdentityServices(builder.Configuration);//Extention Method 

builder.Services.AddCors(Options =>
{
    Options.AddPolicy("MyPlolicy", options =>
    {
        options.AllowAnyHeader();
        options.AllowAnyMethod();
        //options.AllowAnyOrigin();
        options.WithOrigins(builder.Configuration["FrontBaseUrl"]);
    });
});

#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<StoreDbContext>();
    try
    {
      await dbContext.Database.MigrateAsync();
        var IdentityDbContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();
        await IdentityDbContext.Database.MigrateAsync();
        var UserManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        await AppIdentityDbContextSeed.SeedUserAsync(UserManager);
        await StoreContextSeed.SeedAsync(dbContext);
    }
    catch (Exception ex)
    {
        var logger = app.Services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during migration seeding.");
        Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
    }
}

app.UseMiddleware<ExceptionMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerMiddlewares();
}

//app.UseStatusCodePagesWithRedirects("errors/{0}");//2 request
app.UseStatusCodePagesWithReExecute("/errors/{0}");//1 request 
app.UseStaticFiles();
app.UseCors("MyPlolicy");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();

