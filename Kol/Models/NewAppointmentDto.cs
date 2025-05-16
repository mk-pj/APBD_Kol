namespace Kol.Models;

public class NewAppointmentDto
{
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public string Pwz { get; set; } = string.Empty;
    public List<ProvidedServiceDto> Services { get; set; } = [];
}

public class ProvidedServiceDto
{
    public string ServiceName { get; set; } = string.Empty;
    public decimal ServiceFee { get; set; }
}