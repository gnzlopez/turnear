using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AppointmentManager.Models;
using AppointmentManager.Data;

namespace AppointmentManager.Pages.Admin.Availabilities;

public class DeleteModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public DeleteModel(ApplicationDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public ProfessionalAvailability ProfessionalAvailability { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var professionalavailability = await _db.ProfessionalAvailabilities
                                                    .Include(p => p.Professional)
                                                    .ThenInclude(p => p.Specialty)
                                                    .FirstOrDefaultAsync(m => m.Id == id);

        if (professionalavailability is null)
        {
            return NotFound();
        }
        else
        {
            ProfessionalAvailability = professionalavailability;
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var professionalavailability = await _db.ProfessionalAvailabilities.FindAsync(id);
        if (professionalavailability != null)
        {
            ProfessionalAvailability = professionalavailability;
            _db.ProfessionalAvailabilities.Remove(ProfessionalAvailability);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage("./Index");
    }
}
