using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.OpenApi;
using micronet.Data;
using micronet.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IBlob, Blob>();

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

app.MapPost("members/add", async (IFormFile file, DataContext context, IBlob blob, string name, string LastName, int birthyear) =>
{
    string fileName = string.Format("{0}{1}.jpg", name.ToLower(), LastName.ToLower());

    if (birthyear <= 17)
    {
        var item = new Child
        {
            Name = name,
            LastName = LastName,
            BirthYear = birthyear,
            ImageUrl = fileName
        };
        context.Children.Add(item);
    }
    else
    {
        var item = new Adult
        {
            Name = name,
            LastName = LastName,
            BirthYear = birthyear,
            ImageUrl = fileName
        };
        context.Adults.Add(item);
    }

    await context.SaveChangesAsync();   
    await blob.Upload(file, fileName);
    return Results.Ok();


})
.WithName("AddMember");


app.Run();
