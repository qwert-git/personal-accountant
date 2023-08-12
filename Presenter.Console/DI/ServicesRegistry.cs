using BLL;
using BLL.Config;
using BLL.Currencies;
using BLL.Filters;
using BLL.MerchantExtractors;
using BLL.StatementProcessing;
using BLL.StatementReaders;
using CommandLine;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Presenter.Console.DI;

internal static class ServicesRegistry
{
    public static IHostBuilder RegisterAppServices(this IHostBuilder builder, IEnumerable<string> args)
    {
        return builder.ConfigureServices((context, services) =>
        {
            // Add configurations to DI
            var appConfig = context.Configuration.Get<AppConfig>() ??
                            throw new ApplicationException("AppConfig is not configured");
            services.AddSingleton(appConfig.CategoryMapping);
            services.AddSingleton(appConfig.CurrencyProvider);

            // Proceed command line arguments
            var options = appConfig.IsDevelopment
                ? new AppOptions { StatementsPath = "./23-jun(1).xlsx" }
                : ParseAppOptions(args);
            services.AddSingleton(options);

            // Add services to DI
            services.AddSingleton<ReportGenerator>();
            services.AddSingleton<IStatementsReader, BogStatementReader>(
                serviceProvider => new BogStatementReader(
                    options.StatementsPath,
                    serviceProvider.GetRequiredService<ICurrencyProvider>()
                )
            );
            services.AddSingleton<ICategoryMapper, CategoryMapper>();
            services.AddSingleton<IReadOnlyCollection<IMerchantExtractor>>(new IMerchantExtractor[]
            {
                new BogMerchantExtractor(),
                new CustomMerchantExtractor(appConfig.CustomMerchantMapping)
            });
            services.AddSingleton<IReadOnlyCollection<ITransactionsFilter>>(new ITransactionsFilter[]
            {
                new PurposeStopWordFilter("Automatic conversion"),
                new PurposeStopWordFilter("Incoming Transfer")
            });

            if (appConfig.IsDevelopment)
            {
                services.AddSingleton<ICurrencyProvider, StaticCurrencyProvider>();
            }
            else
            {
                services.AddSingleton<ICurrencyProvider, ApiCurrencyProvider>();
            }

            // Add presenters to DI
            services.AddSingleton<ConsolePresenter>();
        });
    }

    private static AppOptions ParseAppOptions(IEnumerable<string> arguments)
    {
        return Parser.Default.ParseArguments<AppOptions>(arguments)
            .WithParsed(appOptions =>
            {
                System.Console.WriteLine("App starts with options:");
                System.Console.WriteLine(appOptions);
                System.Console.WriteLine();
            })
            .Value;
    }
}