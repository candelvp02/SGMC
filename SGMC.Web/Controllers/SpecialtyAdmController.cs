using Microsoft.AspNetCore.Mvc;
using SGMC.Application.Dto.Medical;
using SGMC.Application.Interfaces.Service;

namespace SGMC.Web.Controllers
{
    public class SpecialtyAdmController : Controller
    {
        private readonly ISpecialtyService _specialtyService;

        public SpecialtyAdmController(ISpecialtyService specialtyService)
        {
            _specialtyService = specialtyService;
        }

        // GET: SpecialtyAdm
        public async Task<ActionResult> Index()
        {
            var result = await _specialtyService.GetAllAsync();
            if (!result.Exitoso)
            {
                ViewBag.ErrorMessage = result.Mensaje;
                return View(new List<SpecialtyDto>());
            }
            return View(result.Datos);
        }

        // GET: SpecialtyAdm/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SpecialtyAdm/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateSpecialtyDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(dto);
                }

                var result = await _specialtyService.CreateAsync(dto);

                if (!result.Exitoso)
                {
                    ViewBag.ErrorMessage = result.Mensaje;
                    return View(dto);
                }

                TempData["SuccessMessage"] = "Especialidad creada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error creando la especialidad: " + ex.Message;
                return View(dto);
            }
        }

        // GET: SpecialtyAdm/Edit/5
        public async Task<ActionResult> Edit(short id)
        {
            var result = await _specialtyService.GetByIdAsync(id);
            if (!result.Exitoso || result.Datos == null)
            {
                TempData["ErrorMessage"] = result.Mensaje;
                return RedirectToAction(nameof(Index));
            }

            // Mapeamos del DTO al UpdateDTO
            var updateDto = new UpdateSpecialtyDto
            {
                SpecialtyId = result.Datos.SpecialtyId,
                SpecialtyName = result.Datos.SpecialtyName,
                IsActive = result.Datos.IsActive
            };

            return View(updateDto);
        }

        // POST: SpecialtyAdm/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(short id, UpdateSpecialtyDto dto)
        {
            if (id != dto.SpecialtyId)
            {
                ViewBag.ErrorMessage = "El ID no coincide.";
                return View(dto);
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    return View(dto);
                }

                var result = await _specialtyService.UpdateAsync(dto);

                if (!result.Exitoso)
                {
                    ViewBag.ErrorMessage = result.Mensaje;
                    return View(dto);
                }

                TempData["SuccessMessage"] = "Especialidad actualizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error actualizando la especialidad: " + ex.Message;
                return View(dto);
            }
        }


        // GET: SpecialtyAdm/Delete/5
        public async Task<ActionResult> Delete(short id)
        {
            var result = await _specialtyService.GetByIdAsync(id);
            if (!result.Exitoso || result.Datos == null)
            {
                TempData["ErrorMessage"] = result.Mensaje;
                return RedirectToAction(nameof(Index));
            }
            return View(result.Datos);
        }

        // POST: SpecialtyAdm/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(short id)
        {
            try
            {
                var result = await _specialtyService.DeleteAsync(id);

                if (!result.Exitoso)
                {
                    TempData["ErrorMessage"] = result.Mensaje;
                }
                else
                {
                    TempData["SuccessMessage"] = "Especialidad eliminada correctamente";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error eliminando la especialidad: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
}