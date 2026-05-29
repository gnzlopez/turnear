using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.Pages.Admin.Availabilities;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public CreateModel(ApplicationDbContext db) => _db = db;

    [BindProperty]
    public ProfessionalAvailability ProfessionalAvailability { get; set; } = new()
    {
        SlotDurationMinutes = 30,
        IsActive = true
    };

    public SelectList? ProfessionalSelect { get; set; }

    public List<SelectListItem> DayOfWeekSelect { get; set; } = new();

    public async Task OnGetAsync()
    {
        await LoadSelectListsAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await LoadSelectListsAsync();

        if (ProfessionalAvailability.StartTime >= ProfessionalAvailability.EndTime)
        {
            ModelState.AddModelError(
                "ProfessionalAvailability.EndTime",
                "La hora de fin debe ser mayor que la hora de inicio.");
        }

        if (ProfessionalAvailability.SlotDurationMinutes <= 0)
        {
            ModelState.AddModelError(
                "ProfessionalAvailability.SlotDurationMinutes",
                "La duración del turno debe ser mayor a cero.");
        }

        var overlaps = await _db.ProfessionalAvailabilities
            .AnyAsync(a =>
                a.ProfessionalId == ProfessionalAvailability.ProfessionalId &&
                a.DayOfWeek == ProfessionalAvailability.DayOfWeek &&
                a.IsActive &&
                ProfessionalAvailability.IsActive &&
                ProfessionalAvailability.StartTime < a.EndTime &&
                ProfessionalAvailability.EndTime > a.StartTime);

        if (overlaps)
        {
            ModelState.AddModelError(
                string.Empty,
                "Ya existe un rango horario activo que se superpone para este profesional y día.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        _db.ProfessionalAvailabilities.Add(ProfessionalAvailability);
        await _db.SaveChangesAsync();

        return RedirectToPage("./Index");
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