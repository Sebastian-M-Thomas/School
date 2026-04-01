using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spring2026_Project3_smthomas12.Data;
using Spring2026_Project3_smthomas12.Models;
using Spring2026_Project3_smthomas12.Services;
using Spring2026_Project3_smthomas12.ViewModels;

namespace Spring2026_Project3_smthomas12.Controllers
{
    public class ActorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AIService _aiService;

        public ActorsController(ApplicationDbContext context, AIService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Actors.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var actor = await _context.Actors
                .Include(a => a.ActorMovies)
                    .ThenInclude(am => am.Movie)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (actor == null) return NotFound();

            var tweetTexts = await _aiService.GetActorTweetsAsync(actor.Name);
            var tweets = SentimentHelper.AnalyzeAll(tweetTexts);
            var (avg, label, css) = SentimentHelper.Average(tweets);

            var vm = new ActorDetailsViewModel
            {
                Id = actor.Id,
                Name = actor.Name,
                Gender = actor.Gender,
                Age = actor.Age,
                ImdbLink = actor.ImdbLink,
                Photo = actor.Photo,
                MovieTitles = actor.ActorMovies
                                   .Select(am => am.Movie.Title)
                                   .OrderBy(t => t)
                                   .ToList(),
                Tweets = tweets,
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
        public async Task<IActionResult> Create(Actor actor, IFormFile? photoFile)
        {
            ModelState.Remove("ActorMovies");
            ModelState.Remove("Photo");

            if (photoFile != null && photoFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await photoFile.CopyToAsync(ms);
                actor.Photo = ms.ToArray();
            }

            if (ModelState.IsValid)
            {
                _context.Add(actor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(actor);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var actor = await _context.Actors.FindAsync(id);
            if (actor == null) return NotFound();
            return View(actor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Actor actor, IFormFile? photoFile)
        {
            if (id != actor.Id) return NotFound();

            ModelState.Remove("ActorMovies");
            ModelState.Remove("Photo");

            if (photoFile != null && photoFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await photoFile.CopyToAsync(ms);
                actor.Photo = ms.ToArray();
            }
            else
            {
                var existing = await _context.Actors.AsNoTracking()
                                    .FirstOrDefaultAsync(a => a.Id == id);
                actor.Photo = existing?.Photo ?? Array.Empty<byte>();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Actors.Any(a => a.Id == actor.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(actor);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var actor = await _context.Actors.FirstOrDefaultAsync(a => a.Id == id);
            if (actor == null) return NotFound();
            return View(actor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var related = _context.ActorMovies.Where(am => am.ActorId == id);
            _context.ActorMovies.RemoveRange(related);

            var actor = await _context.Actors.FindAsync(id);
            if (actor != null) _context.Actors.Remove(actor);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}