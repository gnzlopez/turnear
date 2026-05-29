using AppointmentManager.ViewModels.Admin;

namespace AppointmentManager.Services.Availability
{
    public interface IAvailabilityService
    {
        Task<List<AvailableSlotViewModel>> GetAvailableSlotsAsync(
            int professionalId,
            DateOnly date,
            CancellationToken cancellationToken = default);
    }
}
