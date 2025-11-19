using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SGMC.Application.Dto.Users;
using SGMC.Application.Interfaces.Persistence;
using SGMC.Persistence.Common;

namespace SGMC.Persistence.Ado.Users
{
    public class UserAdoRepository : IUserAdoRepository
    {
        private readonly StoredProcedureExecutor _sp;
        private readonly ILogger<UserAdoRepository> _logger;

        public UserAdoRepository(StoredProcedureExecutor sp, ILogger<UserAdoRepository> logger)
        {
            _sp = sp;
            _logger = logger;
        }

        public async Task<List<UserDto>> ListActiveAsync()
        {
            var users = new List<UserDto>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync("users.usp_User_ListActive");
                while (await r.ReadAsync())
                {
                    users.Add(MapUserDto(r));
                }
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing active users");
                return new List<UserDto>();
            }
        }

        public async Task<List<UserDto>> SearchByNameAsync(string name)
        {
            var users = new List<UserDto>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "users.usp_User_SearchByName",
                    ("@Name", name)
                );
                while (await r.ReadAsync())
                {
                    users.Add(MapUserDto(r));
                }
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users by name: {Name}", name);
                return new List<UserDto>();
            }
        }

        public async Task<List<UserDto>> ListByRoleAsync(int roleId)
        {
            var users = new List<UserDto>();
            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "users.usp_User_ListByRole",
                    ("@RoleID", roleId)
                );
                while (await r.ReadAsync())
                {
                    users.Add(MapUserDto(r));
                }
                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing users by role {RoleId}", roleId);
                return new List<UserDto>();
            }
        }

        public async Task<UserDto?> GetByIdWithDetailsAsync(int userId)
        {
            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "users.usp_User_GetByIdWithDetails",
                    ("@UserID", userId)
                );

                if (await r.ReadAsync())
                {
                    return MapUserDto(r);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID with details: {UserId}", userId);
                return null;
            }
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "users.usp_User_GetByEmail",
                    ("@Email", email)
                );

                if (await r.ReadAsync())
                {
                    return MapUserDto(r);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by email: {Email}", email);
                return null;
            }
        }

        public async Task<UserDto?> GetByUsernameAsync(string username)
        {
            try
            {
                using var r = await _sp.ExecuteReaderAsync(
                    "users.usp_User_GetByUsername",
                    ("@Username", username)
                );

                if (await r.ReadAsync())
                {
                    return MapUserDto(r);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username: {Username}", username);
                return null;
            }
        }

        public async Task<bool> UpdateUserStatusAsync(int userId, bool isActive)
        {
            try
            {
                var result = await _sp.ExecuteNonQueryAsync(
                    "users.usp_User_UpdateStatus",
                    ("@UserID", userId),
                    ("@IsActive", isActive)
                );
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user status for user {UserId}", userId);
                return false;
            }
        }

        public async Task<bool> UpdateUserRoleAsync(int userId, int roleId)
        {
            try
            {
                var result = await _sp.ExecuteNonQueryAsync(
                    "users.usp_User_UpdateRole",
                    ("@UserID", userId),
                    ("@RoleID", roleId)
                );
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user role for user {UserId}", userId);
                return false;
            }
        }

        private static UserDto MapUserDto(SqlDataReader r)
        {
            var userDto = new UserDto
            {
                UserId = r.GetInt32(r.GetOrdinal("UserID")),
                Email = r.GetString(r.GetOrdinal("Email")),
                RoleId = r.IsDBNull(r.GetOrdinal("RoleID")) ? null : r.GetInt32(r.GetOrdinal("RoleID")),
                RoleName = r.IsDBNull(r.GetOrdinal("RoleName")) ? string.Empty : r.GetString(r.GetOrdinal("RoleName")),
                IsActive = r.GetBoolean(r.GetOrdinal("IsActive")),
                CreatedAt = r.IsDBNull(r.GetOrdinal("CreatedAt")) ? null : r.GetDateTime(r.GetOrdinal("CreatedAt"))
            };

            // Propiedades de PersonBaseDto
            userDto.FirstName = r.GetString(r.GetOrdinal("FirstName"));
            userDto.LastName = r.GetString(r.GetOrdinal("LastName"));

            if (HasColumn(r, "PhoneNumber") && !r.IsDBNull(r.GetOrdinal("PhoneNumber")))
            {
                userDto.PhoneNumber = r.GetString(r.GetOrdinal("PhoneNumber"));
            }

            if (HasColumn(r, "DateOfBirth") && !r.IsDBNull(r.GetOrdinal("DateOfBirth")))
            {
                userDto.DateOfBirth = DateOnly.FromDateTime(r.GetDateTime(r.GetOrdinal("DateOfBirth")));
            }

            if (HasColumn(r, "Gender") && !r.IsDBNull(r.GetOrdinal("Gender")))
            {
                userDto.Gender = r.GetString(r.GetOrdinal("Gender"));
            }

            if (HasColumn(r, "IdentificationNumber") && !r.IsDBNull(r.GetOrdinal("IdentificationNumber")))
            {
                userDto.IdentificationNumber = r.GetString(r.GetOrdinal("IdentificationNumber"));
            }

            return userDto;
        }

        // metodo helper para verificar existencia de columnas
        private static bool HasColumn(SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}