using Fun_Funding.Api.Exception;
using Fun_Funding.Application;
using Fun_Funding.Application.IEmailServices;
using Fun_Funding.Domain.EmailModel;
using Fun_Funding.Infrastructure;
using Fun_Funding.Infrastructure.Dependency_Injection;
using Fun_Funding.Infrastructure.EmailService;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System.Net.Mail;

namespace Fun_Funding.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddInfrastructure(builder.Configuration);
            //add CORS
            builder.Services.AddCors(p => p.AddPolicy("MyCors", build =>
            {
                build.WithOrigins("*")
                .AllowAnyMethod()
                .AllowAnyHeader();
            }));
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionHandler>();
            })
            .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            //email service
            var smtpSettings = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

            builder.Services.AddFluentEmail(smtpSettings.FromEmail, smtpSettings.FromName)
                .AddRazorRenderer()
                .AddSmtpSender(new SmtpClient(smtpSettings.Host)
                {
                    Port = smtpSettings.Port,
                    Credentials = new System.Net.NetworkCredential(smtpSettings.UserName, smtpSettings.Password),
                    EnableSsl = true,
                });
            builder.Services.AddTransient<IEmailService, EmailService>();

            builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Your API Title",
                    Version = "v1"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"JWT Authorization header using the Bearer scheme. 
            Enter 'Bearer' [space] and then your token in the text input below.
            Example: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });



                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });
            //Add scope


            var app = builder.Build();
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Fun&Funding.Infrastructure", "Media")),
                RequestPath = "/Media"
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("MyCors");

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
