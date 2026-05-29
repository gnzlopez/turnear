using System.Collections.Generic;
using System.Threading.Tasks;
using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.Pages.Admin.Appointments
{
    [Authorize(Roles = "Admin,Reception")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db) => _db = db;

        public IList<Appointment> Appointments { get; set; } = new List<Appointment>();

        public async Task OnGetAsync()
        {
            Appointments = await _db.Appointments
                .Include(a => a.Patient)
                .Include(a => a.Professional)
                    .ThenInclude(p => p!.Specialty)
                .Include(a => a.Specialty)
                .Include(a => a.Status)
                .OrderBy(a => a.StartAt)
                .ToListAsync();
        }
    }
}
