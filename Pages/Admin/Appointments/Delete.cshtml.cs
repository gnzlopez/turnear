using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AppointmentManager.Pages.Admin.Appointments
{
    [Authorize(Roles = "Admin,Reception")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DeleteModel(ApplicationDbContext db) => _db = db;

        [BindProperty]
        public Appointment Appointment { get; set; } = new Appointment();

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
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment is null)
            {
                return NotFound();
            }

            Appointment = appointment;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var a = await _db.Appointments.FindAsync(Appointment.Id);
            if (a != null)
            {
                _db.Appointments.Remove(a);
                await _db.SaveChangesAsync();
            }
            return RedirectToPage("Index");
        }
    }
}
