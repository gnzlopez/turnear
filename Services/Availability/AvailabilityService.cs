using AppointmentManager.Data;
using AppointmentManager.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManager.Services.Availability
{
    public class AvailabilityService: IAvailabilityService
    {
        private readonly ApplicationDbContext _context;

        public AvailabilityService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AvailableSlotViewModel>> GetAvailableSlotsAsync(
            int professionalId,
            DateOnly date,
            CancellationToken cancellationToken = default)
        {
            var dayOfWeek = date.DayOfWeek;

            var availabilities = await _context.ProfessionalAvailabilities
                .Where(a =>
                    a.ProfessionalId == professionalId &&
                    a.DayOfWeek == dayOfWeek &&
                    a.IsActive)
                .OrderBy(a => a.StartTime)
                .ToListAsync(cancellationToken);

            if (!availabilities.Any())
            {
                return [];
            }

            var dayStart = date.ToDateTime(TimeOnly.MinValue);
            var nextDayStart = date.AddDays(1).ToDateTime(TimeOnly.MinValue);

            var busyAppointments = await _context.Appointments
                .Where(a =>
                    a.ProfessionalId == professionalId &&
                    a.StartAt >= dayStart &&
                    a.StartAt < nextDayStart &&
                    a.StatusId != 3 && // Cancelled
                    a.StatusId != 4)   // Rejected
                .Select(a => new
                {
                    a.StartAt,
                    a.EndAt
                })
                .ToListAsync(cancellationToken);

            var availableSlots = new List<AvailableSlotViewModel>();

            foreach (var availability in availabilities)
            {
                var currentStart = date.ToDateTime(availability.StartTime);
                var availabilityEnd = date.ToDateTime(availability.EndTime);

                while (currentStart.AddMinutes(availability.SlotDurationMinutes) <= availabilityEnd)
                {
                    var currentEnd = currentStart.AddMinutes(availability.SlotDurationMinutes);

                    var isBusy = busyAppointments.Any(a =>
                        currentStart < a.EndAt &&
                        currentEnd > a.StartAt);

                    if (!isBusy)
                    {
                        availableSlots.Add(new AvailableSlotViewModel
                        {
                            StartAt = currentStart,
                            EndAt = currentEnd
                        });
                    }

                    currentStart = currentEnd;
                }
            }

            return availableSlots
                .OrderBy(s => s.StartAt)
                .ToList();
        }

        public async Task<List<AvailableDateViewModel>> GetAvailableDatesAsync(
            int professionalId,
            DateOnly fromDate,
            int daysAhead = 30,
            CancellationToken cancellationToken = default)
        {
            var availableDates = new List<AvailableDateViewModel>();

            for (var i = 0; i <= daysAhead; i++)
            {
                var date = fromDate.AddDays(i);

                var slots = await GetAvailableSlotsAsync(
                    professionalId,
                    date,
                    cancellationToken);

                if (slots.Any())
                {
                    availableDates.Add(new AvailableDateViewModel
                    {
                        Date = date,
                        AvailableSlotsCount = slots.Count
                    });
                }
            }

            return availableDates;
        }
    }
}
