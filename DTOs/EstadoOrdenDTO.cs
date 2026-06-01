using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class EstadoOrdenDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre del estado es obligatorio")]
        [Display(Name = "Estado")]
        public string Nombre { get; set; }

        public static List<string> EstadosDisponibles => new List<string>
        {
            "Pendiente",
            "En Progreso",
            "Completada",
            "Cancelada",
            "En Espera Proveedor/Parte"
        };
    }
}