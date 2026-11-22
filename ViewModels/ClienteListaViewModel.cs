namespace MultiservicioB.ViewModels;

public sealed class ClienteListaViewModel
{
    public int? IdCliente { get; init; }
    public string? Identificacion { get; init; }
    public string? Nombre { get; init; }
    public string? Apellidos { get; init; }
    public string? Correo { get; init; }
    public string? Telefono { get; init; }
    public string? NombreNegocio { get; init; }
    public string? Ubicacion { get; init; }
    public string? Estado { get; init; }
    public bool TienePerfilCliente { get; init; }
}
