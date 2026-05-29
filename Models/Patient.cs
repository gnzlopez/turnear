using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentManager.Models
{
    public class Patient
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = null!;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = null!;

        [EmailAddress, MaxLength(256)]
        public string? Email { get; set; }

        [Phone, MaxLength(50)]
        public string? Phone { get; set; }

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        public ICollection<Appointment>? Appointments { get; set; }
    }
}
