using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace SGMC.Persistence.Common
{
    public class StoredProcedureExecutor
    {
        private readonly string _connString;

        public StoredProcedureExecutor(IConfiguration cfg)
        {
            _connString = cfg.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Falta ConnectionString 'DefaultConnection'.");
        }
        public async Task<SqlDataReader> ExecuteReaderAsync(string spName, params (string Name, object? Value)[] parameters)
        {
            var connection = new SqlConnection(_connString);
            try
            {
                await connection.OpenAsync();
                var command = new SqlCommand(spName, connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                foreach (var (name, value) in parameters)
                {
                    command.Parameters.AddWithValue(name, value ?? DBNull.Value);
                }

                return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
            }
            catch
            {
                if (connection.State == ConnectionState.Open)
                    await connection.CloseAsync();
                connection.Dispose();
                throw;
            }
        }
        public async Task<int> ExecuteNonQueryAsync(string spName, params (string Name, object? Value)[] parameters)
        {
            await using var connection = new SqlConnection(_connString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(spName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            foreach (var (name, value) in parameters)
            {
                command.Parameters.AddWithValue(name, value ?? DBNull.Value);
            }

            return await command.ExecuteNonQueryAsync();
        }
        public async Task<T?> ExecuteScalarAsync<T>(string spName, params (string Name, object? Value)[] parameters)
        {
            await using var connection = new SqlConnection(_connString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(spName, connection)
            {
                CommandType = CommandType.StoredProcedure
            };

            foreach (var (name, value) in parameters)
            {
                command.Parameters.AddWithValue(name, value ?? DBNull.Value);
            }

            var result = await command.ExecuteScalarAsync();
            return result is T t ? t : default;
        }
    }
}