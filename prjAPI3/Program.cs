using prjAPI3.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// 1. ELIMINADA: Se elimina builder.Services.AddOpenApi(); (no es compatible con AddSwaggerGen)

// Usar el paquete est�ndar Swashbuckle para Swagger
builder.Services.AddSwaggerGen();

// Agregar la capa de datos como Singleton (buena pr�ctica para la conexi�n a DB)
builder.Services.AddSingleton<clsCatEmpleadosData>();

// Configuraci�n de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("NuevaPolitica", app =>
    {
        app.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // 2. ELIMINADA: Se elimina app.MapOpenApi();

    // Usar Swagger y SwaggerUI
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

// Aplicar la pol�tica de CORS
app.UseCors("NuevaPolitica");

app.Run();
