using System.Collections.Generic;
using System.Threading.Tasks;
using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.Pages.Admin.AppointmentStatuses
{
    [Authorize(Roles = "Admin,Reception")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db) => _db = db;

        public IList<AppointmentStatus> Statuses { get; set; } = new List<AppointmentStatus>();

        public async Task OnGetAsync()
        {
            Statuses = await _db.AppointmentStatuses.AsNoTracking().ToListAsync();
        }
    }
}
