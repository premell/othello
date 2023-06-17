var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

IWebHostEnvironment environment = builder.Environment;

var startup = new Startup(builder.Configuration, environment);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app);





//app.UseHttpsRedirection();

//app.UseEndpoints(endpoints =>
//{
 //   endpoints.MapHub<GameHub>("/Hubs/GameHub");
//});

app.UseRouting();


/* app.MapHub<GameHub>("/Hubs/GameHub"); */
app.Run();


//app.MapControllers();

