using System.Collections.Generic;
using System.Threading.Tasks;
using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.Pages.Admin.AppointmentStatuses
{
    [Authorize(Roles = "Admin,Reception")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db) => _db = db;

        public IList<AppointmentStatus> Statuses { get; set; } = new List<AppointmentStatus>();

        public async Task OnGetAsync()
        {
            Statuses = await _db.AppointmentStatuses.OrderBy(s => s.Name).ToListAsync();
        }

        // Guarda la plantilla enviada desde el modal
        public async Task<IActionResult> OnPostSaveTemplateAsync(int statusId, string? template)
        {
            var status = await _db.AppointmentStatuses.FindAsync(statusId);
            if (status == null) return NotFound();

            status.EmailTemplate = template;
            await _db.SaveChangesAsync();

            return RedirectToPage();
        }
    }
}
