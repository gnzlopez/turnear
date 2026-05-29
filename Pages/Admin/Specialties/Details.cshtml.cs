using System.Threading.Tasks;
using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppointmentManager.Pages.Admin.Specialties
{
    [Authorize(Roles = "Admin,Reception")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DetailsModel(ApplicationDbContext db) => _db = db;

        public Specialty Specialty { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Specialty = await _db.Specialties.FindAsync(id);
            if (Specialty == null) return NotFound();
            return Page();
        }
    }
}
