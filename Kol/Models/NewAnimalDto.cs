namespace Kol_2024.Models;

public class NewAnimalDto
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime AdmissionDate { get; set; }
    public int OwnerId { get; set; }
    public List<NewProcedureDto> Procedures { get; set; }
}

public class NewProcedureDto
{
    public int ProcedureId { get; set; }
    public DateTime Date { get; set; }
}