using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private new List<string> _allowedExtensions = new List<string> { ".jpg", ".png" };
        private long _maxAllowedPosterSize = 1048576;

        private readonly IMoviesService _moviesService ;
        private readonly IGenresService _genresService;
        private readonly IMapper _mapper;

        public MoviesController(IMoviesService moviesService , IGenresService genresService, IMapper mapper)
        {
            _moviesService = moviesService;
            _genresService = genresService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            //var movies = await _context.Movies.Include(g=>g.Genre).ToListAsync(); 
            // another way 

            var movies = await _moviesService.GetAll();
            var result = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies); 
            return Ok(result);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetByIdAsync(int Id)
        {
            var movie = await _moviesService.GetById(Id);

            if (movie == null)
                return NotFound();

            var result = _mapper.Map<MovieDetailsDto>(movie); 
            return Ok(result);
        }

        [HttpGet("GetByGenreIdAsync")]

        public async Task<IActionResult> GetByGenreIdAsync(byte genreId)
        {
            var movies = await _moviesService.GetAll(genreId);
            var result = _mapper.Map<IEnumerable<MovieDetailsDto>>(movies);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] MovieDto dto)
        {

            if (dto.Poster is null)
                return BadRequest("Poster is required");

            if (!_allowedExtensions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                return BadRequest("Only .png and jpg images are allowed !!");

            if (dto.Poster.Length > _maxAllowedPosterSize)
                return BadRequest("Max Allowed Size For Poster Is 1MB!!");

            var isValidGenre = await _genresService.IsValidGenre(dto.GenreId);

            if (!isValidGenre)
                return BadRequest("Invalid Genre Id");


            using var dataStream = new MemoryStream();
            await dto.Poster.CopyToAsync(dataStream);


            var movie = _mapper.Map<Movie>(dto);
            movie.Poster =dataStream.ToArray(); 

            await _moviesService.Add(movie);

            return Ok(movie);
        }


        [HttpPut("{id}")]

        public async Task<IActionResult> UpdateAsync(int id , [FromForm] MovieDto dto)
        {
            var movie = await _moviesService.GetById(id);

            if (movie == null)
                return NotFound($"No Movie found with Id : {id}");

            var isValidGenre = await _genresService.IsValidGenre(dto.GenreId);

            if (!isValidGenre)
                return BadRequest("Invalid Genre Id");

            if (dto.Poster != null )
            {
                if (!_allowedExtensions.Contains(Path.GetExtension(dto.Poster.FileName).ToLower()))
                    return BadRequest("Only .png and jpg images are allowed !!");

                if (dto.Poster.Length > _maxAllowedPosterSize)
                    return BadRequest("Max Allowed Size For Poster Is 1MB!!");

                using var dataStream = new MemoryStream();
                await dto.Poster.CopyToAsync(dataStream);

                movie.Poster = dataStream.ToArray();
            }

            movie.GenreId=dto.GenreId;
            movie.Title=dto.Title;
            movie.Year=dto.Year;
            movie.Rate=dto.Rate;
            movie.StoreLine=dto.StoreLine;

            _moviesService.Update(movie);
            return Ok(movie);


        }
        [HttpDelete("{id}")]

        public async Task <IActionResult> DeleteAsync(int id )
        {


            var movie = await _moviesService.GetById(id);

            if (movie == null) 
                return NotFound($"No Movie found with Id : {id}");

            _moviesService.Delete(movie);

            return Ok(movie);
        }
    }
}
