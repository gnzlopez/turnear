using System.Threading.Tasks;
using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppointmentManager.Pages.Admin.Patients
{
    [Authorize(Roles = "Admin,Reception")]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DeleteModel(ApplicationDbContext db) => _db = db;

        [BindProperty]
        public Patient Patient { get; set; } = new Patient();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var p = await _db.Patients.FindAsync(id);
            if (p == null) return NotFound();
            Patient = p;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var p = await _db.Patients.FindAsync(Patient.Id);
            if (p != null)
            {
                _db.Patients.Remove(p);
                await _db.SaveChangesAsync();
            }
            return RedirectToPage("Index");
        }
    }
}
