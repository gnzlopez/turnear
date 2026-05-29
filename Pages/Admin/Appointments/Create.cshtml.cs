using System.ComponentModel.DataAnnotations;
using System.Globalization;
using AppointmentManager.Data;
using AppointmentManager.Models;
using AppointmentManager.Services.Availability;
using AppointmentManager.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.Pages.Admin.Appointments;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly IAvailabilityService _availabilityService;

    public CreateModel(ApplicationDbContext context, IAvailabilityService availabilityService)
    {
        _db = context;
        _availabilityService = availabilityService;
    }

    [BindProperty]
    public AppointmentCreateInputModel Input { get; set; } = new();

    public SelectList? PatientSelect { get; set; }
    public SelectList? ProfessionalSelect { get; set; }
    public SelectList? StatusSelect { get; set; }

    public List<AvailableDateViewModel> AvailableDates { get; set; } = new();
    public List<AvailableSlotViewModel> AvailableSlots { get; set; } = new();

    public bool DatesWereLoaded { get; set; }
    public bool SlotsWereLoaded { get; set; }

    public async Task OnGetAsync()
    {
        await LoadSelectListsAsync();
    }

    public async Task<IActionResult> OnPostSearchDatesAsync()
    {
        await LoadSelectListsAsync();

        if (Input.ProfessionalId is null)
        {
            ModelState.AddModelError("Input.ProfessionalId", "Debe seleccionar un profesional.");
        }

        if (!ModelState.IsValid)
        {
            return Page();
        }

        DatesWereLoaded = true;

        AvailableDates = await _availabilityService.GetAvailableDatesAsync(
            Input.ProfessionalId.Value,
            DateOnly.FromDateTime(DateTime.Today));

        return Page();
    }

    public async Task<IActionResult> OnPostSearchSlotsAsync()
    {
        await LoadSelectListsAsync();

        DatesWereLoaded = true;
        SlotsWereLoaded = true;

        await LoadAvailableDatesAsync();

        ValidateProfessionalAndDate();

        if (!ModelState.IsValid)
        {
            return Page();
        }

        var dateIsAvailable = AvailableDates.Any(d => d.Date == Input.Date!.Value);

        if (!dateIsAvailable)
        {
            ModelState.AddModelError("Input.Date", "La fecha seleccionada no tiene horarios disponibles.");
            return Page();
        }

        AvailableSlots = await _availabilityService.GetAvailableSlotsAsync(
            Input.ProfessionalId!.Value,
            Input.Date!.Value);

        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        await LoadSelectListsAsync();

        DatesWereLoaded = true;
        SlotsWereLoaded = true;

        await LoadAvailableDatesAsync();

        if (Input.ProfessionalId.HasValue && Input.Date.HasValue)
        {
            AvailableSlots = await _availabilityService.GetAvailableSlotsAsync(
                Input.ProfessionalId.Value,
                Input.Date.Value);
        }

        ValidateProfessionalAndDate();
        ValidateCreateData();

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

        var professional = await _db.Professionals
            .Include(p => p.Specialty)
            .FirstOrDefaultAsync(p =>
                p.Id == Input.ProfessionalId!.Value &&
                p.IsActive);

        if (professional is null)
        {
            ModelState.AddModelError("Input.ProfessionalId", "El profesional seleccionado no está disponible.");
            return Page();
        }

        var patientExists = await _db.Patients
            .AnyAsync(p => p.Id == Input.PatientId!.Value);

        if (!patientExists)
        {
            ModelState.AddModelError("Input.PatientId", "El paciente seleccionado no existe.");
            return Page();
        }

        var statusExists = await _db.AppointmentStatuses
            .AnyAsync(s => s.Id == Input.StatusId!.Value);

        if (!statusExists)
        {
            ModelState.AddModelError("Input.StatusId", "El estado seleccionado no existe.");
            return Page();
        }

        var appointment = new Appointment
        {
            PatientId = Input.PatientId.Value,
            ProfessionalId = professional.Id,
            SpecialtyId = professional.SpecialtyId,
            StatusId = Input.StatusId.Value,
            StartAt = startAt,
            EndAt = endAt,
            Notes = Input.Notes,
            PublicAccessToken = Guid.NewGuid().ToString("N"),
            CreatedAt = DateTime.UtcNow
        };

        _db.Appointments.Add(appointment);
        await _db.SaveChangesAsync();

        return RedirectToPage("./Index");
    }

    private async Task LoadSelectListsAsync()
    {
        var patients = await _db.Patients
            .AsNoTracking()
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Select(p => new
            {
                p.Id,
                Name = p.LastName + ", " + p.FirstName
            })
            .ToListAsync();

        PatientSelect = new SelectList(patients, "Id", "Name", Input.PatientId);

        var professionals = await _db.Professionals
            .AsNoTracking()
            .Include(p => p.Specialty)
            .Where(p => p.IsActive)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Select(p => new
            {
                p.Id,
                Name = p.LastName + ", " + p.FirstName +
                       (p.Specialty != null ? " - " + p.Specialty.Name : "")
            })
            .ToListAsync();

        ProfessionalSelect = new SelectList(professionals, "Id", "Name", Input.ProfessionalId);

        var statuses = await _db.AppointmentStatuses
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .Select(s => new
            {
                s.Id,
                s.Name
            })
            .ToListAsync();

        StatusSelect = new SelectList(statuses, "Id", "Name", Input.StatusId);
    }

    private async Task LoadAvailableDatesAsync()
    {
        if (Input.ProfessionalId is null)
        {
            AvailableDates = new List<AvailableDateViewModel>();
            return;
        }

        AvailableDates = await _availabilityService.GetAvailableDatesAsync(
            Input.ProfessionalId.Value,
            DateOnly.FromDateTime(DateTime.Today));
    }

    private void ValidateProfessionalAndDate()
    {
        if (Input.ProfessionalId is null)
        {
            ModelState.AddModelError("Input.ProfessionalId", "Debe seleccionar un profesional.");
        }

        if (Input.Date is null)
        {
            ModelState.AddModelError("Input.Date", "Debe seleccionar una fecha.");
        }

        if (Input.Date.HasValue && Input.Date.Value < DateOnly.FromDateTime(DateTime.Today))
        {
            ModelState.AddModelError("Input.Date", "La fecha no puede ser anterior a hoy.");
        }
    }

    private void ValidateCreateData()
    {
        if (Input.PatientId is null)
        {
            ModelState.AddModelError("Input.PatientId", "Debe seleccionar un paciente.");
        }

        if (Input.StatusId is null)
        {
            ModelState.AddModelError("Input.StatusId", "Debe seleccionar un estado.");
        }

        if (string.IsNullOrWhiteSpace(Input.SelectedSlotValue))
        {
            ModelState.AddModelError("Input.SelectedSlotValue", "Debe seleccionar un horario.");
        }
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

    public class AppointmentCreateInputModel
    {
        public int? PatientId { get; set; }

        public int? ProfessionalId { get; set; }

        public DateOnly? Date { get; set; }

        public string? SelectedSlotValue { get; set; }

        public int? StatusId { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }
}