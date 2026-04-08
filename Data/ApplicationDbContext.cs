using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Models;

namespace MultiservicioB.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<Empleado> Empleados { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}