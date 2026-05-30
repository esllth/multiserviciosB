using System.ComponentModel.DataAnnotations;

namespace MultiservicioB.DTOs
{
    public class ConsumoMaterialDTO
    {
        public int IdConsumo { get; set; }

        [Required(ErrorMessage = "La orden es obligatoria")]
        [Display(Name = "Orden de Servicio")]
        public int OrdenId { get; set; }

        [Required(ErrorMessage = "El material es obligatorio")]
        [Display(Name = "Material")]
        public int MaterialId { get; set; }

        public string? NombreMaterial { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Display(Name = "Cantidad Usada")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal CantidadUsada { get; set; }

        public decimal? PrecioUnitario { get; set; }

        public decimal CostoTotal => CantidadUsada * (PrecioUnitario ?? 0);
    }
}
