using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AppointmentManager.Pages.Admin.Professionals
{
    [Authorize(Roles = "Admin,Reception")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DeleteModel(ApplicationDbContext db) => _db = db;

        [BindProperty]
        public Professional Professional { get; set; } = new Professional();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var p = await _db.Professionals.Include(p => p.Specialty).FirstOrDefaultAsync(p => p.Id == id);
            if (p == null) return NotFound();
            Professional = p;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var p = await _db.Professionals.FindAsync(Professional.Id);
            if (p != null)
            {
                _db.Professionals.Remove(p);
                await _db.SaveChangesAsync();
            }
            return RedirectToPage("Index");
        }
    }
}
