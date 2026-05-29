using System.Collections.Generic;
using System.Threading.Tasks;
using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.Pages.Admin.Professionals
{
    [Authorize(Roles = "Admin,Reception")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db) => _db = db;

        public IList<Professional> Professionals { get; set; } = new List<Professional>();

        public async Task OnGetAsync()
        {
            Professionals = await _db.Professionals
                            .Include(p => p.Specialty)
                            .OrderBy(p => p.LastName)
                            .ThenBy(p => p.FirstName)
                            .ToListAsync();
        }
    }
}
