using Football247.Data;
using Football247.Mappings;
using Football247.Middleware;
using Football247.Models.Entities;
using Football247.Repositories;
using Football247.Repositories.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddLogging(logging =>
{
    logging.AddConsole(); 
    logging.AddDebug();   
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Để sử dụng IHttpContextAccessor trong các lớp khác như repository, service, ...
builder.Services.AddHttpContextAccessor();


// Cấu hình JWT Bearer Authentication
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Football247 API", Version = "v1" });

    // Khai báo cơ chế bảo mật
    // Thêm nút "Authorize" ở góc trên bên phải của giao diện Swagger UI
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });

    // Bảo Swagger gắn JWT Bearer Authentication vào các request
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme, 
                    Id = JwtBearerDefaults.AuthenticationScheme
                },
                Scheme = "Oauth2",
                Name = JwtBearerDefaults.AuthenticationScheme, 
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddDbContext<Football247DbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Football247ConnectionString")));

builder.Services.AddDbContext<Football247AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Football247AuthConnectionString")));


builder.Services.AddIdentity<ApplicationUser, IdentityRole>() 
    .AddEntityFrameworkStores<Football247AuthDbContext>()
    .AddDefaultTokenProviders();

// Setting Up Identity 
//builder.Services.AddIdentityCore<IdentityUser>()
//    .AddRoles<IdentityRole>()
//    .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("Football247")
//    .AddEntityFrameworkStores<Football247AuthDbContext>()
//    .AddDefaultTokenProviders();

// Cấu hình Quy tắc cho Mật khẩu
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

// * ĐĂNG KÝ Authentication
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
            };
        }
);

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();
app.UseHttpsRedirection();

// Dòng code này nhằm cấu hình ASP.NET Core để phục vụ (serve) các file tĩnh từ thư mục Images, nằm trong gốc của dự án.
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Images")),
    RequestPath = "/Images"
    // https://Localhost:1234/Images

});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
