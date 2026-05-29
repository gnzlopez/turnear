using AppointmentManager.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.Pages.Public.Specialties;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext context)
    {
        _db = context;
    }

    public IList<SpecialtyListItem> Specialties { get; set; } = new List<SpecialtyListItem>();

    public async Task OnGetAsync()
    {
        Specialties = await _db.Specialties
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .Select(s => new SpecialtyListItem
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                ActiveProfessionalsCount = _db.Professionals
                    .Count(p => p.IsActive && p.SpecialtyId == s.Id)
            })
            .ToListAsync();
    }

    public class SpecialtyListItem
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int ActiveProfessionalsCount { get; set; }
    }
}