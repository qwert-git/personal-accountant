using BLL;
using BLL.Config;
using BLL.Currencies;
using BLL.Filters;
using BLL.MerchantExtractors;
using BLL.StatementProcessing;
using BLL.StatementReaders;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

// Add Configuration
// TODO: Last time CustomMerchantMapping doesn't load
var appConfig = builder.Configuration.GetSection("AppConfig").Get<AppConfig>() ?? throw new ApplicationException("AppConfig is not configured");
builder.Services.AddSingleton(appConfig.CategoryMapping ?? throw new ApplicationException("CategoryMapping is not configured"));
builder.Services.AddSingleton(appConfig.CurrencyProvider ?? throw new ArgumentException("CurrencyProvider is not configured"));

// var pathToFile = builder.Configuration.GetSection("StatementsPath").Get<string>() ?? throw new ArgumentException("StatementsPath is not configured");

// Add services to the container.
builder.Services.AddSingleton<ICurrencyProvider, ApiCurrencyProvider>();
builder.Services.AddSingleton<IReadOnlyCollection<IMerchantExtractor>>(new IMerchantExtractor[]
{
    new BogMerchantExtractor(),
    new CustomMerchantExtractor(appConfig.CustomMerchantMapping)
});
builder.Services.AddSingleton<ICategoryMapper, CategoryMapper>();
builder.Services.AddSingleton<IStatementsReader, BogStatementReader>();
builder.Services.AddSingleton<ReportGenerator>(serviceProvider => new ReportGenerator(
    serviceProvider.GetRequiredService<IStatementsReader>(),
    serviceProvider.GetRequiredService<IReadOnlyCollection<IMerchantExtractor>>(),
    serviceProvider.GetRequiredService<ICategoryMapper>(),
    serviceProvider.GetRequiredService<ICurrencyProvider>(),
    appConfig.CurrencyProvider,
    new ITransactionsFilter[]
    {
        new PurposeStopWordFilter("Automatic conversion"),
        new PurposeStopWordFilter("Incoming Transfer")
    },
    serviceProvider.GetRequiredService<ILogger<ReportGenerator>>()
));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();