using Microsoft.AspNetCore.Mvc;
using SGMC.Application.Dto.Appointments;
using SGMC.Application.Interfaces.Service;
using SGMC.Web.Models.Appointment;
using SGMC.Web.Services;

namespace SGMC.Web.Controllers
{
    public class AppointmentAdmController : Controller
    {
        private readonly IAppointmentApiClient _appointmentApiClient;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        private readonly IAppointmentService _appointmentService;

        public AppointmentAdmController(
            IAppointmentApiClient appointmentApiClient,
            IPatientService patientService,
            IDoctorService doctorService,
            IAppointmentService appointmentService)
        {
            _appointmentApiClient = appointmentApiClient;
            _patientService = patientService;
            _doctorService = doctorService;
            _appointmentService = appointmentService;
        }

        // GET: AppointmentAdm/Index
        public async Task<ActionResult> Index([FromQuery] AppointmentFilterDto filter)
        {
            Console.WriteLine("🔍 AppointmentAdmController.Index (consumo API) iniciado...");

            var apiResult = await _appointmentApiClient.GetAllAsync();

            if (!apiResult.Success || apiResult.Data == null)
            {
                ViewBag.ErrorMessage = apiResult.ErrorMessage ?? "Error al obtener las citas desde la API.";

                var emptyViewModel = new AppointmentIndexViewModel
                {
                    Filter = filter,
                    Patients = await GetPatientsList(),
                    Doctors = await GetDoctorsList(),
                    Statuses = GetStatusesList()
                };

                return View(emptyViewModel);
            }

            var filteredAppointments = ApplyFilter(apiResult.Data, filter)
                .Select(AppointmentListViewModel.FromDto)
                .ToList();

            var viewModel = new AppointmentIndexViewModel
            {
                Filter = filter,
                Appointments = filteredAppointments,
                Patients = await GetPatientsList(),
                Doctors = await GetDoctorsList(),
                Statuses = GetStatusesList()
            };

            Console.WriteLine($"ViewModel Count: {viewModel.Appointments.Count}");

            return View(viewModel);
        }

        // Metodo auxiliar para aplicar filtros en la capa de presentación
        private IEnumerable<AppointmentDto> ApplyFilter(IEnumerable<AppointmentDto> source, AppointmentFilterDto filter)
        {
            var query = source;

            if (filter.PatientId.HasValue && filter.PatientId.Value > 0)
                query = query.Where(a => a.PatientId == filter.PatientId.Value);

            if (filter.DoctorId.HasValue && filter.DoctorId.Value > 0)
                query = query.Where(a => a.DoctorId == filter.DoctorId.Value);

            if (filter.StatusId.HasValue && filter.StatusId.Value > 0)
                query = query.Where(a => a.StatusId == filter.StatusId.Value);

            return query;
        }

        // GET: /AppointmentAdm/TestDb
        public async Task<IActionResult> TestDb()
        {
            try
            {
                Console.WriteLine("TestDb (API) iniciado...");

                var apiResult = await _appointmentApiClient.GetAllAsync();

                var debugInfo = new
                {
                    Success = apiResult.Success,
                    ErrorMessage = apiResult.ErrorMessage,
                    TotalCitas = apiResult.Data?.Count ?? 0,
                    Citas = apiResult.Data?.Select(d => new
                    {
                        d.AppointmentId,
                        d.PatientName,
                        d.DoctorName,
                        d.StatusName,
                        d.AppointmentDate
                    }).ToList()
                };

                return Json(debugInfo);
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    Error = ex.Message,
                    InnerException = ex.InnerException?.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        // GET: AppointmentAdm/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var apiResult = await _appointmentApiClient.GetByIdAsync(id);

            if (!apiResult.Success || apiResult.Data == null)
            {
                ViewBag.ErrorMessage = apiResult.ErrorMessage ?? "No se pudo obtener la cita desde la API.";
                return View();
            }

            var viewModel = AppointmentDetailsViewModel.FromDto(apiResult.Data);
            return View(viewModel);
        }

        // GET: AppointmentAdm/Create
        public async Task<ActionResult> Create()
        {
            var viewModel = new CreateAppointmentViewModel
            {
                Patients = await GetPatientsList(),
                Doctors = await GetDoctorsList(),
                AppointmentDate = DateTime.Now.AddDays(1).Date.AddHours(9)
            };

            return View(viewModel);
        }

        // POST: AppointmentAdm/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateAppointmentViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    viewModel.Patients = await GetPatientsList();
                    viewModel.Doctors = await GetDoctorsList();
                    return View(viewModel);
                }

                var dto = viewModel.ToDto();
                var apiResult = await _appointmentApiClient.CreateAsync(dto);

