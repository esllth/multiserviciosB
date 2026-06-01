namespace MultiservicioB.DTOs
{
    public class UsuarioRolDTO
    {

        public string Id { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string? RolActual { get; set; }
        public List<string> RolesDisponibles { get; set; } = new List<string>();

    }
}
