using System.Threading.Tasks;
using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.Pages.Admin.Professionals
{
    [Authorize(Roles = "Admin,Reception")]
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public EditModel(ApplicationDbContext db) => _db = db;

        [BindProperty]
        public Professional Professional { get; set; } = new Professional
        {
            IsActive = true
        };

        public SelectList? SpecialtySelect { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var p = await _db.Professionals.FindAsync(id);
            if (p == null) return NotFound();
            Professional = p;
            SpecialtySelect = new SelectList(await _db.Specialties.AsNoTracking().ToListAsync(), "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                SpecialtySelect = new SelectList(await _db.Specialties.AsNoTracking().ToListAsync(), "Id", "Name");
                return Page();
            }

            _db.Professionals.Update(Professional);
            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
