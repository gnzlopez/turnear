using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AppointmentManager.Models;
using AppointmentManager.Data;

namespace AppointmentManager.Pages.Admin.Availabilities;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public DetailsModel(ApplicationDbContext db)
    {
        _db = db;
    }

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
}
