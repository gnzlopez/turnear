using System.ComponentModel.DataAnnotations;

namespace AppointmentManager.Models
{
    public class Specialty
    {
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Name { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Professional>? Professionals { get; set; }
        public ICollection<Appointment>? Appointments { get; set; }
    }
}
