using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Appointments;
using IcompCare.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace IcompCare.Api.Controllers;

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
    public async Task<ActionResult<PagedResult<AppointmentDto>>> GetAll(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10
    )
    {
        var result = await _appointmentService.GetAllAsync(pageNumber, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDto>> GetById(Guid id)
    {
        var result = await _appointmentService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<AppointmentDto>> Create(CreateAppointmentDto dto)
    {
        var result = await _appointmentService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<AppointmentDto>> Update(Guid id, UpdateAppointmentDto dto)
    {
        var result = await _appointmentService.UpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _appointmentService.DeleteAsync(id);
        return NoContent();
    }
}
