
using Kol.Models;

namespace KoL.Repositories;

public interface IRepository
{
    public Task<bool> CheckIfAppointmentExists(int appointmentId);
    public Task<bool> CheckIfPatientExists(int patientId);
    public Task<bool> CheckIfDoctorExists(string pwz);

    public Task<AppointmentDto> GetAppointment(int appointmentId);

    public Task<int> AddAppointment(NewAppointmentDto appointment);
}