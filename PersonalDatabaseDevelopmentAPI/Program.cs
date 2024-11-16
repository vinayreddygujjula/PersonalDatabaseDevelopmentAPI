using Microsoft.Extensions.FileProviders;
using PersonalDatabaseDevelopmentAPI.Contexts;
using PersonalDatabaseDevelopmentAPI.Models;
using System.IO;

namespace PersonalDatabaseDevelopmentAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));
            builder.Services.AddSingleton<MongoDBService>();

            // Add services to the container.
            builder.Services.AddScoped<MongoDBService>();

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", policyBuilder =>
                {
                    policyBuilder.WithOrigins("http://localhost:3000")
                                 .AllowAnyHeader()
                                 .AllowAnyMethod()
                                 .AllowCredentials();
                });
            });

            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            var app = builder.Build();

            // Inject IWebHostEnvironment to get environment details
            var env = app.Environment;

            // Ensure the "Uploads" directory exists before using it
            var uploadsPath = Path.Combine(env.ContentRootPath, "Uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath); // Create the directory if it doesn't exist
            }

            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Serve static files from the "Uploads" directory
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadsPath), // Ensure the path exists
                RequestPath = "/Uploads"
            });

            app.UseHttpsRedirection();

            app.UseCors("AllowSpecificOrigin");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
