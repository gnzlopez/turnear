using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.Pages.Admin.Appointments;

public class DetailsModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public DetailsModel(ApplicationDbContext db) => _db = db;

    public Appointment Appointment { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var appointment = await _db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Professional)
                .ThenInclude(p => p!.Specialty)
            .Include(a => a.Specialty)
            .Include(a => a.Status)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (appointment is null)
        {
            return NotFound();
        }

        Appointment = appointment;

        return Page();
    }
}