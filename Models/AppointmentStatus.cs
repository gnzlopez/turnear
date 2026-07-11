using System.ComponentModel.DataAnnotations;

namespace AppointmentManager.Models
{
    public class AppointmentStatus
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        public bool SendMail { get; set; } = false;

        public ICollection<Appointment>? Appointments { get; set; }
    }
}
