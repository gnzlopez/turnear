using AppointmentManager.Data;
using AppointmentManager.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.Pages.Admin.Dashboard;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;

    public IndexModel(ApplicationDbContext db)
    {
        _db = db;
    }

    public int TodayAppointmentsCount { get; set; }

    public int NextSevenDaysAppointmentsCount { get; set; }

    public int PendingAppointmentsCount { get; set; }

    public int PatientsCount { get; set; }

    public int ProfessionalsCount { get; set; }

    public int ActiveAvailabilitiesCount { get; set; }

    public IList<Appointment> UpcomingAppointments { get; set; } = new List<Appointment>();

    public async Task OnGetAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var todayStart = today.ToDateTime(TimeOnly.MinValue);
        var tomorrowStart = today.AddDays(1).ToDateTime(TimeOnly.MinValue);
        var nextSevenDaysEnd = today.AddDays(7).ToDateTime(TimeOnly.MinValue);

        var now = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

        TodayAppointmentsCount = await _db.Appointments
            .CountAsync(a => a.StartAt >= todayStart && a.StartAt < tomorrowStart);

        NextSevenDaysAppointmentsCount = await _db.Appointments
            .CountAsync(a => a.StartAt >= todayStart && a.StartAt < nextSevenDaysEnd);

        var pendingStatusIds = await _db.AppointmentStatuses
            .Where(s => s.Name == "Pending" || s.Name == "Pendiente")
            .Select(s => s.Id)
            .ToListAsync();

        PendingAppointmentsCount = pendingStatusIds.Count == 0
            ? 0
            : await _db.Appointments.CountAsync(a => pendingStatusIds.Contains(a.StatusId));

        PatientsCount = await _db.Patients.CountAsync();

        ProfessionalsCount = await _db.Professionals.CountAsync();

        ActiveAvailabilitiesCount = await _db.ProfessionalAvailabilities
            .CountAsync(a => a.IsActive);

        UpcomingAppointments = await _db.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Professional)
                .ThenInclude(p => p!.Specialty)
            .Include(a => a.Specialty)
            .Include(a => a.Status)
            .Where(a => a.StartAt >= now)
            .OrderBy(a => a.StartAt)
            .Take(8)
            .ToListAsync();
    }
}