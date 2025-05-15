using Kol.Middlewares;
using Microsoft.Data.SqlClient;

namespace KoL.Repositories;

public class Repository(IConfiguration configuration) : IRepository
{
    private readonly string _connectionString = configuration.GetConnectionString("Default") ?? string.Empty;
    

    // public async Task<bool> CheckIfAnimalExists(int animalId)
    // {
    //     const string query = "SELECT 1 FROM Animal WHERE Animal.ID = @animalId;";
    //     await using var conn = new SqlConnection(_connectionString);
    //     await conn.OpenAsync();
    //     
    //     await using var cmd = new SqlCommand(query, conn);
    //     cmd.Parameters.AddWithValue("@animalId", animalId);
    //
    //     var res = await cmd.ExecuteScalarAsync();
    //     return res is not null;
    // }
    //
    // public async Task<AnimalDto> GetAnimal(int animalId)
    // {
    //     const string query = @"
    //         SELECT a.ID AS AnimalId, a.Name AS AnimalName, a.Type, a.AdmissionDate, o.ID AS OwnerId, 
    //                o.FirstName, o.LastName, p.Name AS ProcedureName, p.Description, pa.Date
    //         FROM Animal a
    //         LEFT JOIN Owner o ON a.Owner_ID = o.ID
    //         LEFT JOIN Procedure_Animal pa ON a.ID = pa.Animal_ID
    //         LEFT JOIN [Procedure] p ON pa.Procedure_ID = p.ID
    //         WHERE a.ID = @AnimalId;";
    //     
    //     await using var conn = new SqlConnection(_connectionString);
    //     await conn.OpenAsync();
    //     
    //     await using var cmd = new SqlCommand(query, conn);
    //     cmd.Parameters.AddWithValue("@AnimalId", animalId);
    //
    //     AnimalDto? animal = null;
    //     
    //     await using (var reader = await cmd.ExecuteReaderAsync())
    //     {
    //         while (await reader.ReadAsync())
    //         {
    //             animal ??= new AnimalDto()
    //             {
    //                 Id = reader.GetInt32(reader.GetOrdinal("AnimalId")),
    //                 Name = reader.GetString(reader.GetOrdinal("AnimalName")),
    //                 Type = reader.GetString(reader.GetOrdinal("Type")),
    //                 AdmissionDate = reader.GetDateTime(reader.GetOrdinal("AdmissionDate")),
    //                 Owner = new OwnerDto()
    //                 {
    //                     Id = reader.GetInt32(reader.GetOrdinal("OwnerId")),
    //                     FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
    //                     LastName = reader.GetString(reader.GetOrdinal("LastName")),
    //                 },
    //                 Procedures = []
    //             };
    //             
    //             var procedureOrdinal = reader.GetOrdinal("ProcedureName");
    //             
    //            if(reader.IsDBNull(procedureOrdinal))
    //                continue;
    //
    //            animal.Procedures.Add(new ProcedureDto()
    //            {
    //                Name = reader.GetString(procedureOrdinal),
    //                Description = reader.GetString(reader.GetOrdinal("Description")),
    //                Date = reader.GetDateTime(reader.GetOrdinal("Date")),
    //            });
    //         }
    //     }
    //     
    //     return animal ?? throw new NotFoundException("Animal not found");
    // }
    //
    // private async Task<bool> CheckIfProcedureExists(int procedureId)
    // {
    //     const string query = "SELECT 1 FROM [Procedure] p WHERE p.ID = @ProcedureId;";
    //     await using var conn = new SqlConnection(_connectionString);
    //     await conn.OpenAsync();
    //     
    //     await using var cmd = new SqlCommand(query, conn);
    //     cmd.Parameters.AddWithValue("@ProcedureId", procedureId);
    //     
    //     var res = await cmd.ExecuteScalarAsync();
    //     return res is not null;
    // }
    //
    //
    // private async Task<bool> CheckIfOwnerExists(int ownerId)
    // {
    //     const string query = "SELECT 1 FROM Owner WHERE ID = @OwnerId;";
    //     await using var conn = new SqlConnection(_connectionString);
    //     await conn.OpenAsync();
    //     
    //     await using var cmd = new SqlCommand(query, conn);
    //     cmd.Parameters.AddWithValue("@OwnerId", ownerId);
    //     
    //     var res = await cmd.ExecuteScalarAsync();
    //     return res is not null;
    // }
    //
    //
    // public async Task<int> AddAnimal(NewAnimalDto animal)
    // {
    //     const string insertAnimal = @"
    //         INSERT INTO Animal(Name, Type, AdmissionDate, Owner_ID) 
    //         VALUES(@Name, @Type, @AdmissionDate, @OwnerId);
    //         SELECT CAST(SCOPE_IDENTITY() AS INT);";
    //     await using var conn = new SqlConnection(_connectionString);
    //     await conn.OpenAsync();
    //     
    //     var transaction = conn.BeginTransaction();
    //     
    //     await using var cmd = new SqlCommand(insertAnimal, conn, transaction);
    //     
    //     cmd.Parameters.AddWithValue("@Name", animal.Name);
    //     cmd.Parameters.AddWithValue("@Type", animal.Type);
    //     cmd.Parameters.AddWithValue("@AdmissionDate", animal.AdmissionDate);
    //     cmd.Parameters.AddWithValue("@OwnerId", animal.OwnerId);
    //
    //     try
    //     {
    //         if (!await CheckIfOwnerExists(animal.OwnerId))
    //             throw new ArgumentException("Owner not found");
    //         
    //         int animalId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
    //         foreach (var procedure in animal.Procedures)
    //         {
    //             if (await CheckIfProcedureExists(procedure.ProcedureId))
    //             {
    //                 cmd.Parameters.Clear();
    //                 cmd.CommandText = @"INSERT INTO Procedure_Animal(Procedure_ID, Animal_ID, Date)
    //                                     VALUES(@ProcedureId, @AnimalId, @Date);";
    //                 cmd.Parameters.AddWithValue("@ProcedureId", procedure.ProcedureId);
    //                 cmd.Parameters.AddWithValue("@AnimalId", animalId);
    //                 cmd.Parameters.AddWithValue("@Date", procedure.Date);
    //                 await cmd.ExecuteNonQueryAsync();
    //             }
    //         }
    //         await transaction.CommitAsync();
    //         return animalId;
    //     }
    //     catch (Exception)
    //     {
    //         await transaction.RollbackAsync();
    //         throw;
    //     }
    //     
    // }
    
}