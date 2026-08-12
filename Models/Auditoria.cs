using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace MultiservicioB.Models
{
    [Table("Auditoria")]
    public class Auditoria : BaseModel
    {
        [Key]
        public int IdAuditoria { get; set; }

        [Required, StringLength(450)]
        public string UsuarioId { get; set; } = "";

        [StringLength(100)]
        public string? Accion { get; set; }

        public DateTime? Fecha { get; set; } = DateTime.Now;

        [StringLength(255)]
        public string? Detalle { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public IdentityUser? Usuario { get; set; }
    }
}
