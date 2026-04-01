using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Spring2026_Project3_smthomas12.Data;
using Spring2026_Project3_smthomas12.Models;

namespace Spring2026_Project3_smthomas12.Controllers
{
    public class ActorMoviesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActorMoviesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var actorMovies = await _context.ActorMovies
                .Include(am => am.Actor)
                .Include(am => am.Movie)
                .OrderBy(am => am.Movie.Title)
                .ToListAsync();

            return View(actorMovies);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var actorMovie = await _context.ActorMovies
                .Include(am => am.Actor)
                .Include(am => am.Movie)
                .FirstOrDefaultAsync(am => am.Id == id);

            if (actorMovie == null) return NotFound();
            return View(actorMovie);
        }

        public IActionResult Create()
        {
            ViewBag.ActorId = new SelectList(_context.Actors.OrderBy(a => a.Name), "Id", "Name");
            ViewBag.MovieId = new SelectList(_context.Movies.OrderBy(m => m.Title), "Id", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActorMovie actorMovie)
        {
            
            ModelState.Remove("Actor");
            ModelState.Remove("Movie");

            bool exists = await _context.ActorMovies
                .AnyAsync(am => am.ActorId == actorMovie.ActorId && am.MovieId == actorMovie.MovieId);

            if (exists)
            {
                ModelState.AddModelError("", "This actor is already assigned to that movie.");
                ViewBag.ActorId = new SelectList(_context.Actors.OrderBy(a => a.Name), "Id", "Name", actorMovie.ActorId);
                ViewBag.MovieId = new SelectList(_context.Movies.OrderBy(m => m.Title), "Id", "Title", actorMovie.MovieId);
                return View(actorMovie);
            }

            if (ModelState.IsValid)
            {
                _context.Add(actorMovie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.ActorId = new SelectList(_context.Actors.OrderBy(a => a.Name), "Id", "Name", actorMovie.ActorId);
            ViewBag.MovieId = new SelectList(_context.Movies.OrderBy(m => m.Title), "Id", "Title", actorMovie.MovieId);
            return View(actorMovie);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var actorMovie = await _context.ActorMovies
                .Include(am => am.Actor)
                .Include(am => am.Movie)
                .FirstOrDefaultAsync(am => am.Id == id);

            if (actorMovie == null) return NotFound();
            return View(actorMovie);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actorMovie = await _context.ActorMovies.FindAsync(id);
            if (actorMovie != null) _context.ActorMovies.Remove(actorMovie);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}