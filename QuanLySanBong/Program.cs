using QuanLySanBong.Extensions;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Vô hiệu hóa $id và $values
        options.JsonSerializerOptions.ReferenceHandler = null;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApplicationServiceExtensions(builder.Configuration);
builder.Services.AddDependencyInjectionExtensions(builder.Configuration);


var app = builder.Build();

app.UseCors("AllowSpecificOrigin");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseStaticFiles();
app.UseStaticFiles();

app.Run();