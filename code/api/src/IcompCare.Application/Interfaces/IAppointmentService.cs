using IcompCare.Application.DTOs;
using IcompCare.Application.DTOs.Appointments;

namespace IcompCare.Application.Interfaces;

public interface IAppointmentService
{
    Task<PagedResult<AppointmentDto>> GetAllAsync(int pageNumber, int pageSize);
    Task<AppointmentDto> GetByIdAsync(Guid id);
    Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto);
    Task<AppointmentDto> UpdateAsync(Guid id, UpdateAppointmentDto dto);
    Task DeleteAsync(Guid id);
    Task<PagedResult<AppointmentDto>> GetByProfessionalIdAsync(
        Guid professionalId,
        int pageNumber = 1,
        int pageSize = 20,
        string? status = null,
        Guid? sessionTypeId = null,
        string? search = null
    );
}
