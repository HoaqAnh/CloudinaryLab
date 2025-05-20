// Các using hiện có của bạn
using Cloudinary.Service;
using Cloudinary.Settings;

var builder = WebApplication.CreateBuilder(args);

// === BẮT ĐẦU PHẦN THÊM CORS ===
// Đặt tên cho chính sách CORS cụ thể
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173") 
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials(); // Nếu bạn cần gửi cookies/credentials
                      });
});
// === KẾT THÚC PHẦN THÊM CORS ===

// Add services to the container.
builder.Services.Configure<CloudinarySetting>(
    builder.Configuration.GetSection("Cloudinary"));

builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IVideoService, VideoService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// === ÁP DỤNG MIDDLEWARE CORS ===
// Đảm bảo UseCors() được gọi SAU UseRouting (ngầm định có)
// và TRƯỚC UseAuthorization() và app.MapControllers().
app.UseCors(MyAllowSpecificOrigins);
// === KẾT THÚC ÁP DỤNG MIDDLEWARE CORS ===

app.UseAuthorization();

app.MapControllers();

app.Run();