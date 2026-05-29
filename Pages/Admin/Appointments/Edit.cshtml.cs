using AppointmentManager.Data;
using AppointmentManager.Models;
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

        public EditModel(ApplicationDbContext db) => _db = db;

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

            appointment.StatusId = Input.StatusId;
            appointment.Notes = Input.Notes;

            await _db.SaveChangesAsync();

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
