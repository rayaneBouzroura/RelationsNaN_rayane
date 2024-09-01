using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RelationsNaN.Data;
using RelationsNaN.Models;

namespace RelationsNaN.Controllers
{
    public class GamesController : Controller
    {
        private readonly RelationsNaNContext _context;

        public GamesController(RelationsNaNContext context)
        {
            _context = context;
        }

        // GET: Games
        public async Task<IActionResult> Index()
        {
            var relationsNaNContext = _context.Game.Include(g => g.Genre).Include(g=>g.Platforms);
            return View(await relationsNaNContext.ToListAsync());
        }

        // GET: Games/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(m => m.Id == id); 
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // GET: Games/Create
        public IActionResult Create()
        {
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name");
            return View();
        }

        // POST: Games/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Image,ReleaseYear,GenreId")] Game game)
        {
            if (ModelState.IsValid)
            {
                _context.Add(game);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", game.Name);
            return View(game);
        }







        // GET: Games/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Game == null)
            {
                return NotFound();
            }


            var game = await _context.Game.Include(g => g.Platforms).FirstOrDefaultAsync(x => x.Id == id);

            if (game == null)
            {
                return NotFound();
            }
            //1st param le iterable
            //1d param is the iterable item
            //2nd param is the iterable thingy that will be used as the value of the dropwodown
            //3rd param is the iterable value that will be displayed (we want le nom)
            //4th is the optional standard value that will be the initial one showcased in the drow
            var platforms = _context.Platform.ToList();
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", game.GenreId);
            ViewData["Platforms"] = new SelectList(_context.Platform.Where(p => !game.Platforms.Contains(p)), "Id", "Name");


            return View(game);
        }

        // POST: Games/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Image,ReleaseYear,GenreId")] Game game)
        {
            if (id != game.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(game);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GameExists(game.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            var platforms = _context.Platform.ToList();
            ViewData["Platforms"] = new SelectList(_context.Platform.Where(platform => !game.Platforms.Contains(platform)), "Id", "Name");
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Id", game.GenreId);
            return View(game);
        }

        // GET: Games/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var game = await _context.Game
                .Include(g => g.Genre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            return View(game);
        }

        // POST: Games/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var game = await _context.Game.FindAsync(id);
            if (game != null)
            {
                _context.Game.Remove(game);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GameExists(int id)
        {
            return _context.Game.Any(e => e.Id == id);
        }





        //same method used
        [HttpPost]
        public async Task<IActionResult> RemovePlatform(int gameId, int platformId)
        {
            //bool true since we modifying c tt
            return await EditPlatform(gameId, platformId, false);
        }

        [HttpPost]
        public async Task<IActionResult> AddPlatform(int gameId, int platformId)
        {
            //bool true since we modifying c tt
            return await EditPlatform(gameId, platformId, true);
        }

        //platform actions (selon le bool it either delete la platform from le jeu or it doesnt

        [HttpPost]
        public async Task<IActionResult> EditPlatform(int gameId , int platformId, bool add)
        {
            //recup la pla  tform
            Platform platform = _context.Platform.First(x => x.Id == platformId);
            //recup  the game 
            Game game = _context.Game.Include(g => g.Platforms).First(x => x.Id == gameId);

            //si bool pour ajouter we ajoute sinn we delete 
            if (add)
            {
                game.Platforms.Add(platform);
            }
            else
            {
                game.Platforms.Remove(platform);
            }
            await _context.SaveChangesAsync();
            //TODO : select list is not refreshing ... je la re refresh i guess
            ViewData["Platforms"] = new SelectList(_context.Platform.Where(platform => !game.Platforms.Contains(platform)), "Id", "Name");
            ViewData["GenreId"] = new SelectList(_context.Genre, "Id", "Name", game.GenreId);
            //return to edit view
            return View("Edit", game);

        }

       
    }
}
