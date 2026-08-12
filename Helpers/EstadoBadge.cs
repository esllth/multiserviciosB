using System.Globalization;
using System.Text;

namespace MultiservicioB.Helpers;

public static class EstadoBadge
{
    public const string Activo = "idx-badge-active";
    public const string Inactivo = "idx-badge-inactive";
    public const string Atencion = Inactivo;
    public const string Peligro = "idx-badge-danger";
    public const string Principal = "idx-badge-primary";
    public const string Informativo = "idx-badge-info";

    public static string General(string? estado)
    {
        var valor = Normalizar(estado);

        if (Es(valor, "activo", "aprobado", "aprobada", "completado", "completada",
                "finalizado", "finalizada", "confirmado", "confirmada", "registrada",
                "compromiso confirmado", "aviso enviado", "dentro del plazo", "operativo"))
            return Activo;

        if (Es(valor, "rechazado", "rechazada", "cancelado", "cancelada",
                "fuera de servicio", "bajo stock", "stock critico", "mantenimiento vencido",
                "mantenimiento critico", "fueraservicio", "error"))
            return Peligro;

        if (Es(valor, "pendiente de confirmacion", "compromiso pendiente", "en mantenimiento",
                "mantenimiento proximo", "proximo al vencimiento", "en diseno", "endiseno",
                "enmantenimiento", "en espera"))
            return Atencion;

        if (Es(valor, "evaluado", "evaluada", "en produccion", "enproduccion", "administrador", "empleado",
                "cliente", "gerente", "rol asignado"))
            return Principal;

        if (Es(valor, "en progreso", "enprogreso", "obligatoria", "informacion preventiva"))
            return Informativo;

        return Inactivo;
    }

    public static string Cotizacion(string? estado) => General(estado);

    public static string OrdenServicio(string? estado) =>
        Normalizar(estado) == "pendiente" ? Inactivo : General(estado);

    public static string Empleado(string? estado) =>
        Normalizar(estado) == "pendiente" ? Inactivo : General(estado);

    public static string CuentaEmpleado(bool registrada) => registrada ? Activo : Inactivo;

    public static string Rol(bool asignado) => asignado ? Principal : Inactivo;

    public static string Servicio(string? estado) =>
        Normalizar(estado) == "inactivo" ? Peligro : General(estado);

    public static string Equipo(string? estado) => General(estado);

    public static string Fabricacion(string? estado)
    {
        var valor = Normalizar(estado);
        return Es(valor, "aprobado", "aprobada") ? Informativo : General(estado);
    }

    public static string Material(string? estado) => General(estado);

    public static string ConfiguracionDisponible(string? estado) =>
        Normalizar(estado) == "inactivo" ? Peligro : General(estado);

    public static string ConfiguracionClave() => Inactivo;

    public static string EvidenciaCompleta(bool completa) => completa ? Activo : Inactivo;

    public static string TipoEvidencia(string? tipo) =>
        Normalizar(tipo) == "final" ? Activo : Principal;

    private static bool Es(string valor, params string[] opciones) => opciones.Contains(valor);

    private static string Normalizar(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            return string.Empty;

        var descompuesto = valor.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var resultado = new StringBuilder(descompuesto.Length);

        foreach (var caracter in descompuesto)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(caracter) != UnicodeCategory.NonSpacingMark)
                resultado.Append(caracter);
        }

        return string.Join(' ', resultado.ToString().Normalize(NormalizationForm.FormC)
            .Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}
