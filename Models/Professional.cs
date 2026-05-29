using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentManager.Models
{
    public class Professional
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = null!;

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [EmailAddress, MaxLength(256)]
        public string? Email { get; set; }

        [Phone, MaxLength(50)]
        public string? Phone { get; set; }

        public int? SpecialtyId { get; set; }

        public Specialty? Specialty { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<ProfessionalAvailability> Availabilities { get; set; } = new List<ProfessionalAvailability>();

        public ICollection<Appointment>? Appointments { get; set; }
    }
}
