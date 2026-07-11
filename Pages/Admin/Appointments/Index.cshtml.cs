using System.Collections.Generic;
using System.Threading.Tasks;
using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.Pages.Admin.Appointments
{
    [Authorize(Roles = "Admin,Reception")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db) => _db = db;

        public IList<Appointment> Appointments { get; set; } = new List<Appointment>();

        // Lista de estados para el modal
        public IList<AppointmentStatus> Statuses { get; set; } = new List<AppointmentStatus>();

        public async Task OnGetAsync()
        {
            Appointments = await _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Professional)
                    .ThenInclude(p => p!.Specialty)
                .Include(a => a.Specialty)
                .Include(a => a.Status)
                .OrderBy(a => a.StartAt)
                .ToListAsync();

            Statuses = await _db.AppointmentStatuses
                .OrderBy(s => s.Name)
                .ToListAsync();
        }

        // Handler para cambiar el estado desde el modal
        public async Task<IActionResult> OnPostChangeStatusAsync(int appointmentId, int statusId)
        {
            var appointment = await _db.Appointments.FindAsync(appointmentId);
            if (appointment == null) return NotFound();

            // Opcional: validar que el status exista
            var status = await _db.AppointmentStatuses.FindAsync(statusId);
            if (status == null) return BadRequest("Estado inválido.");

            appointment.StatusId = statusId;
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
