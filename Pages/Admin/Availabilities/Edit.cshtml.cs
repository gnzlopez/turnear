using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.Pages.Admin.Availabilities;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public EditModel(ApplicationDbContext context)
    {
        _db = context;
    }

    public SelectList? ProfessionalSelect { get; set; }

    public List<SelectListItem> DayOfWeekSelect { get; set; } = new();

    [BindProperty]
    public ProfessionalAvailability ProfessionalAvailability { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        await LoadSelectListsAsync();

        if (id is null)
        {
            return NotFound();
        }

        var professionalavailability = await _db.ProfessionalAvailabilities
                                                    .Include(p => p.Professional)
                                                    .ThenInclude(p => p.Specialty)
                                                    .FirstOrDefaultAsync(m => m.Id == id);

        if (professionalavailability is null)
        {
            return NotFound();
        }
        ProfessionalAvailability = professionalavailability;
        return Page();
    }

    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _db.Attach(ProfessionalAvailability).State = EntityState.Modified;

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ProfessionalAvailabilityExists(ProfessionalAvailability.Id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return RedirectToPage("./Index");
    }

    private bool ProfessionalAvailabilityExists(int id)
    {
        return _db.ProfessionalAvailabilities.Any(e => e.Id == id);
    }

    private async Task LoadSelectListsAsync()
    {
        var professionals = await _db.Professionals
            .Include(p => p.Specialty)
            //.Where(p => p.IsActive)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Select(p => new
            {
                p.Id,
                Name = p.LastName + ", " + p.FirstName + " - " + p.Specialty.Name
            })
            .ToListAsync();

        ProfessionalSelect = new SelectList(professionals, "Id", "Name");

        DayOfWeekSelect =
        [
            new SelectListItem("Lunes", DayOfWeek.Monday.ToString()),
            new SelectListItem("Martes", DayOfWeek.Tuesday.ToString()),
            new SelectListItem("Miércoles", DayOfWeek.Wednesday.ToString()),
            new SelectListItem("Jueves", DayOfWeek.Thursday.ToString()),
            new SelectListItem("Viernes", DayOfWeek.Friday.ToString()),
            new SelectListItem("Sábado", DayOfWeek.Saturday.ToString()),
            new SelectListItem("Domingo", DayOfWeek.Sunday.ToString())
        ];
    }
}
