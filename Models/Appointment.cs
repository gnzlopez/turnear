using System;
using System.ComponentModel.DataAnnotations;

namespace AppointmentManager.Models
{
    public class Appointment
    {
        public int Id { get; set; }

        [Required]
        public DateTime StartAt { get; set; }

        [Required]
        public DateTime EndAt { get; set; }

        public int PatientId { get; set; }
        public Patient? Patient { get; set; }

        public int ProfessionalId { get; set; }
        public Professional? Professional { get; set; }

        public int? SpecialtyId { get; set; }
        public Specialty? Specialty { get; set; }

        public int StatusId { get; set; }
        public AppointmentStatus? Status { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public string? PublicAccessToken { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int DurationMinutes => (int)(EndAt - StartAt).TotalMinutes;
    }
}
