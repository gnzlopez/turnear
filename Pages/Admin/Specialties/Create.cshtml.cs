using System.Threading.Tasks;
using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppointmentManager.Pages.Admin.Specialties
{
    [Authorize(Roles = "Admin,Reception")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public CreateModel(ApplicationDbContext db) => _db = db;

        [BindProperty]
        public Specialty Specialty { get; set; } = new Specialty
        {
            IsActive = true
        };

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();
            _db.Specialties.Add(Specialty);
            await _db.SaveChangesAsync();
            return RedirectToPage("Index");
        }
    }
}
