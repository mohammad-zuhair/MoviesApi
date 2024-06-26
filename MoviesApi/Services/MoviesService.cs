
using Microsoft.EntityFrameworkCore;
using MoviesApi.Models;

namespace MoviesApi.Services
{
    public class MoviesService : IMoviesService
    {

        private readonly ApplicationDbContext _context;

        public MoviesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Movie> Add(Movie movie)
        {
            await _context.AddAsync(movie);
            _context.SaveChanges();
            return movie;
        }

        public Movie Delete(Movie movie)
        {
            _context.Remove(movie);
            _context.SaveChanges();
            return movie;
        }

        public async Task<IEnumerable<Movie>> GetAll(byte genreId = 0)  // 
        {
            return await _context.Movies
                .Where(m=>m.GenreId == genreId || genreId == 0) // to return all data if the genre id equal 0
                .OrderByDescending(r => r.Rate)
                .Include(m => m.Genre).ToListAsync();
            //.Select(m => new MovieDetailsDto
            //{
            //    Id = m.Id,
            //    GenreId = m.GenreId,
            //    GenreName = m.Genre.Name,
            //    Poster = m.Poster,
            //    Rate = m.Rate,
            //    StoreLine = m.StoreLine,
            //    Title = m.Title,
            //    Year = m.Year,
            //})
        }

        public async Task<Movie> GetById(int id)
        {
            return await _context.Movies.Include(g => g.Genre).SingleOrDefaultAsync(x => x.Id == id);
        }

        public Movie Update(Movie movie)
        {
           _context.Update(movie);
            _context.SaveChanges();
            return movie;
        }
    }
}
