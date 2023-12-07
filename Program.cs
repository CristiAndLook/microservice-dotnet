using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OpenApi;
using micronet.Data;
using micronet.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.services.AddTransient<IBlob blob>();

builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

async Task<List<Adult>> GetAdults(DataContext context) => await context.Adults.ToListAsync();

app.MapGet("/adults", async (DataContext context) => await GetAdults(context))
.WithName("GetAdults")
.WithOpenApi();

app.MapGet("/adults/{id}", async (DataContext context, int id) => await context.Adults.FindAsync(id))
.WithName("GetAdultById")
.WithOpenApi();

app.MapPost("/adults/add", async (DataContext context, Adult adult) =>
{
    context.Adults.Add(adult);
    await context.SaveChangesAsync();
    return Results.Ok(await GetAdults(context));
})
.WithName("AddAdult")
.WithOpenApi();

app.MapPost("/members/add", async (IFormFile file, DataContext context, IBlob blob, string name, string lastname, int birthyear) =>
{
    string filename = string.Format("{0}{1}.jpg", name.ToLower(), lastname.ToLower());
    if(birthyear <= 17) {
        var item = new Child() { Name = name, LastName = lastname, BirthYear = birthyear, ImageUrl = filename };
        context.Children.Add(item);
    }
    else {
        var item = new Adult() { Name = name, LastName = lastname, BirthYear = birthyear, ImageUrl = filename };
        context.Adults.Add(item);
    }
})
.WithName("AddMember")
.WithOpenApi();


app.Run();
