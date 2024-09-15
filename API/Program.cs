using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

app.UseCors(x => x.AllowAnyHeader()
                  .AllowAnyMethod()
                  .WithOrigins(
                    "http://localhost:3000",
                    "https://localhost:3000"
                  )
          );
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();