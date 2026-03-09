using Microsoft.EntityFrameworkCore;
using ProyectoDuolingoC_.Data;
using ProyectoDuolingoC_.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();


string connectionString = builder.Configuration.GetConnectionString("SqlProyecto");
builder.Services.AddTransient<RepositoryLogIn>();
builder.Services.AddTransient<RepositoryCursos>();
builder.Services.AddTransient<RepositoryLecciones>();
builder.Services.AddTransient<RepositoryPreguntas>();
builder.Services.AddDbContext<ProyectoContext>(options => options.UseSqlServer(connectionString));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
