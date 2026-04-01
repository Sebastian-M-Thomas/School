using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spring2026_Project3_smthomas12.Data;
using Spring2026_Project3_smthomas12.Models;
using Spring2026_Project3_smthomas12.Services;
using Spring2026_Project3_smthomas12.ViewModels;

namespace Spring2026_Project3_smthomas12.Controllers
{
    public class MoviesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AIService _aiService;

        public MoviesController(ApplicationDbContext context, AIService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Movies.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies
                .Include(m => m.ActorMovies)
                    .ThenInclude(am => am.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            var reviewTexts = await _aiService.GetMovieReviewsAsync(movie.Title, movie.Year);
            var reviews = SentimentHelper.AnalyzeAll(reviewTexts);
            var (avg, label, css) = SentimentHelper.Average(reviews);

            var vm = new MovieDetailsViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                ImdbLink = movie.ImdbLink,
                Genre = movie.Genre,
                Year = movie.Year,
                Poster = movie.Poster,
                ActorNames = movie.ActorMovies
                                  .Select(am => am.Actor.Name)
                                  .OrderBy(n => n)
                                  .ToList(),
                Reviews = reviews,
                AverageSentiment = avg,
                AverageSentimentLabel = label,
                AverageSentimentCss = css
            };

            return View(vm);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Movie movie, IFormFile? posterFile)
        {
            ModelState.Remove("ActorMovies");
            ModelState.Remove("Poster");

            if (posterFile != null && posterFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await posterFile.CopyToAsync(ms);
                movie.Poster = ms.ToArray();
            }

            if (ModelState.IsValid)
            {
                _context.Add(movie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(movie);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var movie = await _context.Movies.FindAsync(id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Movie movie, IFormFile? posterFile)
        {
            if (id != movie.Id) return NotFound();

            ModelState.Remove("ActorMovies");
            ModelState.Remove("Poster");

            if (posterFile != null && posterFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await posterFile.CopyToAsync(ms);
                movie.Poster = ms.ToArray();
            }
            else
            {
                var existing = await _context.Movies.AsNoTracking()
                                    .FirstOrDefaultAsync(m => m.Id == id);
                movie.Poster = existing?.Poster ?? Array.Empty<byte>();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Movies.Any(m => m.Id == movie.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(movie);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null) return NotFound();
            return View(movie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var related = _context.ActorMovies.Where(am => am.MovieId == id);
            _context.ActorMovies.RemoveRange(related);

            var movie = await _context.Movies.FindAsync(id);
            if (movie != null) _context.Movies.Remove(movie);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}