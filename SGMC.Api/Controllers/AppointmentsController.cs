using Microsoft.AspNetCore.Mvc;
using SGMC.Application.Dto.Appointments;
using SGMC.Application.Interfaces.Service;
using SGMC.Domain.Base;

namespace SGMC.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public async Task<ActionResult<OperationResult<List<AppointmentDto>>>> GetAll()
        {
            var result = await _appointmentService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OperationResult<AppointmentDto>>> GetById(int id)
        {
            var result = await _appointmentService.GetByIdAsync(id);
            if (!result.Exitoso)
                return NotFound(result);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<OperationResult<AppointmentDto>>> Create([FromBody] CreateAppointmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(OperationResult.Fallo("Datos inválidos"));

            var result = await _appointmentService.CreateAsync(dto);
            if (!result.Exitoso)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetById), new { id = result.Datos?.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<OperationResult<AppointmentDto>>> Update(int id, [FromBody] UpdateAppointmentDto dto)
        {
            if (id != dto.Id)
                return BadRequest(OperationResult.Fallo("ID no coincide"));

            if (!ModelState.IsValid)
                return BadRequest(OperationResult.Fallo("Datos inválidos"));

            var result = await _appointmentService.UpdateAsync(dto);
            if (!result.Exitoso)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<OperationResult>> Delete(int id)
        {
            var result = await _appointmentService.DeleteAsync(id);
            if (!result.Exitoso)
                return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<OperationResult<List<AppointmentDto>>>> GetByPatient(int patientId)
        {
            var result = await _appointmentService.GetByPatientIdAsync(patientId);
            return Ok(result);
        }

        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<OperationResult<List<AppointmentDto>>>> GetByDoctor(int doctorId)
        {
            var result = await _appointmentService.GetByDoctorIdAsync(doctorId);
            return Ok(result);
        }
    }
}