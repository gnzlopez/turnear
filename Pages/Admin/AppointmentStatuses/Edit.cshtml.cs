using System.Threading.Tasks;
using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppointmentManager.Pages.Admin.AppointmentStatuses
{
    [Authorize(Roles = "Admin,Reception")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db) => _db = db;

        [BindProperty]
        public AppointmentStatus Status { get; set; } = new AppointmentStatus();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var s = await _db.AppointmentStatuses.FindAsync(id);
            if (s == null) return NotFound();
            Status = s;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            _db.AppointmentStatuses.Update(Status);
            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
