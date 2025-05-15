using Kol.Middlewares;
using KoL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Kol.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimalsController(IRepository repository) : ControllerBase 
{
    private readonly IRepository _repository = repository;
    
    

    // [HttpGet("{animalId:int}")]
    // public async Task<IActionResult> GetAnimal(int animalId)
    // {
    //     if(! await _repository.CheckIfAnimalExists(animalId))
    //         throw new NotFoundException($"Animal - {animalId} not found");
    //     var animal = await _repository.GetAnimal(animalId);
    //     return Ok(animal);
    // }
    //
    // [HttpPost]
    // public async Task<IActionResult> AddAnimal([FromBody] NewAnimalDto animal)
    // {
    //     var id = await _repository.AddAnimal(animal);
    //     return Created($"api/animals/{id}", animal); 
    // }
    
}