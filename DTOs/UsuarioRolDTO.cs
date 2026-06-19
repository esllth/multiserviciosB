namespace MultiservicioB.DTOs
{
    public class UsuarioRolDTO
    {

        public required string Id { get; set; }
        public required string Email { get; set; }
        public required string UserName { get; set; }
        public string? RolActual { get; set; }
        public List<string> RolesDisponibles { get; set; } = new List<string>();

    }
}
