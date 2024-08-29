using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt => {
     opt.UseSqlServer(builder.Configuration.GetConnectionString("DevConection"));
});
builder.Services.AddCors();

var app = builder.Build();
app.UseCors(x => x.AllowAnyHeader()
                  .AllowAnyMethod()
                  .WithOrigins("http://localhost:3000", 
                               "https://localhost:3000"
                              )
          );
app.MapControllers();
app.Run();
