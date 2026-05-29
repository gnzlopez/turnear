using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.Pages.Admin.AppointmentStatuses
{
    [Authorize(Roles = "Admin,Reception")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DeleteModel(ApplicationDbContext db) => _db = db;

        public AppointmentStatus Status { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Status = await _db.AppointmentStatuses.FindAsync(id);
            if (Status == null) return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            var status = await _db.AppointmentStatuses.FindAsync(id);
            if (status != null)
            {
                _db.AppointmentStatuses.Remove(status);
                await _db.SaveChangesAsync();
            }
            return RedirectToPage("Index");
        }
    }
}
