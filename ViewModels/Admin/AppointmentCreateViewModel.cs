using System.ComponentModel.DataAnnotations;

namespace AppointmentManager.ViewModels.Admin;

public class AppointmentCreateViewModel
{
    [Required(ErrorMessage = "Debe seleccionar un profesional.")]
    public int? ProfessionalId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar una fecha.")]
    public DateOnly? Date { get; set; }

    public string? SelectedSlotValue { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un paciente.")]
    public int? PatientId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un estado.")]
    public int StatusId { get; set; } = 1;

    public string? Notes { get; set; }
}