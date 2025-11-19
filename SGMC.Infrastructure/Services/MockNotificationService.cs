using Microsoft.Extensions.Logging;
using SGMC.Application.Dto.System;
using SGMC.Application.Interfaces.Service;
using SGMC.Domain.Base;
using SGMC.Domain.Repositories.Appointments;

namespace SGMC.Infrastructure.Services
{
    public class MockNotificationService : INotificationService
    {
        private readonly ILogger<MockNotificationService> _logger;
        private readonly IAppointmentRepository _appointmentRepository;

        private readonly List<string> _sentNotifications = new();

        public MockNotificationService(
            ILogger<MockNotificationService> logger,
            IAppointmentRepository appointmentRepository)
        {
            _logger = logger;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<OperationResult> SendAppointmentConfirmationAsync(int appointmentId)
        {
            _logger.LogInformation("Enviando confirmacion de cita {AppointmentId}", appointmentId);

            await Task.Delay(100);

            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
            if (appointment == null)
            {
                return new OperationResult
                {
                    Exitoso = false,
                    Mensaje = "Cita no encontrada"
                };
            }

            var success = new Random().Next(0, 100) < 90;

            if (success)
            {
                _sentNotifications.Add($"CONFIRMATION_{appointmentId}");
                _logger.LogInformation("Confirmacion enviada para cita {AppointmentId}", appointmentId);
                return new OperationResult
                {
                    Exitoso = true,
                    Mensaje = $"Confirmacion enviada al paciente {appointment.PatientId}"
                };
            }
            else
            {
                _logger.LogWarning("Error simulado enviando confirmacion {AppointmentId}", appointmentId);
                return new OperationResult
                {
                    Exitoso = false,
                    Mensaje = "Error simulado: No se pudo enviar la confirmacion"
                };
            }
        }

        public async Task<OperationResult> SendAppointmentReminderAsync(int appointmentId)
        {
            _logger.LogInformation("Enviando recordatorio de cita {AppointmentId}", appointmentId);

            await Task.Delay(80);

            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
            if (appointment == null)
                return new OperationResult { Exitoso = false, Mensaje = "Cita no encontrada" };

            var hoursUntil = (appointment.AppointmentDate - DateTime.Now).TotalHours;

            _sentNotifications.Add($"REMINDER_{appointmentId}");
            _logger.LogInformation("Recordatorio enviado para cita {AppointmentId} ({HoursUntil}h antes)",
                appointmentId, hoursUntil);

            return new OperationResult
            {
                Exitoso = true,
                Mensaje = $"Recordatorio enviado ({hoursUntil:F1}h antes)"
            };
        }

        public async Task<OperationResult> SendAppointmentCancellationAsync(int appointmentId)
        {
            _logger.LogInformation("Enviando cancelacion de cita {AppointmentId}", appointmentId);

            await Task.Delay(120);

            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
            if (appointment == null)
                return new OperationResult { Exitoso = false, Mensaje = "Cita no encontrada" };

            _sentNotifications.Add($"CANCELLATION_{appointmentId}");
            _logger.LogInformation("Cancelacion enviada para cita {AppointmentId}", appointmentId);

            return new OperationResult
            {
                Exitoso = true,
                Mensaje = $"Cancelacion notificada al paciente {appointment.PatientId}"
            };
        }

        public async Task<OperationResult> SendAppointmentRescheduleAsync(int appointmentId)
        {
            _logger.LogInformation("Enviando reprogramacion de cita {AppointmentId}", appointmentId);

            await Task.Delay(100);

            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
            if (appointment == null)
                return new OperationResult { Exitoso = false, Mensaje = "Cita no encontrada" };

            _sentNotifications.Add($"RESCHEDULE_{appointmentId}");
            _logger.LogInformation("Reprogramacion enviada para cita {AppointmentId}", appointmentId);

            return new OperationResult
            {
                Exitoso = true,
                Mensaje = $"Reprogramacion notificada para {appointment.AppointmentDate}"
            };
        }

        public async Task<OperationResult> SendCustomReminderAsync(int appointmentId, string message)
        {
            _logger.LogInformation("Enviando recordatorio personalizado para cita {AppointmentId}", appointmentId);

            await Task.Delay(90);

            var appointment = await _appointmentRepository.GetByIdWithDetailsAsync(appointmentId);
            if (appointment == null)
                return new OperationResult { Exitoso = false, Mensaje = "Cita no encontrada" };

            _sentNotifications.Add($"CUSTOM_{appointmentId}");
            _logger.LogInformation("Recordatorio personalizado: {Message}", message);

            return new OperationResult
            {
                Exitoso = true,
                Mensaje = $"Recordatorio personalizado enviado: {message}"
            };
        }

        public async Task<OperationResult> SendAccountActivationEmailAsync(string email, int userId)
        {
            _logger.LogInformation("Enviando activacion de cuenta a {Email}", email);

            await Task.Delay(150);

            _sentNotifications.Add($"ACCOUNT_ACTIVATION_{userId}");
            _logger.LogInformation("Email de activacion enviado a {Email}", email);

            return new OperationResult
            {
                Exitoso = true,
                Mensaje = $"Email de activacion enviado a {email}"
            };
        }

        // Metodos para testing
        public List<string> GetSentNotifications() => new(_sentNotifications);
        public void ClearSentNotifications() => _sentNotifications.Clear();
        public bool WasNotificationSent(string type, int appointmentId) =>
            _sentNotifications.Contains($"{type}_{appointmentId}");

        public async Task<OperationResult> SendNotificationAsync(NotificationDto notificationDto)
        {
            _logger.LogInformation("MOCK: Enviando notificacion DTO de tipo {Type} a usuario {UserId}", "N/A", "N/A"); // Ajusta el logging
            await Task.Delay(50);
            return new OperationResult { Exitoso = true, Mensaje = "Notificacion DTO simulada enviada" };
        }

        public async Task<IEnumerable<NotificationDto>> GetPendingNotificationsAsync(int userId)
        {
            _logger.LogInformation("MOCK: Obteniendo notificaciones pendientes para usuario {UserId}", userId);
            await Task.Delay(50);
            return new List<NotificationDto>();
        }

        public async Task<OperationResult> MarkAsReadAsync(int notificationId)
        {
            _logger.LogInformation("MOCK: Marcando notificacion {NotificationId} como leída", notificationId);
            await Task.Delay(50);
            return new OperationResult { Exitoso = true, Mensaje = $"Notificacion {notificationId} marcada como leída" };
        }

        public async Task<OperationResult> SendPasswordResetEmailAsync(string email, int userId)
        {
            _logger.LogInformation("MOCK: Enviando reset de password a {Email} (UserId: {UserId})", email, userId);
            await Task.Delay(150);
            _sentNotifications.Add($"PASSWORD_RESET_{email}_{userId}");

            return new OperationResult
            {
                Exitoso = true,
                Mensaje = $"Email de recuperacion enviado a {email} (UserId: {userId})"
            };
        }

        Task<OperationResult<List<NotificationDto>>> INotificationService.GetPendingNotificationsAsync(int userId)
        {
            throw new NotImplementedException();
        }
    }
}