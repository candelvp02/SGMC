using System.ComponentModel.DataAnnotations;
using SGMC.Application.Dto.System;
using SGMC.Application.Dto.Users;

namespace SGMC.Web.Models.User
{
    // ViewModel para la vista de creación de usuarios
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
            ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula y un número")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe seleccionar un rol")]
        [Range(1, short.MaxValue, ErrorMessage = "Debe seleccionar un rol válido")]
        [Display(Name = "Rol")]
        public short RoleId { get; set; }

        // Lista para el dropdown de roles
        public List<RoleSelectViewModel>? Roles { get; set; }

        // Convierte el ViewModel a DTO
        public RegisterUserDto ToDto()
        {
            return new RegisterUserDto
            {
                Email = this.Email,
                Password = this.Password,
                RoleId = this.RoleId
            };
        }
    }

    // ViewModel para la vista de edición de usuarios
    public class EditUserViewModel
    {
        [Required]
        public int UserId { get; set; }

        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        // Propiedades de solo lectura
        [Display(Name = "Rol Actual")]
        public string RoleName { get; set; } = string.Empty;

        [Display(Name = "Estado")]
        public bool IsActive { get; set; }

        [Display(Name = "Fecha de Creación")]
        public DateTime? CreatedAt { get; set; }

        // Crea el ViewModel desde un DTO
        public static EditUserViewModel FromDto(UserDto dto)
        {
            return new EditUserViewModel
            {
                UserId = dto.UserId,
                Email = dto.Email,
                RoleName = dto.RoleName,
                IsActive = dto.IsActive,
                CreatedAt = dto.CreatedAt
            };
        }

        // Convierte el ViewModel a DTO
        public UpdateUserDto ToDto()
        {
            return new UpdateUserDto
            {
                UserId = this.UserId,
                Email = this.Email
            };
        }
    }

    // ViewModel para la lista de usuarios
    public class UserListViewModel
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string CreatedAtFormatted => CreatedAt.HasValue
            ? CreatedAt.Value.ToString("dd/MM/yyyy HH:mm")
            : "N/A";

        public string StatusBadgeClass => IsActive ? "bg-success" : "bg-danger";
        public string StatusText => IsActive ? "Activo" : "Inactivo";

        public static UserListViewModel FromDto(UserDto dto)
        {
            return new UserListViewModel
            {
                UserId = dto.UserId,
                Email = dto.Email,
                RoleName = dto.RoleName,
                IsActive = dto.IsActive,
                CreatedAt = dto.CreatedAt
            };
        }
    }

    // ViewModel para detalles del usuario
    public class UserDetailsViewModel
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string StatusBadgeClass => IsActive ? "bg-success" : "bg-danger";
        public string StatusText => IsActive ? "Activo" : "Inactivo";
        public string CreatedAtFormatted => CreatedAt.HasValue
            ? CreatedAt.Value.ToString("dd/MM/yyyy HH:mm")
            : "N/A";

        public static UserDetailsViewModel FromDto(UserDto dto)
        {
            return new UserDetailsViewModel
            {
                UserId = dto.UserId,
                Email = dto.Email,
                RoleName = dto.RoleName,
                IsActive = dto.IsActive,
                CreatedAt = dto.CreatedAt
            };
        }
    }

    // ViewModel para cambio de contraseña
    public class ChangePasswordViewModel
    {
        [Required]
        public int UserId { get; set; }

        [Required(ErrorMessage = "La contraseña actual es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña Actual")]
        public string CurrentPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$",
            ErrorMessage = "La contraseña debe contener al menos una mayúscula, una minúscula y un número")]
        [DataType(DataType.Password)]
        [Display(Name = "Nueva Contraseña")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Debe confirmar la contraseña")]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nueva Contraseña")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // Convierte el ViewModel a DTO
        public ChangePasswordDto ToDto()
        {
            return new ChangePasswordDto
            {
                UserId = this.UserId,
                CurrentPassword = this.CurrentPassword,
                NewPassword = this.NewPassword
            };
        }
    }

    // ViewModel para login
    public class LoginViewModel
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }

        // Convierte el ViewModel a DTO
        public LoginDto ToDto()
        {
            return new LoginDto
            {
                Email = this.Email,
                Password = this.Password
            };
        }
    }

    // ViewModel auxiliar para selección de roles
    public class RoleSelectViewModel
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}