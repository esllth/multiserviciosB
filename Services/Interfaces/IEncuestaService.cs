using MultiservicioB.ViewModels;

namespace MultiservicioB.Services.Interfaces
{
    public interface IEncuestaService
    {
        Task<IReadOnlyList<EncuestaClienteViewModel>> ObtenerEncuestasClienteAsync(string correo);
        Task<bool> TieneEncuestasPendientesClienteAsync(string correo);
        Task<bool> GuardarRespuestaClienteAsync(string correo, ResponderEncuestaViewModel respuesta);
    }
}
