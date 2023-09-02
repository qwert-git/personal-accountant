using BLL.Currencies;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.Common;

namespace Presenter.Web.Pages;

public class Admin : PageModel
{
    public Admin(IHostEnvironment env, ICurrencyProvider currencyProvider)
    {
        Environment = env.EnvironmentName;
        Currencies = currencyProvider.GetAllAsync().Result;
    }
    
    public string Environment { get; private set; }
    public IReadOnlyCollection<Currency> Currencies { get; private set; }
    
    public void OnGet()
    {
        
    }
}