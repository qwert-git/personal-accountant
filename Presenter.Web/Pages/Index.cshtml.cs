using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Presenter.Web.Pages;

public class IndexModel : PageModel
{
    private IWebHostEnvironment _environment;

    public IndexModel(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public void OnGet() { }
    
    [BindProperty]
    public IFormFile? Upload { get; set; }
    
    public async Task<IActionResult> OnPostAsync()
    {
        if (Upload is null)
        {
            return BadRequest();
        }

        var file = Path.Combine(_environment.ContentRootPath, Upload.FileName);
        await using (var fileStream = new FileStream(file, FileMode.Create))
        {
            await Upload.CopyToAsync(fileStream);
        }

        return RedirectToPage("Report", "FromHomePage", new { pathToFile = file });
    }
}