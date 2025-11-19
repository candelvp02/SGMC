using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using SGMC.Application.Dto.Appointments;

namespace SGMC.Web.Services
{
    public class AppointmentApiClient : IAppointmentApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private class OperationResultDto<T>
        {
            public bool Exitoso { get; set; }
            public string? Mensaje { get; set; }
            public T? Datos { get; set; }
        }

        public AppointmentApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<ApiResponse<List<AppointmentDto>>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("Appointments");
            return await HandleResponse<List<AppointmentDto>>(response);
        }

        public async Task<ApiResponse<AppointmentDto>> GetByIdAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Appointments/{id}");
            return await HandleResponse<AppointmentDto>(response);
        }

        public async Task<ApiResponse<AppointmentDto>> CreateAsync(CreateAppointmentDto dto)
        {
            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("Appointments", content);
            return await HandleResponse<AppointmentDto>(response);
        }

        public async Task<ApiResponse<AppointmentDto>> UpdateAsync(UpdateAppointmentDto dto)
        {
            var json = JsonSerializer.Serialize(dto, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"Appointments/{dto.Id}", content);
            return await HandleResponse<AppointmentDto>(response);
        }

        public async Task<ApiResponse<bool>> DeleteAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"Appointments/{id}");

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

        public async Task<ApiResponse<List<AppointmentDto>>> GetByPatientAsync(int patientId)
        {
            var response = await _httpClient.GetAsync($"Appointments/patient/{patientId}");
            return await HandleResponse<List<AppointmentDto>>(response);
        }

        public async Task<ApiResponse<List<AppointmentDto>>> GetByDoctorAsync(int doctorId)
        {
            var response = await _httpClient.GetAsync($"Appointments/doctor/{doctorId}");
            return await HandleResponse<List<AppointmentDto>>(response);
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
