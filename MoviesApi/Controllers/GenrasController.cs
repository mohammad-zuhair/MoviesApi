using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Dtos;
using MoviesApi.Models;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenrasController : ControllerBase
    {

        private readonly IGenresService _genresService;
        public GenrasController(IGenresService genresService)
        {
            _genresService = genresService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var genras = await _genresService.GetAll();
            return Ok(genras);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync( [FromBody] GenraDto dto)
        {
            var genre = new Genre { Name = dto.Name };

            //await _context.AddAsync(genra); // cuase the object type is genra 
            //_context.SaveChanges();

            await _genresService.Add(genre);

            return Ok(genre);
        }

        [HttpPut("{id}")]
        //api/Genra/1
        public async Task<IActionResult> UpdateAsync(byte id ,[FromBody] GenraDto dto)
        {
            var genre = await _genresService.GetById(id);

            if (genre == null)
                return NotFound($"No Genra was Found with ID :{id}");

            genre.Name = dto.Name;
            _genresService.Update(genre);
            return Ok(genre);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(byte id)
        {
            var genre = await _genresService.GetById(id);

            if (genre == null)
                return NotFound($"No Genra was Found with ID :{id}");


            _genresService.Delete(genre);
            return Ok(genre);
        }

    }
}
