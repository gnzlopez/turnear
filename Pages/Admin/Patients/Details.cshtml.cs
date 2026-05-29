using System.Threading.Tasks;
using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppointmentManager.Pages.Admin.Patients
{
    [Authorize(Roles = "Admin,Reception")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DetailsModel(ApplicationDbContext db) => _db = db;

        public Patient Patient { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Patient = await _db.Patients.FindAsync(id);
            if (Patient == null) return NotFound();
            return Page();
        }
    }
}
