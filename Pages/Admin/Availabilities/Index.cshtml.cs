using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using AppointmentManager.Models;
using AppointmentManager.Data;

namespace AppointmentManager.Pages.Admin.Availabilities;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext context)
    {
        _db = context;
    }

    public IList<ProfessionalAvailability> ProfessionalAvailability { get; set; } = default!;

    public async Task OnGetAsync()
    {
        ProfessionalAvailability = await _db.ProfessionalAvailabilities
                                            .Include(p => p.Professional)
                                            .ThenInclude(p => p.Specialty)
                                            .OrderBy(p => p.Professional.LastName)
                                            .ThenBy(p => p.DayOfWeek)
                                            .ThenBy(p => p.StartTime)
                                            .ToListAsync();
    }
}
