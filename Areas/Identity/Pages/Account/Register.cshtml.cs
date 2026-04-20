using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;

namespace MultiservicioB.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
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
            [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos {2} caracteres")]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Password { get; set; } = "";

            [Required(ErrorMessage = "Debe confirmar la contraseña")]
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar contraseña")]
            [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
            public string ConfirmPassword { get; set; } = "";
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // VALIDAR ANTES DE CREAR
            var hadUsers = await _userManager.Users.AnyAsync();

            var user = new IdentityUser
            {
                UserName = Input.Email,
                Email = Input.Email
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                return Page();
            }

            string email = Input.Email.ToLower();
            bool isCompanyEmail = email.EndsWith("@multiserviciosb.com");

            // ============================
            // 🔴 1. PRIMER USUARIO = ADMIN
            // ============================
            if (!hadUsers)
            {
                await _userManager.AddToRoleAsync(user, "Administrador");
            }

            // ============================
            // 🔵 2. EMPLEADO
            // ============================
            else if (isCompanyEmail)
            {
                var empleado = await _context.Empleados
                    .FirstOrDefaultAsync(e =>
                        e.CorreoElectronicoEmpleado.ToLower() == email);

                if (empleado == null)
                {
                    ModelState.AddModelError("", "No autorizado como empleado.");
                    await _userManager.DeleteAsync(user);
                    return Page();
                }

                if (empleado.TieneUsuario)
                {
                    ModelState.AddModelError("", "Este empleado ya tiene cuenta.");
                    await _userManager.DeleteAsync(user);
                    return Page();
                }

                await _userManager.AddToRoleAsync(user, "Empleado");

                // 🔥 SINCRONIZACIÓN
                empleado.UserId = user.Id;
                empleado.TieneUsuario = true;
                empleado.EstadoEmpleado = "Activo";

                _context.Update(empleado);
                await _context.SaveChangesAsync();
            }

            // ============================
            // 🟢 3. CLIENTE
            // ============================
            else
            {
                await _userManager.AddToRoleAsync(user, "Cliente");
            }

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction("Dashboard", "Home");
        }
    }
}