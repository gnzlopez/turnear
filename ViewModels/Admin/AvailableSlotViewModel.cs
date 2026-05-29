namespace AppointmentManager.ViewModels.Admin
{
    public class AvailableSlotViewModel
    {
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string DisplayText => $"{StartAt:HH:mm} - {EndAt:HH:mm}";
    }
}
