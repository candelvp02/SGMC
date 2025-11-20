using Microsoft.AspNetCore.Mvc;
using SGMC.Application.Dto.Users;
using SGMC.Application.Interfaces.Service;
using SGMC.Domain.Base;
using SGMC.Web.Models.Patient;

namespace SGMC.Web.Controllers
{
    public class PatientAdmController : Controller
    {
        private readonly IPatientService _patientService;
        private readonly IInsuranceProviderService _insuranceProviderService;

        public PatientAdmController(
            IPatientService patientService,
            IInsuranceProviderService insuranceProviderService)
        {
            _patientService = patientService;
            _insuranceProviderService = insuranceProviderService;
        }

        // GET: PatientAdmController
        public async Task<ActionResult> Index()
        {
            OperationResult<List<PatientDto>> result = await _patientService.GetAllAsync();

            if (!result.Exitoso)
            {
                ViewBag.ErrorMessage = result.Mensaje;
                return View();
            }

            return View(result.Datos);
        }

        // GET: PatientAdmController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            OperationResult<PatientDto> result = await _patientService.GetByIdWithDetailsAsync(id);

            if (!result.Exitoso)
            {
                ViewBag.ErrorMessage = result.Mensaje;
                return View();
            }

            return View(result.Datos);
        }

        // GET: PatientAdmController/Create
        public async Task<ActionResult> Create()
        {
            await LoadInsuranceProviders();
            return View();
        }

        // POST: PatientAdmController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create(RegisterPatientDto registerPatientDto)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            await LoadInsuranceProviders();
        //            return View(registerPatientDto);
        //        }

        //        OperationResult<PatientDto> result = await _patientService.CreateAsync(registerPatientDto);

        //        if (!result.Exitoso)
        //        {
        //            ViewBag.ErrorMessage = result.Mensaje;
        //            await LoadInsuranceProviders();
        //            return View(registerPatientDto);
        //        }

        //        TempData["SuccessMessage"] = "Paciente creado correctamente";
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (Exception ex)
        //    {
        //        var innerMessage =
        //            ex.InnerException?.InnerException?.Message
        //            ?? ex.InnerException?.Message
        //            ?? ex.Message;

        //        ViewBag.ErrorMessage = $"Error al crear paciente: {innerMessage}";
        //        await LoadInsuranceProviders();
        //        return View(registerPatientDto);
        //    }
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterPatientDto registerPatientDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadInsuranceProviders();
                return View(registerPatientDto);
            }

            OperationResult<PatientDto> result = await _patientService.CreateAsync(registerPatientDto);

            if (!result.Exitoso)
            {
                ViewBag.ErrorMessage = result.Mensaje;
                await LoadInsuranceProviders();
                return View(registerPatientDto);
            }

            TempData["SuccessMessage"] = "Paciente creado correctamente";
            return RedirectToAction(nameof(Index));
        }

        // GET: PatientAdmController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            OperationResult<PatientDto> result = await _patientService.GetByIdAsync(id);

            if (!result.Exitoso)
            {
                ViewBag.ErrorMessage = result.Mensaje;
                return View();
            }

            await LoadInsuranceProviders();
            return View(result.Datos);
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

                OperationResult<PatientDto> result = await _patientService.UpdateAsync(updatePatientDto);

                if (!result.Exitoso)
                {
                    ViewBag.ErrorMessage = result.Mensaje;
                    await LoadInsuranceProviders();
                    return View(updatePatientDto);
                }

                TempData["SuccessMessage"] = "Paciente actualizado correctamente";
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
            OperationResult<PatientDto> result = await _patientService.GetByIdAsync(id);

            if (!result.Exitoso)
            {
                ViewBag.ErrorMessage = result.Mensaje;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Datos);
        }

        // POST: PatientAdmController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                OperationResult result = await _patientService.DeleteAsync(id);

                if (!result.Exitoso)
                {
                    TempData["ErrorMessage"] = result.Mensaje;
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Paciente desactivado correctamente";
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