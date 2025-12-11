using EfCoreProject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextPool<AppDbContext>((sp, o) =>
{
    //o.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TestDbContext;Trusted_Connection=True;MultipleActiveResultSets=true"));
    o.UseNpgsql("Host=localhost;Port=5432;Database=testdb;Username=postgres;Password=postgres");
});

var app = builder.Build();

app.Map("/", ([FromServices] AppDbContext db) => db.Items.ToListAsync());

app.Run();
