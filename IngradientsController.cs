using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DishesDelivery.Models;

namespace DishesDelivery.Controllers
{
    public class IngradientsController : Controller
    {
        private readonly DishesContext _context;

        public IngradientsController(DishesContext context)
        {
            _context = context;
        }

        // GET: Ingradients
        public async Task<IActionResult> Index()
        {
            var dishesContext = _context.Ingradients.Include(i => i.Dish).Include(i => i.Product);
            return View(await dishesContext.ToListAsync());
        }

        // GET: Ingradients/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Ingradients == null)
            {
                return NotFound();
            }

            var ingradient = await _context.Ingradients
                .Include(i => i.Dish)
                .Include(i => i.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ingradient == null)
            {
                return NotFound();
            }

            return View(ingradient);
        }

        // GET: Ingradients/Create
        public IActionResult Create()
        {
            ViewData["DishId"] = new SelectList(_context.Dishes, "Id", "Id");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
            return View();
        }

        // POST: Ingradients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DishId,ProductId,Amount")] Ingradient ingradient)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ingradient);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DishId"] = new SelectList(_context.Dishes, "Id", "Id", ingradient.DishId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", ingradient.ProductId);
            return View(ingradient);
        }

        // GET: Ingradients/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Ingradients == null)
            {
                return NotFound();
            }

            var ingradient = await _context.Ingradients.FindAsync(id);
            if (ingradient == null)
            {
                return NotFound();
            }
            ViewData["DishId"] = new SelectList(_context.Dishes, "Id", "Id", ingradient.DishId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", ingradient.ProductId);
            return View(ingradient);
        }

        // POST: Ingradients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DishId,ProductId,Amount")] Ingradient ingradient)
        {
            if (id != ingradient.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ingradient);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IngradientExists(ingradient.Id))
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
            ViewData["DishId"] = new SelectList(_context.Dishes, "Id", "Id", ingradient.DishId);
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", ingradient.ProductId);
            return View(ingradient);
        }

        // GET: Ingradients/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Ingradients == null)
            {
                return NotFound();
            }

            var ingradient = await _context.Ingradients
                .Include(i => i.Dish)
                .Include(i => i.Product)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (ingradient == null)
            {
                return NotFound();
            }

            return View(ingradient);
        }

        // POST: Ingradients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Ingradients == null)
            {
                return Problem("Entity set 'DishesContext.Ingradients'  is null.");
            }
            var ingradient = await _context.Ingradients.FindAsync(id);
            if (ingradient != null)
            {
                _context.Ingradients.Remove(ingradient);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IngradientExists(int id)
        {
          return (_context.Ingradients?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
