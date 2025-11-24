using System.Net.Http;
using System.Text;
using System.Text.Json;
using SGMC.Application.Dto.Users;
using SGMC.Web.Models;

namespace SGMC.Web.Services
{
    public class PatientApiClient : IPatientApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public PatientApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiResponse<List<PatientDto>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("Patients");
            return await HandleResponse<List<PatientDto>>(response);
        }

        public async Task<ApiResponse<PatientDto>> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Patients/{id}");
            return await HandleResponse<PatientDto>(response);
        }

        public async Task<ApiResponse<PatientDto>> CreateAsync(RegisterPatientDto dto)
        {
            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Patients", content);
            return await HandleResponse<PatientDto>(response);
        }

        public async Task<ApiResponse<PatientDto>> UpdateAsync(UpdatePatientDto dto)
        {
            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"Patients/{dto.PatientId}", content);
            return await HandleResponse<PatientDto>(response);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"Patients/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<bool>
                {
                    Success = false,
                    ErrorMessage = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}"
                };
            }

            return new ApiResponse<bool>
            {
                Success = true,
                Data = true
            };
        }

        private async Task<ApiResponse<T>> HandleResponse<T>(HttpResponseMessage response)
        {
            var apiResponse = new ApiResponse<T>();

            if (!response.IsSuccessStatusCode)
            {
                apiResponse.Success = false;
                apiResponse.ErrorMessage = $"HTTP {(int)response.StatusCode}: {response.ReasonPhrase}";
                return apiResponse;
            }

            var content = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(content))
            {
                apiResponse.Success = false;
                apiResponse.ErrorMessage = "Empty response from API.";
                return apiResponse;
            }

            var opResult = JsonSerializer.Deserialize<OperationResultDto<T>>(content, _jsonOptions);

            if (opResult == null)
            {
                apiResponse.Success = false;
                apiResponse.ErrorMessage = "Could not parse API response.";
                return apiResponse;
            }

            apiResponse.Success = opResult.Exitoso;
            apiResponse.ErrorMessage = opResult.Mensaje;
            apiResponse.Data = opResult.Datos;

            return apiResponse;
        }
    }
}
