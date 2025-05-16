using Kol.Middlewares;
using Kol.Models;
using Microsoft.Data.SqlClient;

namespace KoL.Repositories;

public class Repository(IConfiguration configuration) : IRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("Default") ?? string.Empty;
    
    public async Task<bool> CheckIfAppointmentExists(int appointmentId)
    {
        const string query = "SELECT 1 FROM Appointment a WHERE a.appoitment_id  = @AppointmentId;";
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        
        await using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
    
        var res = await cmd.ExecuteScalarAsync();
        return res is not null;
    }
    
    public async Task<AppointmentDto> GetAppointment(int appointmentId)
    {
        const string query = @"
            SELECT a.date, p.first_name, p.last_name, p.date_of_birth, d.doctor_id, d.pwz, s.name, ap_s.service_fee
            FROM Appointment a 
            LEFT JOIN Patient p ON a.patient_id = p.patient_id
            LEFT JOIN Doctor d ON a.doctor_id = d.doctor_id
            LEFT JOIN Appointment_Service ap_s ON a.appoitment_id = ap_s.appoitment_id
            LEFT JOIN Service s ON ap_s.service_id = s.service_id
            WHERE a.appoitment_id  = @AppointmentId;";
        
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        
        await using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@AppointmentId", appointmentId);
    
        AppointmentDto? appointment = null;
        
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                appointment ??= new AppointmentDto()
                {
                    Date = reader.GetDateTime(reader.GetOrdinal("date")),
                    Patient = new PatientDto()
                    {
                        FirstName = reader.GetString(reader.GetOrdinal("first_name")),
                        LastName = reader.GetString(reader.GetOrdinal("last_name")),
                        DateOfBirth = reader.GetDateTime(reader.GetOrdinal("date_of_birth")),
                    },
                    Doctor = new DoctorDto()
                    {
                        DoctorId = reader.GetInt32(reader.GetOrdinal("doctor_id")),
                        Pwz = reader.GetString(reader.GetOrdinal("pwz")),
                    },
                    AppointmentServices = []
                };
                
                var serviceName = reader.GetOrdinal("name");
                
               if(reader.IsDBNull(serviceName))
                   continue;
    
               appointment.AppointmentServices.Add(new ServiceDto()
               {
                   Name = reader.GetString(serviceName),
                   ServiceFee = reader.GetDecimal(reader.GetOrdinal("service_fee"))
               });
            }
        }
        
        return appointment ?? throw new NotFoundException("Appointment not found");
    }
    
    public async Task<bool> CheckIfPatientExists(int patientId)
    {
        const string query = "SELECT 1 FROM Patient p WHERE p.patient_id = @PatientId;";
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        
        await using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@PatientId", patientId);
        
        var res = await cmd.ExecuteScalarAsync();
        return res is not null;
    }
    
    public async Task<bool> CheckIfDoctorExists(string pwz)
    {
        const string query = "SELECT 1 FROM Doctor d WHERE d.pwz = @Pwz;";
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        
        await using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Pwz", pwz);
        
        var res = await cmd.ExecuteScalarAsync();
        return res is not null;
    }
    
    
    private async Task<bool> CheckIfServiceExists(string name)
    {
        const string query = "SELECT 1 FROM Service s WHERE s.name = @Name;";
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        
        await using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Name", name);
        
        var res = await cmd.ExecuteScalarAsync();
        return res is not null;
    }
    
    private async Task<int> GetDoctorId(string pwz)
    {
        const string query = "SELECT d.doctor_id FROM Doctor d WHERE d.pwz = @Pwz;";
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        
        await using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Pwz", pwz);
        
        var res = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(res);
    }

    private async Task<int> GetServiceId(string name)
    {
        const string query = "SELECT s.service_id FROM Service s WHERE s.name = @Name;";
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        
        await using var cmd = new SqlCommand(query, conn);
        cmd.Parameters.AddWithValue("@Name", name);
        
        var res = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(res);
    }
    
    public async Task<int> AddAppointment(NewAppointmentDto appointment)
    {
        const string query = @"
            INSERT INTO Appointment(appoitment_id, date, patient_id, doctor_id)
            VALUES (@AppointmentId, @Date, @PatientId, @DoctorId);";
        
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        
        var transaction = conn.BeginTransaction();
        
        await using var cmd = new SqlCommand(query, conn, transaction);
        
        try
        {
            var doctorId = await GetDoctorId(appointment.Pwz);
            cmd.Parameters.AddWithValue("@AppointmentId", appointment.AppointmentId);
            cmd.Parameters.AddWithValue("@Date", DateTime.Now);
            cmd.Parameters.AddWithValue("@PatientId", appointment.PatientId);
            cmd.Parameters.AddWithValue("@DoctorId", doctorId);
            
            await cmd.ExecuteNonQueryAsync();
            
            foreach (var service in appointment.Services)
            {
                if (!await CheckIfServiceExists(service.ServiceName)) 
                    throw new ArgumentException("Service not found");
                
                var serviceId = await GetServiceId(service.ServiceName);
                cmd.Parameters.Clear();
                cmd.CommandText = @"
                        INSERT INTO Appointment_Service(appoitment_id, service_id, service_fee)
                        VALUES(@AppointmentId, @ServiceId, @ServiceFee);";
                cmd.Parameters.AddWithValue("@AppointmentId", appointment.AppointmentId);
                cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                cmd.Parameters.AddWithValue("@ServiceFee", service.ServiceFee);
                await cmd.ExecuteNonQueryAsync();
            }
            await transaction.CommitAsync();
            return appointment.AppointmentId;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
        
    }
    
}