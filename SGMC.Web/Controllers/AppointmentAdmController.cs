using Microsoft.AspNetCore.Mvc;
using SGMC.Application.Dto.Appointments;
using SGMC.Application.Interfaces.Service;
using SGMC.Web.Models.Appointment;

namespace SGMC.Web.Controllers
{
    public class AppointmentAdmController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;

        public AppointmentAdmController(
            IAppointmentService appointmentService,
            IPatientService patientService,
            IDoctorService doctorService)
        {
            _appointmentService = appointmentService;
            _patientService = patientService;
            _doctorService = doctorService;
        }

        // GET: AppointmentAdmController
        public async Task<ActionResult> Index([FromQuery] AppointmentFilterDto filter)
        {
            Console.WriteLine("🔍 Controller Index iniciado con filtros...");

            // 1. Llamar al nuevo servicio de filtrado
            var result = await _appointmentService.GetFilteredAppointmentsAsync(filter);

            Console.WriteLine($"📊 Result.Exitoso: {result.Exitoso}");
            Console.WriteLine($"📊 Result.Datos Count: {result.Datos?.Count ?? 0}");

            if (!result.Exitoso)
            {
                ViewBag.ErrorMessage = result.Mensaje;
                // Devolver el ViewModel vacío pero con las listas de filtros
                var emptyViewModel = new AppointmentIndexViewModel
                {
                    Filter = filter,
                    Patients = await GetPatientsList(),
                    Doctors = await GetDoctorsList(),
                    Statuses = GetStatusesList()
                };
                return View(emptyViewModel);
            }

            // 2. Mapear la lista de citas
            var appointmentsList = result.Datos?.Select(AppointmentListViewModel.FromDto).ToList()
                                  ?? new List<AppointmentListViewModel>();

            // 3. Crear el ViewModel principal
            var viewModel = new AppointmentIndexViewModel
            {
                Filter = filter,
                Appointments = appointmentsList,
                Patients = await GetPatientsList(),
                Doctors = await GetDoctorsList(),
                Statuses = GetStatusesList()
            };

            Console.WriteLine($"✅ ViewModel Count: {viewModel.Appointments.Count}");

            return View(viewModel);
        }

        // GET: /AppointmentAdm/TestDb - Endpoint de prueba temporal
        public async Task<IActionResult> TestDb()
        {
            try
            {
                Console.WriteLine("🧪 TestDb iniciado...");

                var directQuery = await _appointmentService.GetAllAsync();

                var debugInfo = new
                {
                    Exitoso = directQuery.Exitoso,
                    Mensaje = directQuery.Mensaje,
                    TotalCitas = directQuery.Datos?.Count ?? 0,
                    Citas = directQuery.Datos?.Select(d => new {
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

        // GET: AppointmentAdmController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            var result = await _appointmentService.GetByIdAsync(id);

            if (!result.Exitoso)
            {
                ViewBag.ErrorMessage = result.Mensaje;
                return View();
            }

            var viewModel = AppointmentDetailsViewModel.FromDto(result.Datos!);
            return View(viewModel);
        }

        // GET: AppointmentAdmController/Create
        public async Task<ActionResult> Create()
        {
            var viewModel = new CreateAppointmentViewModel
            {
                Patients = await GetPatientsList(),
                Doctors = await GetDoctorsList(),
                AppointmentDate = DateTime.Now.AddDays(1).Date.AddHours(9) // 9:00 AM mañana
            };

            return View(viewModel);
        }

        // POST: AppointmentAdmController/Create
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
                var result = await _appointmentService.CreateAsync(dto);

                if (!result.Exitoso)
                {
                    ViewBag.ErrorMessage = result.Mensaje;
                    viewModel.Patients = await GetPatientsList();
                    viewModel.Doctors = await GetDoctorsList();
                    return View(viewModel);
                }

                TempData["SuccessMessage"] = "Cita creada correctamente";
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

        // GET: AppointmentAdmController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            var result = await _appointmentService.GetByIdAsync(id);

            if (!result.Exitoso)
            {
                ViewBag.ErrorMessage = result.Mensaje;
                return View();
            }

            var viewModel = EditAppointmentViewModel.FromDto(result.Datos!);
            viewModel.Statuses = GetStatusesList();

            return View(viewModel);
        }

        // POST: AppointmentAdmController/Edit/5
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
                var result = await _appointmentService.UpdateAsync(dto);

                if (!result.Exitoso)
                {
                    ViewBag.ErrorMessage = result.Mensaje;
                    viewModel.Statuses = GetStatusesList();
                    return View(viewModel);
                }

                TempData["SuccessMessage"] = "Cita actualizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Error al actualizar cita: {ex.Message}";
                viewModel.Statuses = GetStatusesList();
                return View(viewModel);
            }
        }

        // GET: AppointmentAdmController/Confirm/5
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

        // GET: AppointmentAdmController/Cancel/5
        public async Task<ActionResult> Cancel(int id)
        {
            var result = await _appointmentService.GetByIdAsync(id);

            if (!result.Exitoso)
            {
                TempData["ErrorMessage"] = result.Mensaje;
                return RedirectToAction(nameof(Index));
            }

            var viewModel = AppointmentDetailsViewModel.FromDto(result.Datos!);
            return View(viewModel);
        }

        // POST: AppointmentAdmController/Cancel/5
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

        // Métodos auxiliares
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