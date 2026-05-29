using System.Threading.Tasks;
using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppointmentManager.Pages.Admin.AppointmentStatuses
{
    [Authorize(Roles = "Admin,Reception")]
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public DetailsModel(ApplicationDbContext db) => _db = db;

        public AppointmentStatus Status { get; set; } = null!;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Status = await _db.AppointmentStatuses.FindAsync(id);
            if (Status == null) return NotFound();
            return Page();
        }
    }
}
