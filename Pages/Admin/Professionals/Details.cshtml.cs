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
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DetailsModel(ApplicationDbContext db) => _db = db;

        public Professional Professional { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Professional = await _db.Professionals.Include(p => p.Specialty).FirstOrDefaultAsync(p => p.Id == id);
            if (Professional == null) return NotFound();
            return Page();
        }
    }
}
