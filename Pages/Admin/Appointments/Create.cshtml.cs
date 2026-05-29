using AppointmentManager.Data;
using AppointmentManager.Models;
using AppointmentManager.Services.Availability;
using AppointmentManager.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AppointmentManager.Pages.Admin.Appointments;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly IAvailabilityService _availabilityService;

    public CreateModel(ApplicationDbContext db, IAvailabilityService availabilityService)
    {
        _db = db;
        _availabilityService = availabilityService;
    }

    [BindProperty]
    public AppointmentCreateViewModel Input { get; set; } = new();

    public SelectList? PatientSelect { get; set; }
    public SelectList? ProfessionalSelect { get; set; }
    public SelectList? StatusSelect { get; set; }

    public List<AvailableSlotViewModel> AvailableSlots { get; set; } = new();

    public bool SearchWasPerformed { get; set; }

    public async Task OnGetAsync()
    {
        Input.Date = DateOnly.FromDateTime(DateTime.Today);
        Input.StatusId = 1; // Pending

        await LoadSelectListsAsync();
    }

    public async Task<IActionResult> OnPostSearchSlotsAsync()
    {
        await LoadSelectListsAsync();

        SearchWasPerformed = true;

        // Estos campos son necesarios recién al crear el turno, no al buscar horarios.
        ModelState.Remove("Input.PatientId");
        ModelState.Remove("Input.StatusId");
        ModelState.Remove("Input.SelectedSlotValue");
        ModelState.Remove("Input.Notes");

        if (!ModelState.IsValid)
        {
            return Page();
        }

        if (Input.ProfessionalId is null || Input.Date is null)
        {
            return Page();
        }

        AvailableSlots = await _availabilityService.GetAvailableSlotsAsync(
            Input.ProfessionalId.Value,
            Input.Date.Value);

        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        await LoadSelectListsAsync();

        SearchWasPerformed = true;

        if (Input.ProfessionalId is not null && Input.Date is not null)
        {
            AvailableSlots = await _availabilityService.GetAvailableSlotsAsync(
                Input.ProfessionalId.Value,
                Input.Date.Value);
        }

        if (string.IsNullOrWhiteSpace(Input.SelectedSlotValue))
        {
            ModelState.AddModelError("Input.SelectedSlotValue", "Debe seleccionar un horario.");
        }

        if (!TryParseSelectedSlot(Input.SelectedSlotValue, out var startAt, out var endAt))
        {
            ModelState.AddModelError("Input.SelectedSlotValue", "El horario seleccionado no es válido.");
        }

        var slotStillAvailable = AvailableSlots.Any(s =>
            s.StartAt == startAt &&
            s.EndAt == endAt);

        if (!slotStillAvailable)
        {
            ModelState.AddModelError("Input.SelectedSlotValue", "El horario seleccionado ya no está disponible.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var appointment = new Appointment
        {
            PatientId = Input.PatientId!.Value,
            ProfessionalId = Input.ProfessionalId!.Value,
            StatusId = Input.StatusId,
            StartAt = startAt,
            EndAt = endAt,
            Notes = Input.Notes,
            CreatedAt = DateTime.UtcNow
        };

        _db.Appointments.Add(appointment);
        await _db.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    private async Task LoadSelectListsAsync()
    {
        var patients = await _db.Patients
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Select(p => new
            {
                p.Id,
                Name = p.LastName + ", " + p.FirstName
            })
            .ToListAsync();

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

        var statuses = await _db.AppointmentStatuses
            .OrderBy(s => s.Id)
            .Select(s => new
            {
                s.Id,
                s.Name
            })
            .ToListAsync();

        PatientSelect = new SelectList(patients, "Id", "Name");
        ProfessionalSelect = new SelectList(professionals, "Id", "Name");
        StatusSelect = new SelectList(statuses, "Id", "Name");
    }

    private static bool TryParseSelectedSlot(
        string? selectedSlotValue,
        out DateTime startAt,
        out DateTime endAt)
    {
        startAt = default;
        endAt = default;

        if (string.IsNullOrWhiteSpace(selectedSlotValue))
        {
            return false;
        }

        var parts = selectedSlotValue.Split('|');

        if (parts.Length != 2)
        {
            return false;
        }

        var startIsValid = DateTime.TryParse(
            parts[0],
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out startAt);

        var endIsValid = DateTime.TryParse(
            parts[1],
            CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind,
            out endAt);

        return startIsValid && endIsValid && startAt < endAt;
    }
}