                if (!apiResult.Success)
                {
                    ViewBag.ErrorMessage = apiResult.ErrorMessage ?? "Error al crear la cita en la API.";
                    viewModel.Patients = await GetPatientsList();
                    viewModel.Doctors = await GetDoctorsList();
                    return View(viewModel);
                }

                TempData["SuccessMessage"] = "Cita creada correctamente (API).";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error al crear cita: {ex.Message}";
                viewModel.Patients = await GetPatientsList();
                viewModel.Doctors = await GetDoctorsList();
                return View(viewModel);
            }
        }

        // GET: AppointmentAdm/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var apiResult = await _appointmentApiClient.GetByIdAsync(id);

            if (!apiResult.Success || apiResult.Data == null)
            {
                ViewBag.ErrorMessage = apiResult.ErrorMessage ?? "No se pudo obtener la cita desde la API.";
                return View();
            }

            var viewModel = EditAppointmentViewModel.FromDto(apiResult.Data);
            viewModel.Statuses = GetStatusesList();

            return View(viewModel);
        }

        // POST: AppointmentAdm/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EditAppointmentViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    viewModel.Statuses = GetStatusesList();
                    return View(viewModel);
                }

                var dto = viewModel.ToDto();
                var apiResult = await _appointmentApiClient.UpdateAsync(dto);

                if (!apiResult.Success)
                {
                    ViewBag.ErrorMessage = apiResult.ErrorMessage ?? "Error al actualizar la cita en la API.";
                    viewModel.Statuses = GetStatusesList();
                    return View(viewModel);
                }

                TempData["SuccessMessage"] = "Cita actualizada correctamente (API).";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error al actualizar cita: {ex.Message}";
                viewModel.Statuses = GetStatusesList();
                return View(viewModel);
            }
        }

        // POST: AppointmentAdm/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            var apiResult = await _appointmentApiClient.DeleteAsync(id);

            if (!apiResult.Success)
            {
                TempData["ErrorMessage"] = apiResult.ErrorMessage ?? "No se pudo eliminar la cita en la API.";
            }
            else
            {
                TempData["SuccessMessage"] = "Cita eliminada correctamente (API).";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: AppointmentAdm/Confirm/5
        public async Task<ActionResult> Confirm(int id)
        {
            var result = await _appointmentService.ConfirmAsync(id);

            if (result.Exitoso)
            {
                TempData["SuccessMessage"] = "Cita confirmada correctamente";
            }
            else
            {
                TempData["ErrorMessage"] = result.Mensaje;
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: AppointmentAdm/Cancel/5
        public async Task<ActionResult> Cancel(int id)
        {
            var apiResult = await _appointmentApiClient.GetByIdAsync(id);

            if (!apiResult.Success || apiResult.Data == null)
            {
                TempData["ErrorMessage"] = apiResult.ErrorMessage ?? "No se pudo obtener la cita desde la API.";
                return RedirectToAction(nameof(Index));
            }

            var viewModel = AppointmentDetailsViewModel.FromDto(apiResult.Data);
            return View(viewModel);
        }

        // POST: AppointmentAdm/CancelConfirmed/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CancelConfirmed(int id)
        {
            try
            {
                var result = await _appointmentService.CancelAsync(id);

                if (!result.Exitoso)
                {
                    TempData["ErrorMessage"] = result.Mensaje;
                    return RedirectToAction(nameof(Index));
                }

                TempData["SuccessMessage"] = "Cita cancelada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cancelar cita: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Metodos auxiliares para cargar combos
        private async Task<List<PatientSelectViewModel>> GetPatientsList()
        {
            var result = await _patientService.GetActiveAsync();

            if (!result.Exitoso || result.Datos == null)
                return new List<PatientSelectViewModel>();

            return result.Datos.Select(p => new PatientSelectViewModel
            {
                PatientId = p.PatientId,
                FullName = p.FullName,
                IdentificationNumber = p.IdentificationNumber
            }).ToList();
        }

        private async Task<List<DoctorSelectViewModel>> GetDoctorsList()
        {
            var result = await _doctorService.GetActiveDoctorsAsync();

            if (!result.Exitoso || result.Datos == null)
                return new List<DoctorSelectViewModel>();

            return result.Datos.Select(d => new DoctorSelectViewModel
            {
                DoctorId = d.DoctorId,
                FullName = d.FullName,
                SpecialtyName = d.SpecialtyName
            }).ToList();
        }

        private List<StatusSelectViewModel> GetStatusesList()
        {
            return new List<StatusSelectViewModel>
            {
                new StatusSelectViewModel { StatusId = 1, StatusName = "Pendiente" },
                new StatusSelectViewModel { StatusId = 2, StatusName = "Confirmada" },
                new StatusSelectViewModel { StatusId = 3, StatusName = "Cancelada" },
                new StatusSelectViewModel { StatusId = 4, StatusName = "Completada" }
            };
        }
    }
}
