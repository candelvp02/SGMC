using Microsoft.AspNetCore.Mvc;
using SGMC.Application.Dto.Users;
using SGMC.Application.Interfaces.Service;
using SGMC.Domain.Base;

namespace SGMC.Web.Controllers
{
    public class DoctorController : Controller
    {
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        // GET: Doctor
        public async Task<ActionResult> Index()
        {
            OperationResult<List<DoctorDto>> result = await _doctorService.GetAllAsync();

            if (!result.Exitoso)
            {
                ViewBag.ErrorMessage = result.Mensaje;
                return View(new List<DoctorDto>());
            }

            return View(result.Datos);
        }

        // GET: Doctor/Details/5
        public async Task<ActionResult> Details(int id)
        {
            OperationResult<DoctorDto> result = await _doctorService.GetByIdWithDetailsAsync(id);

            if (!result.Exitoso)
            {
                ViewBag.ErrorMessage = result.Mensaje;
                return View();
            }

            return View(result.Datos);
        }

        // GET: Doctor/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Doctor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(RegisterDoctorDto registerDoctorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(registerDoctorDto);
                }

                OperationResult<DoctorDto> result = await _doctorService.CreateAsync(registerDoctorDto);

                if (!result.Exitoso)
                {
                    ViewBag.ErrorMessage = result.Mensaje;
                    return View(registerDoctorDto);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(registerDoctorDto);
            }
        }

        // GET: Doctor/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            OperationResult<DoctorDto> result = await _doctorService.GetByIdAsync(id);

            if (!result.Exitoso || result.Datos == null)
            {
                ViewBag.ErrorMessage = result.Mensaje;
                return View();
            }

            int doctorId = result.Datos.DoctorId;
            var updateDto = new UpdateDoctorDto
            {
                DoctorId = doctorId,
                PhoneNumber = result.Datos.PhoneNumber,
                YearsOfExperience = result.Datos.YearsOfExperience,
                Education = result.Datos.Education,
                Bio = result.Datos.Bio,
                ConsultationFee = result.Datos.ConsultationFee,
                ClinicAddress = result.Datos.ClinicAddress,
                LicenseExpirationDate = result.Datos.LicenseExpirationDate
            };

            return View(updateDto);
        }

        // POST: Doctor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UpdateDoctorDto updateDoctorDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(updateDoctorDto);
                }

                OperationResult<DoctorDto> result = await _doctorService.UpdateAsync(updateDoctorDto);

                if (!result.Exitoso)
                {
                    ViewBag.ErrorMessage = result.Mensaje;
                    return View(updateDoctorDto);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(updateDoctorDto);
            }
        }

        // GET: Doctor/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            OperationResult<DoctorDto> result = await _doctorService.GetByIdAsync(id);

            if (!result.Exitoso)
            {
                ViewBag.ErrorMessage = result.Mensaje;
                return RedirectToAction(nameof(Index));
            }

            return View(result.Datos);
        }

        // POST: Doctor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                OperationResult result = await _doctorService.DeleteAsync(id);

                if (!result.Exitoso)
                {
                    ViewBag.ErrorMessage = result.Mensaje;
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}