using AppointmentManager.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.Pages.Public.Professionals;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext context)
    {
        _db = context;
    }

    [BindProperty(SupportsGet = true)]
    public int? SpecialtyId { get; set; }

    public IList<ProfessionalListItem> Professionals { get; set; } = new List<ProfessionalListItem>();

    public IList<SpecialtyFilterItem> Specialties { get; set; } = new List<SpecialtyFilterItem>();

    public async Task OnGetAsync()
    {
        Specialties = await _db.Specialties
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .Select(s => new SpecialtyFilterItem
            {
                Id = s.Id,
                Name = s.Name
            })
            .ToListAsync();

        var query = _db.Professionals
            .AsNoTracking()
            .Include(p => p.Specialty)
            .Where(p => p.IsActive);

        if (SpecialtyId.HasValue)
        {
            query = query.Where(p => p.SpecialtyId == SpecialtyId.Value);
        }

        Professionals = await query
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Select(p => new ProfessionalListItem
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                Phone = p.Phone,
                SpecialtyName = p.Specialty != null ? p.Specialty.Name : null,
                SpecialtyIsActive = p.Specialty == null || p.Specialty.IsActive
            })
            .Where(p => p.SpecialtyIsActive)
            .ToListAsync();
    }

    public class ProfessionalListItem
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{FirstName} {LastName}";

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public string? SpecialtyName { get; set; }

        public bool SpecialtyIsActive { get; set; }
    }

    public class SpecialtyFilterItem
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}