using AppointmentManager.Data;
using AppointmentManager.Models;
using AppointmentManager.Services.Availability;
using AppointmentManager.Services.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace AppointmentManager.Pages.Admin.Appointments
{
    [Authorize(Roles = "Admin,Reception")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmailQueue _emailQueue;

        public EditModel(ApplicationDbContext db, IEmailQueue emailQueue)
        {
            _db = db;
            _emailQueue = emailQueue;
        }

        [BindProperty]
        public AppointmentEditInputModel Input { get; set; } = new();

        public Appointment? AppointmentDetails { get; set; }

        public SelectList? StatusSelect { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id is null)
            {
                return NotFound();
            }

            AppointmentDetails = await GetAppointmentDetailsAsync(id.Value);

            if (AppointmentDetails is null)
            {
                return NotFound();
            }

            Input = new AppointmentEditInputModel
            {
                Id = AppointmentDetails.Id,
                StatusId = AppointmentDetails.StatusId,
                Notes = AppointmentDetails.Notes
            };

            await LoadStatusSelectAsync(Input.StatusId);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadStatusSelectAsync(Input.StatusId);

            if (!ModelState.IsValid)
            {
                AppointmentDetails = await GetAppointmentDetailsAsync(Input.Id);
                return Page();
            }

            var appointment = await _db.Appointments
                .FirstOrDefaultAsync(a => a.Id == Input.Id);

            if (appointment is null)
            {
                return NotFound();
            }

            var status = await _db.AppointmentStatuses
                .FirstOrDefaultAsync(s => s.Id == Input.StatusId);

            if (status is null)
            {
                ModelState.AddModelError("Input.StatusId", "El estado seleccionado no existe.");
                return Page();
            }

            appointment.StatusId = Input.StatusId;
            appointment.Notes = Input.Notes;

            await _db.SaveChangesAsync();

            if (status.SendMail && !string.IsNullOrWhiteSpace(appointment.Patient?.Email))
            {
                var to = appointment.Patient!.Email!;
                var subject = $"Cambio de estado de su turno - {status.Name}";

                // Si EmailTemplate está definido, usarlo (reemplazos simples)
                var template = status.EmailTemplate ?? "<p>Su turno cambió de estado a {Status}.</p>";
                var body = template
                    .Replace("{Nombre}", appointment.Patient!.FirstName)
                    .Replace("{Apellido}", appointment.Patient!.LastName)
                    .Replace("{Dia}", appointment.StartAt.ToString("dd/MM/yyyy"))
                    .Replace("{Hora}", appointment.StartAt.ToString("HH:mm"))
                    .Replace("{Profesional}", appointment.Professional!.FullName)
                    .Replace("{Estado}", status.Name);

                var msg = new EmailMessage { To = to, Subject = subject, Body = body };
                _emailQueue.Enqueue(msg);
            }

            return RedirectToPage("./Index");
        }

        private async Task<Appointment?> GetAppointmentDetailsAsync(int id)
        {
            return await _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Professional)
                    .ThenInclude(p => p!.Specialty)
                .Include(a => a.Specialty)
                .Include(a => a.Status)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        private async Task LoadStatusSelectAsync(int? selectedStatusId = null)
        {
            var statuses = await _db.AppointmentStatuses
                .OrderBy(s => s.Id)
                .Select(s => new
                {
                    s.Id,
                    s.Name
                })
                .ToListAsync();

            StatusSelect = new SelectList(statuses, "Id", "Name", selectedStatusId);
        }

        public class AppointmentEditInputModel
        {
            public int Id { get; set; }

            [Required(ErrorMessage = "Debe seleccionar un estado.")]
            public int StatusId { get; set; }

            [MaxLength(1000, ErrorMessage = "Las notas no pueden superar los 1000 caracteres.")]
            public string? Notes { get; set; }
        }
    }
}
