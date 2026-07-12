using System.ComponentModel.DataAnnotations;

namespace AppointmentManager.Models
{
    public class AppointmentStatus
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        // Indica si al cambiar a este estado se debe enviar un mail al paciente
        public bool SendMail { get; set; } = false;

        // Plantilla HTML/texto para el correo (opcional)
        [MaxLength(4000)]
        public string? EmailTemplate { get; set; }

        public ICollection<Appointment>? Appointments { get; set; }
    }
}
