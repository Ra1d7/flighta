using Flighta.DataAccess;
using Flighta.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMvcCore().AddApiExplorer();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    options => {
                        options.LoginPath = "/Login";
                    }
                ).AddJwtBearer(opts =>
                {
                    opts.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration.GetValue<string>("Authentication:Issuer"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Authentication:SecretKey"))),
                        ValidAudience = builder.Configuration.GetValue<string>("Authentication:Audience"),
                        ValidateLifetime = true
                    };
                }); ;
builder.Services.AddAuthorization(opts =>
{
    opts.AddPolicy("Admin", policy => { policy.RequireClaim("Role", "Admin"); });
    opts.AddPolicy("Client", policy => { policy.RequireClaim("Role", "Client"); });

});
builder.Services.AddSingleton(new FlightDB(builder.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blazor API V1");
});
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=login}/{action=Index}/{id?}");

app.Run();
