using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace AppointmentManager.Models
{
    public class ProfessionalAvailability
    {
        public int Id { get; set; }

        [Required]
        public int ProfessionalId { get; set; }

        [ValidateNever]
        public Professional Professional { get; set; } = null!;

        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        public TimeOnly StartTime { get; set; }

        [Required]
        public TimeOnly EndTime { get; set; }

        [Range(5, 240)]
        public int SlotDurationMinutes { get; set; } = 30;

        public bool IsActive { get; set; } = true;
    }
}
