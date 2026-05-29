namespace AppointmentManager.ViewModels.Admin
{
    public class AvailableDateViewModel
    {
        public DateOnly Date { get; set; }

        public int AvailableSlotsCount { get; set; }

        public string DisplayText => Date.ToString("dd/MM/yyyy");
    }
}
