using Microsoft.AspNetCore.Http;
using MultiservicioB.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiservicioB.Services.Interfaces
{
    /// <summary>
    /// Servicio para gestionar documentos de fabricación (RF-002, RF-008)
    /// </summary>
    public interface IDocumentoFabricacionService
    {
        Task<IEnumerable<DocumentoFabricacionDTO>> GetByProyectoIdAsync(int proyectoId);
        Task<IEnumerable<DocumentoFabricacionDTO>> GetByProyectoIdAndTipoAsync(int proyectoId, string tipoDocumento);
        Task<DocumentoFabricacionDTO?> GetByIdAsync(int id);
        Task<DocumentoFabricacionDTO> CreateAsync(DocumentoFabricacionDTO documentoDto, IFormFile archivo);
        Task<bool> DeleteAsync(int id);
    }
}
