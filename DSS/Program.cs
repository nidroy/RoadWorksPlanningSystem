using Microsoft.EntityFrameworkCore;
using DSS.Models;
using Microsoft.OpenApi.Models;
using System.Reflection;
using DSS.Loggers;

namespace DSS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Метод для создания полного пути к файлу журнала
            string CreateLogFilePath()
            {
                string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logging");
                string logFileName = $"DSS_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log";

                return Path.Combine(logDirectory, logFileName);
            }

            string logFilePath = CreateLogFilePath();
            builder.Logging.AddFile(logFilePath);

            // Получаем строку подключения из файла конфигурации
            string connection = builder.Configuration.GetConnectionString("DefaultConnection");

            // Добавляем контекст ApplicationContext в качестве сервиса в приложение
            builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter("Microsoft", LogLevel.Warning);
                loggingBuilder.AddFilter("System", LogLevel.Warning);
                loggingBuilder.AddFilter("Microsoft.Hosting.Lifetime", LogLevel.Information);
                loggingBuilder.AddFilter("Microsoft.AspNetCore.Hosting.Diagnostics", LogLevel.Information);
                loggingBuilder.AddFilter("Microsoft.Extensions.Hosting", LogLevel.Information);

                loggingBuilder.AddFilter((provider, category, logLevel) =>
                {
                    if (category.StartsWith("tensorflow"))
                    {
                        return false;
                    }
                    if (category.StartsWith("pandas"))
                    {
                        return false;
                    }
                    return true;
                });
            });

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Road works planning system API",
                    Version = "v1",
                    Description = "API для выполнения CRUD операций",
                });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
