using AppointmentManager.ViewModels.Admin;

namespace AppointmentManager.Services.Availability
{
    public interface IAvailabilityService
    {
        Task<List<AvailableSlotViewModel>> GetAvailableSlotsAsync(
            int professionalId,
            DateOnly date,
            CancellationToken cancellationToken = default);

        Task<List<AvailableDateViewModel>> GetAvailableDatesAsync(
            int professionalId,
            DateOnly fromDate,
            int daysAhead = 30,
            CancellationToken cancellationToken = default);
    }
}
