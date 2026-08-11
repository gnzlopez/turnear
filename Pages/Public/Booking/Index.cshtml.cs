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

namespace AppointmentManager.Pages.Public.Booking;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly IAvailabilityService _availabilityService;

    public IndexModel(ApplicationDbContext db, IAvailabilityService availabilityService)
    {
        _db = db;
        _availabilityService = availabilityService;
    }

    [BindProperty]
    public BookingInputModel Input { get; set; } = new();

    [BindProperty(SupportsGet = true, Name = "professionalId")]
    public int? PreselectedProfessionalId { get; set; }

    public SelectList? ProfessionalSelect { get; set; }

    public List<AvailableDateViewModel> AvailableDates { get; set; } = new();

    public List<AvailableSlotViewModel> AvailableSlots { get; set; } = new();

    public bool DatesWereLoaded { get; set; }

    public bool SlotsWereLoaded { get; set; }

    public async Task OnGetAsync()
    {
        if (PreselectedProfessionalId.HasValue)
        {
            Input.ProfessionalId = PreselectedProfessionalId.Value;
            DatesWereLoaded = true;

            AvailableDates = await _availabilityService.GetAvailableDatesAsync(
                Input.ProfessionalId.Value,
                DateOnly.FromDateTime(DateTime.Today));
        }

        await LoadSelectListsAsync();
    }

    public async Task<IActionResult> OnPostSearchDatesAsync()
    {
        RemovePatientAndSlotValidation();

        await LoadSelectListsAsync();

        if (Input.ProfessionalId is null)
        {
            ModelState.AddModelError("Input.ProfessionalId", "Debe seleccionar un profesional.");
        }

        //if (!ModelState.IsValid)
        //{
        //    return Page();
        //}

        DatesWereLoaded = true;

        AvailableDates = await _availabilityService.GetAvailableDatesAsync(
            Input.ProfessionalId.Value,
            DateOnly.FromDateTime(DateTime.Today));

        return Page();
    }

    public async Task<IActionResult> OnPostSearchSlotsAsync()
    {
        RemovePatientAndSlotValidation();

        await LoadSelectListsAsync();

        DatesWereLoaded = true;
        SlotsWereLoaded = true;

        await LoadAvailableDatesAsync();

        ValidateSearchData();

        //if (!ModelState.IsValid)
        //{
        //    return Page();
        //}

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

        ValidateSearchData();

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

        var professional = await _db.Professionals
            .Include(p => p.Specialty)
            .FirstOrDefaultAsync(p =>
                p.Id == Input.ProfessionalId!.Value &&
                p.IsActive &&
                (p.Specialty == null || p.Specialty.IsActive));

        if (professional is null)
        {
            ModelState.AddModelError("Input.ProfessionalId", "El profesional seleccionado no está disponible.");
            return Page();
        }

        var pendingStatusId = await GetPendingStatusIdAsync();

        if (pendingStatusId is null)
        {
            ModelState.AddModelError(string.Empty, "No existe un estado configurado para crear el turno. Por favor, pongase en contacto con la administración.");
            return Page();
        }

        var patient = await FindOrCreatePatientAsync();

        var appointment = new Appointment
        {
            Patient = patient,
            ProfessionalId = professional.Id,
            SpecialtyId = professional.SpecialtyId,
            StatusId = pendingStatusId.Value,
            StartAt = startAt,
            EndAt = endAt,
            Notes = Input.Notes,
            PublicAccessToken = Guid.NewGuid().ToString("N"),
            CreatedAt = DateTime.UtcNow
        };

        _db.Appointments.Add(appointment);
        await _db.SaveChangesAsync();

        return RedirectToPage("./Success");
    }

    private async Task LoadSelectListsAsync()
    {
        var professionals = await _db.Professionals
            .AsNoTracking()
            .Include(p => p.Specialty)
            .Where(p => p.IsActive && (p.Specialty == null || p.Specialty.IsActive))
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

    private void ValidateSearchData()
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

    private void RemovePatientAndSlotValidation()
    {
        ModelState.Remove("Input.SelectedSlotValue");
        ModelState.Remove("Input.PatientFirstName");
        ModelState.Remove("Input.PatientLastName");
        ModelState.Remove("Input.Phone");
        ModelState.Remove("Input.Email");
        ModelState.Remove("Input.Notes");
    }

    private async Task<int?> GetPendingStatusIdAsync()
    {
        return await _db.AppointmentStatuses
            .Where(s =>
                s.Name == "Pending" ||
                s.Name == "Pendiente")
            .Select(s => (int?)s.Id)
            .FirstOrDefaultAsync();
    }

    private async Task<Patient> FindOrCreatePatientAsync()
    {
        var firstName = Input.PatientFirstName.Trim();
        var lastName = Input.PatientLastName.Trim();
        var phone = Input.Phone.Trim();
        var DNI = Input.Dni.Trim();

        var email = string.IsNullOrWhiteSpace(Input.Email)
            ? null
            : Input.Email.Trim();

        var patient = await _db.Patients
            .FirstOrDefaultAsync(p =>
                p.Dni == DNI);

        if (patient is not null)
        {
            patient.FirstName = firstName;
            patient.LastName = lastName;
            patient.Phone = phone;

            if (!string.IsNullOrWhiteSpace(email))
            {
                patient.Email = email;
            }

            return patient;
        }

        patient = new Patient
        {
            FirstName = firstName,
            LastName = lastName,
            Phone = phone,
            Email = email,
            Dni = DNI
        };

        // Cuando la creación viene desde el flujo público, marcar siempre como Externo
        patient.PatientType = PatientType.External;

        _db.Patients.Add(patient);

        return patient;
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

    public class BookingInputModel
    {
        public int? ProfessionalId { get; set; }

        public DateOnly? Date { get; set; }

        public string? SelectedSlotValue { get; set; }

        [Required(ErrorMessage = "Debe ingresar el nombre.")]
        [MaxLength(100)]
        public string PatientFirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar el apellido.")]
        [MaxLength(100)]
        public string PatientLastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe ingresar un teléfono.")]
        [Phone(ErrorMessage = "El teléfono no tiene un formato válido.")]
        [MaxLength(50)]
        public string Phone { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        [MaxLength(256)]
        public string? Email { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [Required(ErrorMessage = "Debe ingresar el DNI.")]
        public string Dni { get; set; } = string.Empty;
    }
}