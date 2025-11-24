using SGMC.Application.Dto.System;
using SGMC.Domain.Base;

namespace SGMC.Application.Validators.System
{
    // Validador para DTOs de Notification
    public static class NotificationValidator
    {
        // Valida NotificationDto
        public static OperationResult IsValidDto(this NotificationDto dto)
        {
            var errores = new List<string>();

            if (dto.RecipientId <= 0)
                errores.Add("El ID del destinatario es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Title))
                errores.Add("El título de la notificación es requerido.");

            if (string.IsNullOrWhiteSpace(dto.Message))
                errores.Add("El mensaje de la notificación es requerido.");

            return errores.Count > 0
                ? OperationResult.Fallo("Errores de validación de notificación.", errores)
                : OperationResult.Exito();
        }
    }
}