using EntityFrameworkApi.Database;
using EntityFrameworkApi.Model.Shared.Api;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var appSettings = builder.Configuration.GetSection("AppSettings");
builder.Services.Configure<AppSettings>(appSettings);
var setting = appSettings.Get<AppSettings>();
appSettings.Bind(setting);
builder.Services.AddSingleton<AppSettings>(setting);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DatabaseContext>(options =>
{
    options.UseMySql(setting.DefaultConnection, ServerVersion.AutoDetect(setting.DefaultConnection));
});

builder.Services.AddControllers(opt =>
{
    opt.AllowEmptyInputInBodyModelBinding = true;
    opt.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});


byte[] key = Encoding.ASCII.GetBytes(setting.Secret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "your_issuer",
        ValidAudience = "your_audience",
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

#region Auto Database Update

//using (var serviceScope = app.Services.GetService<IServiceScopeFactory>()?.CreateScope())
//{
//    if (serviceScope != null)
//    {
//        DatabaseContext cont = serviceScope.ServiceProvider.GetRequiredService<DatabaseContext>();
//        if (cont.Database.GetPendingMigrations().Count() > 0)
//        {
//            cont.Database.Migrate();
//        }
//    }
//}

#endregion


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseAuthorization();
app.MapControllers();
app.Run();
