using System.Collections.Generic;
using System.Threading.Tasks;
using AppointmentManager.Data;
using AppointmentManager.Models;
using AppointmentManager.Services.Email;
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
        private readonly IEmailQueue _emailQueue;

        public IndexModel(ApplicationDbContext db, IEmailQueue emailQueue)
        {
            _db = db;
            _emailQueue = emailQueue;
        }

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
            var appointment = await _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Status)
                .Include(a => a.Professional)
                .FirstOrDefaultAsync(a => a.Id == appointmentId);

            if (appointment == null) return NotFound();

            var newStatus = await _db.AppointmentStatuses.FindAsync(statusId);
            if (newStatus == null) return BadRequest("Estado inválido.");

            appointment.StatusId = statusId;
            await _db.SaveChangesAsync();

            if (newStatus.SendMail && !string.IsNullOrWhiteSpace(appointment.Patient?.Email))
            {
                var to = appointment.Patient!.Email!;
                var subject = $"Cambio de estado de su turno - {newStatus.Name}";

                // Si EmailTemplate está definido, usarlo (reemplazos simples)
                var template = newStatus.EmailTemplate ?? "<p>Su turno cambió de estado a {Status}.</p>";
                var body = template
                    .Replace("{Nombre}", appointment.Patient!.FirstName)
                    .Replace("{Apellido}", appointment.Patient!.LastName)
                    .Replace("{Dia}", appointment.StartAt.ToString("dd/MM/yyyy"))
                    .Replace("{Hora}", appointment.StartAt.ToString("HH:mm"))
                    .Replace("{Profesional}", appointment.Professional!.FullName)
                    .Replace("{Estado}", newStatus.Name);

                var msg = new EmailMessage { To = to, Subject = subject, Body = body };
                _emailQueue.Enqueue(msg);
            }

            return RedirectToPage();
        }
    }
}
