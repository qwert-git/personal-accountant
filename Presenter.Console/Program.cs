using BLL;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Presenter.Console;
using Presenter.Console.DI;

var host = CreateHostBuilder(args).Build();

var generator = host.Services.GetRequiredService<ReportGenerator>();
var presenter = host.Services.GetRequiredService<ConsolePresenter>();

var report = await generator.GenerateAsync();
presenter.Present(report);

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((_, builder) =>
        {
            builder.AddJsonFile("appsettings.json");
            builder.AddEnvironmentVariables();
        })
        .RegisterAppServices(args)
        .ConfigureLogging((_, logging) =>
        {
            logging.AddSimpleConsole(options => options.IncludeScopes = true);
        });