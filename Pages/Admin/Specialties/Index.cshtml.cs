using System.Collections.Generic;
using System.Threading.Tasks;
using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.Pages.Admin.Specialties
{
    [Authorize(Roles = "Admin,Reception")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _db;

        public IndexModel(ApplicationDbContext db) => _db = db;

        public IList<Specialty> Specialties { get; set; } = new List<Specialty>();

        public async Task OnGetAsync()
        {
            Specialties = await _db.Specialties.AsNoTracking().OrderBy(s => s.Id).ToListAsync();
        }
    }
}
