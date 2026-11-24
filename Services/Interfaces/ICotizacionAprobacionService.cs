namespace MultiservicioB.Services.Interfaces
{
    public sealed record CotizacionAprobacionResult(bool Exito, string? Error = null, int? OrdenId = null);

    public interface ICotizacionAprobacionService
    {
        Task<CotizacionAprobacionResult> AprobarAsync(int cotizacionId, string usuarioId);
        Task<CotizacionAprobacionResult> RechazarAsync(int cotizacionId, string usuarioId, string motivo);
    }
}
