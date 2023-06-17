using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using othello_api.Contexts;
using othello_api.Services;

public class Startup
{
    private IConfigurationRoot Configuration { get; }
    private readonly IWebHostEnvironment _env;

    //private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    public Startup(IConfigurationRoot configuration, IWebHostEnvironment env)
    {
        Configuration = configuration;
        _env = env;
    }



    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContextFactory<OthelloContext>(
            options => options.UseSqlServer(Configuration.GetConnectionString("DBConnection"))
        );

        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.SetMinimumLevel(LogLevel.Information); // Set the desired log level
        });


        services.AddSignalR(hubOptions =>
        {
            hubOptions.ClientTimeoutInterval = TimeSpan.FromMinutes(10); // Client timeout interval
            hubOptions.KeepAliveInterval = TimeSpan.FromMinutes(40); // Keep alive interval
        });
        services.AddHttpClient();

        services.AddSingleton<GameDataService>();

        services.AddTransient<OthelloService>(
            serviceProvider =>
                new OthelloService(
                    serviceProvider.GetService<OthelloContext>(),
                    serviceProvider.GetService<IHubContext<GameHub>>(),
                    serviceProvider.GetService<GameDataService>(),
                    serviceProvider.GetService<IServiceScopeFactory>()
                )
        );

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddControllers();

        services.AddCors(options =>
        {
            options.AddPolicy("DevelopmentPolicy",
                builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();

                });

            options.AddPolicy("ProductionPolicy",
                builder =>
                {
                    builder.WithOrigins("https://othello-again.azurewebsites.net")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
        });


    }

    public void Configure(IApplicationBuilder app)
    {
        // Configure the HTTP request pipeline.
        if (_env.IsDevelopment())
        {
            app.UseCors("DevelopmentPolicy");
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseCors("ProductionPolicy");
        }

        app.UseRouting();
        //app.UseWebSockets();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHub<GameHub>("/Hubs/GameHub");
        });

        //app.UseAuthorization();

        //app.UseCors(builder => builder.AllowAnyOrigin());
        //{
        //app.UseCors(builder => builder.WithOrigins("http://localhost:5173"));
        //}
    }
}
