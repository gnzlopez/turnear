using System.Threading.Tasks;
using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppointmentManager.Pages.Admin.Specialties
{
    [Authorize(Roles = "Admin,Reception")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db) => _db = db;

        [BindProperty]
        public Specialty Specialty { get; set; } = new Specialty
        {
            IsActive = true
        };

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var s = await _db.Specialties.FindAsync(id);
            if (s == null) return NotFound();
            Specialty = s;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            _db.Specialties.Update(Specialty);
            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
