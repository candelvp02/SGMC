using Microsoft.AspNetCore.Mvc;
using SGMC.Application.Dto.Users;
using SGMC.Application.Interfaces.Service;
using SGMC.Web.Models.Patient;
using SGMC.Web.Services;

namespace SGMC.Web.Controllers
{
    public class PatientAdmController : Controller
    {
        private readonly IPatientApiClient _patientApiClient;
        private readonly IInsuranceProviderService _insuranceProviderService;

        public PatientAdmController(
            IPatientApiClient patientApiClient,
            IInsuranceProviderService insuranceProviderService)
        {
            _patientApiClient = patientApiClient;
            _insuranceProviderService = insuranceProviderService;
        }

        // GET: PatientAdmController
        public async Task<ActionResult> Index()
        {
            var apiResult = await _patientApiClient.GetAllAsync();

            if (!apiResult.Success || apiResult.Data == null)
            {
                ViewBag.ErrorMessage = apiResult.ErrorMessage ?? "Error al obtener los pacientes desde la API.";
                return View(new List<PatientDto>());
            }

            return View(apiResult.Data);
        }

        // GET: PatientAdmController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var apiResult = await _patientApiClient.GetByIdAsync(id);

            if (!apiResult.Success || apiResult.Data == null)
            {
                ViewBag.ErrorMessage = apiResult.ErrorMessage ?? "No se pudo obtener el paciente desde la API.";
                return View();
            }

            return View(apiResult.Data);
        }

        // GET: PatientAdmController/Create
        public async Task<ActionResult> Create()
        {
            await LoadInsuranceProviders();
            return View();
        }

        // POST: PatientAdmController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterPatientDto registerPatientDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadInsuranceProviders();
                return View(registerPatientDto);
            }

            var apiResult = await _patientApiClient.CreateAsync(registerPatientDto);

            if (!apiResult.Success)
            {
                ViewBag.ErrorMessage = apiResult.ErrorMessage ?? "Error al crear el paciente en la API.";
                await LoadInsuranceProviders();
                return View(registerPatientDto);
            }

            TempData["SuccessMessage"] = "Paciente creado correctamente (API).";
            return RedirectToAction(nameof(Index));
        }

        // GET: PatientAdmController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var apiResult = await _patientApiClient.GetByIdAsync(id);

            if (!apiResult.Success || apiResult.Data == null)
            {
                ViewBag.ErrorMessage = apiResult.ErrorMessage ?? "No se pudo obtener el paciente desde la API.";
                return View();
            }

            await LoadInsuranceProviders();
            return View(apiResult.Data);
        }

        // POST: PatientAdmController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UpdatePatientDto updatePatientDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await LoadInsuranceProviders();
                    return View(updatePatientDto);
                }

                var apiResult = await _patientApiClient.UpdateAsync(updatePatientDto);

                if (!apiResult.Success)
                {
                    ViewBag.ErrorMessage = apiResult.ErrorMessage ?? "Error al actualizar el paciente en la API.";
                    await LoadInsuranceProviders();
                    return View(updatePatientDto);
                }

                TempData["SuccessMessage"] = "Paciente actualizado correctamente (API).";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error al actualizar paciente: {ex.Message}";
                await LoadInsuranceProviders();
                return View(updatePatientDto);
            }
        }

        // GET: PatientAdmController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            var apiResult = await _patientApiClient.GetByIdAsync(id);

            if (!apiResult.Success || apiResult.Data == null)
            {
                ViewBag.ErrorMessage = apiResult.ErrorMessage ?? "No se pudo obtener el paciente desde la API.";
                return RedirectToAction(nameof(Index));
            }

            return View(apiResult.Data);
        }

        // POST: PatientAdmController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var apiResult = await _patientApiClient.DeleteAsync(id);

                if (!apiResult.Success)
                {
                    TempData["ErrorMessage"] = apiResult.ErrorMessage ?? "Error al desactivar paciente en la API.";
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Paciente desactivado correctamente (API).";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al desactivar paciente: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Método auxiliar para cargar proveedores de seguro
        private async Task LoadInsuranceProviders()
        {
            var insuranceResult = await _insuranceProviderService.GetActiveAsync();

            if (!insuranceResult.Exitoso || insuranceResult.Datos == null)
            {
                ViewBag.InsuranceProviders = new List<InsuranceProviderViewModel>();
                return;
            }

            ViewBag.InsuranceProviders = insuranceResult.Datos
                .Select(InsuranceProviderViewModel.FromDto)
                .ToList();
        }
    }
}
