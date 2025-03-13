using AutoMapper;
using BusinessLayer.Validation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();



// Register AutoMapper
builder.Services.AddSingleton<IMapper>(sp =>
{
    var config = new MapperConfiguration(cfg =>
    {
        cfg.AddMaps(AppDomain.CurrentDomain.GetAssemblies());
    });

    return config.CreateMapper();
});

// Add FluentValidation
builder.Services.AddControllers()
    .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<AddressBookValidator>());



// Add OpenAPI (Swagger) support
builder.Services.AddEndpointsApiExplorer();

var xmlFile = "AddressBookApplication.xml";
var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

if (File.Exists(xmlPath))
{
    builder.Services.AddSwaggerGen(options =>
    {
        options.IncludeXmlComments(xmlPath);
    });
}
else
{
    Console.WriteLine($"?? Warning: XML documentation file not found at {xmlPath}");
}



// Configure Database Context
var connectionString = builder.Configuration.GetConnectionString("SqlConnection");
builder.Services.AddDbContext<AddressBookContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();



// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
