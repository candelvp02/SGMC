using System.ComponentModel.DataAnnotations;

namespace SGMC.Application.Dto.System
{
    // Base para credenciales
    public record CredentialsBaseDto
    {
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; init; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        public string Password { get; init; } = string.Empty;
    }

    // Login con credenciales
    public record LoginDto : CredentialsBaseDto
    {
    }

    // Registro de usuario
    public record RegisterUserDto : CredentialsBaseDto
    {
        [Required(ErrorMessage = "El rol es requerido")]
        public short RoleId { get; init; }
    }

    // Cambio de password
    public record ChangePasswordDto
    {
        [Required]
        public int UserId { get; init; }

        [Required(ErrorMessage = "La contraseña actual es requerida")]
        public string CurrentPassword { get; init; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres")]
        public string NewPassword { get; init; } = string.Empty;
    }

    // Reset de password
    public record ResetPasswordDto
    {
        [Required]
        public string Token { get; init; } = string.Empty;

        [Required(ErrorMessage = "La nueva contraseña es requerida")]
        [MinLength(6, ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres")]
        public string NewPassword { get; init; } = string.Empty;
    }
}