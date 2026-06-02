using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;

namespace MultiservicioB.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public LoginModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "El correo electrónico es requerido")]
            [EmailAddress(ErrorMessage = "El formato del correo no es válido")]
            [Display(Name = "Correo electrónico")]
            public string Email { get; set; } = "";

            [Required(ErrorMessage = "La contraseña es requerida")]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; } = "";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            string email = Input.Email.Trim().ToLower();

            // 1. Buscar si el usuario existe en la seguridad de Identity
            var user = await _userManager.FindByEmailAsync(Input.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Credenciales inválidas");
                return Page();
            }

            // ==========================================================================
            // FILTRO DE SEGURIDAD INTERNO [RMGUS-005]
            // ==========================================================================

            // Verificamos de primero si el usuario tiene el rol de Administrador
            bool esAdmin = await _userManager.IsInRoleAsync(user, "Administrador");

            // REGLA SUPREMA: Si es ADMIN, se salta olímpicamente cualquier validación de empleado
            if (!esAdmin)
            {
                // Solo si NO es admin y usa correo de la empresa, validamos como empleado técnico
                if (email.EndsWith("@multiserviciosb.com"))
                {
                    var empleado = await _context.Empleados
                        .FirstOrDefaultAsync(e => e.CorreoElectronicoEmpleado.ToLower() == email);

                    // Si no está pre-cargado en la tabla o fue desactivado por gerencia
                    if (empleado == null || empleado.EstadoEmpleado == false)
                    {
                        ModelState.AddModelError("", "Su cuenta de empleado se encuentra inactiva. Contacte a gerencia.");
                        return Page();
                    }

                }
            }

            // 2. Verificar la contraseña si superó los filtros correspondientes
            var result = await _signInManager.CheckPasswordSignInAsync(user, Input.Password, false);

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Credenciales inválidas");
                return Page();
            }

            // 3. Iniciar sesión oficialmente
            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("Dashboard", "Home");
        }
    }
}
