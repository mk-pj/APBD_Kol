using Kol.Middlewares;
using Kol.Models;
using KoL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Kol.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController(IRepository repository) : ControllerBase 
{
    private readonly IRepository _repository = repository;
    
    [HttpGet("{appointmentId:int}")]
    public async Task<IActionResult> GetAppointment(int appointmentId)
    {
        if(! await _repository.CheckIfAppointmentExists(appointmentId))
            throw new NotFoundException($"Appointment - {appointmentId} not found");
        var appointment = await _repository.GetAppointment(appointmentId);
        return Ok(appointment);
    }
    
    [HttpPost]
    public async Task<IActionResult> AddAppointment([FromBody] NewAppointmentDto appointment)
    {
        if (await _repository.CheckIfAppointmentExists(appointment.AppointmentId))
            throw new ConflictException($"Appointment {appointment.AppointmentId} already exists");
        if (!await _repository.CheckIfDoctorExists(appointment.Pwz))
            throw new NotFoundException($"Doctor {appointment.Pwz} does not exist");
        if (!await _repository.CheckIfPatientExists(appointment.PatientId))
            throw new NotFoundException($"Patient {appointment.PatientId} does not exist");
        var id = await _repository.AddAppointment(appointment);
        return Created($"api/appointments/{id}", appointment); 
    }
    
}