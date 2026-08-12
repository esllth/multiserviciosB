namespace MultiservicioB.DTOs
{
    public class UsuarioRolDTO
    {

        public required string Id { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public string? RolActual { get; set; }
        public List<string> RolesDisponibles { get; set; } = new List<string>();
        public bool EsEmpleado { get; set; }
        public bool EsAdministrador { get; set; }
        public bool EsSecretaria { get; set; }
        public bool EsAdministradorPrincipal { get; set; }

    }
}
