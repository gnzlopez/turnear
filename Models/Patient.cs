using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppointmentManager.Models
{
    public enum PatientType
    {
        [Display(Name = "Externo")]
        External = 0,

        [Display(Name = "Interno")]
        Internal = 1
    }

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

        // Tipo de paciente: Externo (por defecto) o Interno
        public PatientType PatientType { get; set; } = PatientType.External;

        [Required(ErrorMessage = "Debe ingresar el DNI.")]
        [Display(Name = "Número de Documento")]
        public string Dni { get; set; } = string.Empty;

        public ICollection<Appointment>? Appointments { get; set; }
    }
}
