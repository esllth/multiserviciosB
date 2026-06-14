using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MultiservicioB.Data;
using MultiservicioB.Models;

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
            [StringLength(100, MinimumLength = 12, ErrorMessage = "La contraseña debe tener al menos {2} caracteres")]
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

            string email = Input.Email.Trim().ToLower();
            bool isCompanyEmail = email.EndsWith("@multiserviciosb.com");

            var existingUser = await _userManager.FindByEmailAsync(email);
            var hadUsers = await _userManager.Users.AnyAsync();
            if (!hadUsers && !isCompanyEmail)
            {
                ModelState.AddModelError("", "El primer usuario debe usar el dominio corporativo @multiserviciosb.com.");
                return Page();
            }

            // ==========================================================================
            //  MODO REGISTRO DE CUENTA (Para Técnicos / Empleados precargados)
            // ==========================================================================
            var empleado = await _context.Empleados
                .FirstOrDefaultAsync(e => e.CorreoElectronicoEmpleado.Trim().ToLower() == email);

            if (empleado != null)
            {
                if (!EstadosEmpleado.PuedeAcceder(empleado))
                {
                    ModelState.AddModelError("", "Su perfil de empleado todavía no está activo. Contacte a gerencia.");
                    return Page();
                }

                await using var transaction = await _context.Database.BeginTransactionAsync();

                if (existingUser != null)
                {
                    if (await _userManager.HasPasswordAsync(existingUser))
                    {
                        ModelState.AddModelError("", "Este correo ya tiene una cuenta registrada. Inicie sesión.");
                        return Page();
                    }

                    var addPasswordResult = await _userManager.AddPasswordAsync(existingUser, Input.Password);
                    if (!addPasswordResult.Succeeded)
                    {
                        foreach (var error in addPasswordResult.Errors)
                            ModelState.AddModelError("", error.Description);
                        return Page();
                    }

                    if (!await _userManager.IsInRoleAsync(existingUser, "Empleado"))
                    {
                        var existingUserRoleResult = await _userManager.AddToRoleAsync(existingUser, "Empleado");
                        if (!existingUserRoleResult.Succeeded)
                        {
                            foreach (var error in existingUserRoleResult.Errors)
                                ModelState.AddModelError("", error.Description);
                            return Page();
                        }
                    }

                    empleado.UserId = existingUser.Id;
                }
                else
                {
                    var employeeUser = new IdentityUser { UserName = email, Email = email };
                    var createResult = await _userManager.CreateAsync(employeeUser, Input.Password);
                    if (!createResult.Succeeded)
                    {
                        foreach (var error in createResult.Errors)
                            ModelState.AddModelError("", error.Description);
                        return Page();
                    }

                    var employeeRoleResult = await _userManager.AddToRoleAsync(employeeUser, "Empleado");
                    if (!employeeRoleResult.Succeeded)
                    {
                        foreach (var error in employeeRoleResult.Errors)
                            ModelState.AddModelError("", error.Description);
                        return Page();
                    }

                    empleado.UserId = employeeUser.Id;
                }

                empleado.TieneUsuario = true;
                EstadosEmpleado.Aplicar(empleado, EstadosEmpleado.Activo);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Registro completado. Ya puede iniciar sesión.";
                return RedirectToPage("./Login");
            }

            // ==========================================================================
            //  MODO CREACIÓN NORMAL (Para el Admin inicial o Clientes nuevos)
            // ==========================================================================

            // Si llega aquí, significa que es un cliente externo o el primer administrador.
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Este correo electrónico ya se encuentra registrado. Intente iniciar sesión.");
                return Page();
            }

            var user = new IdentityUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, Input.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return Page();
            }

            // Asignación de Roles estándar
            var roleResult = await _userManager.AddToRoleAsync(user, !hadUsers ? "Administrador" : "Cliente");
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                foreach (var error in roleResult.Errors)
                    ModelState.AddModelError("", error.Description);
                return Page();
            }

            if (hadUsers)
            {
                var cliente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.Correo != null && c.Correo.ToLower() == email);

                if (cliente != null)
                {
                    cliente.Estado = "Activo";
                    await _context.SaveChangesAsync();
                }

                await _signInManager.SignInAsync(user, false);
                return RedirectToAction("Dashboard", "Home");
            }

            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Dashboard", "Home");
        }
    }
}
