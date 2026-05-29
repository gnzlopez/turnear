using System.Threading.Tasks;
using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppointmentManager.Pages.Admin.Patients
{
    [Authorize(Roles = "Admin,Reception")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public CreateModel(ApplicationDbContext db) => _db = db;

        [BindProperty]
        public Patient Patient { get; set; } = new Patient();

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            _db.Patients.Add(Patient);
            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
