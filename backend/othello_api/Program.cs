var builder = WebApplication.CreateBuilder(args);

IWebHostEnvironment environment = builder.Environment;

var startup = new Startup(builder.Configuration, environment);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app);

app.UseRouting();
app.Run();
