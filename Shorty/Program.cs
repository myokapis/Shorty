using LiteDB.Async;
using Serilog;
using Shorty.Configs;
using Shorty.Models;
using Shorty.Services;

namespace Shorty
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // setup a bootstrap logger to capture output until the real logger is configured
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateBootstrapLogger();

            builder.Services.Configure<EncoderConfig>(builder.Configuration.GetSection("Encoder"));
            var dataStore = await InitializeDataStore();

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();
            builder.Services.AddSwaggerGen();
            builder.Services.AddScoped<IDataService, DataService>();
            builder.Services.AddScoped<IEncodingService, EncodingService>();
            builder.Services.AddScoped<IUrlService, UrlService>();
            builder.Services.AddScoped((service) => KeyProvider(service));
            builder.Services.AddSingleton(dataStore);

            builder.Host.UseSerilog((context, serilogConfig) =>
                serilogConfig.ReadFrom.Configuration(context.Configuration));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler("/Error");
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }

        private static Func<byte[]> KeyProvider(IServiceProvider _serviceProvider)
        {
            return new Func<byte[]>(() =>
            {
                // NOTE: this GUID is based on timestamp followed by other goodies. It's a
                //       better choice than timestamp alone which would inevitably produce
                //       collisions in a high volume environment. With excess bytes available,
                //       we can choose how many bytes to use.
                var guid = Guid.CreateVersion7();
                return guid.ToByteArray()[..12];
            });
        }

        private static async Task<ILiteDatabaseAsync> InitializeDataStore()
        {
            var appDir = AppDomain.CurrentDomain.BaseDirectory;
            var dbDir = Path.Combine(appDir, @"Database");
            Directory.CreateDirectory(dbDir);
            var dbFilePath = Path.Combine(dbDir, "UrlDatabase.db");

            var dbEngine = new LiteDatabaseAsync(dbFilePath);
            var urlCol = dbEngine.GetCollection<UrlDocument>();
            await urlCol.EnsureIndexAsync(c => c.FullUrl);
            await urlCol.EnsureIndexAsync(c => c.Tag);

            return dbEngine;
        }
    }
}
