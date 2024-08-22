using API.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt => {
     opt.UseSqlServer(builder.Configuration.GetConnectionString("DevConection"));
});
var app = builder.Build();
app.MapControllers();
app.Run();
