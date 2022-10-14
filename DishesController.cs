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
    public class DishesController : Controller
    {
        private readonly DishesContext _context;

        Dish activeDish;

        public DishesController(DishesContext context)
        {
            _context = context;
        }

        // GET: Dishes

        public async Task<IActionResult> ListForClient()
        {
            return _context.Dishes != null ?
                 View(await _context.Dishes.ToListAsync()) :
                 Problem("Entity set 'DishesContext.Dishes'  is null.");
        }

        public async Task<IActionResult> Index()
        {
            if (DishesContext.ActiveSiteViewMode == SiteViewMode.Admin 
                || DishesContext.ActiveSiteViewMode == SiteViewMode.Chef)
            {
                return _context.Dishes != null ?
                          View(await _context.Dishes.ToListAsync()) :
                          Problem("Entity set 'DishesContext.Dishes'  is null.");

                
            }

            return RedirectToAction("AccessDenied", "Home");
        }

        // GET: Dishes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (DishesContext.ActiveSiteViewMode == SiteViewMode.Admin
                || DishesContext.ActiveSiteViewMode == SiteViewMode.Chef)
            {


                if (id == null || _context.Dishes == null)
                {
                    return NotFound();
                }

                var dish = await _context.Dishes
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (dish == null)
                {
                    return NotFound();
                }

                return View(dish);
            }

            else
            {
                return RedirectToAction("AccessDenied", "Home");
            }

        }

        public async Task<IActionResult> ClientDetails(int? id)
        {

            if (id == null || _context.Dishes == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dish == null)
            {
                return NotFound();
            }

            String desc = "";

            var dishesIngadients = _context.Ingradients.Include(i => i.Dish).Include(i => i.Product).Where(i => i.DishId == id);
            foreach (var i in dishesIngadients)
            {
                desc += i.Product.Name + "(" + i.Amount + ") "; ;
            }


            //todo description
            ViewBag.DishDescription = desc;

            return View(dish);
        }

        // Add to busket
        public async Task<IActionResult> AddToBusket(int? id)
        {
            if (DishesContext.ActiveOrder == null)
            {
                DishesContext.ActiveOrder = new Order();
                DishesContext.ActiveOrder.State = 0;
            }

            if (DishesContext.ActiveOrder.State == 0)
            {
                DishesContext.ActiveOrder.ClientId = DishesContext.ActiveUser.Id;
                DishesContext.ActiveOrder.State = 1;


                DishesContext.ActiveOrder.ChefId = 0;
                DishesContext.ActiveOrder.CurierId = 0;
                DishesContext.ActiveOrder.Address = "";
                DishesContext.ActiveOrder.CreateTime = DateTime.Now;
                DishesContext.ActiveOrder.CloseTime = DateTime.Now;

                _context.Add(DishesContext.ActiveOrder);
                _context.SaveChanges();

            }

            if (DishesContext.ActiveOrder.State == 1)
            {
                OrderItem item = new OrderItem();
                item.DishId = (int)id;
                item.OrderId = DishesContext.ActiveOrder.Id;
                _context.Add(item);
                _context.SaveChanges();
            }

            return RedirectToAction("ListForClient");
        }

        // GET: Dishes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Dishes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Dish dish)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dish);
        }

        // GET: Dishes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Dishes == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes.FindAsync(id);

            activeDish = new Dish();
            activeDish = dish;

            DishesViewModel dvm = new DishesViewModel();
            dvm.Id = dish.Id;
            dvm.Name = dish.Name;

            if (dvm == null)
            {
                return NotFound();
            }
            return View(dvm);
        }

        // POST: Dishes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name, Avatar")] DishesViewModel dishVM)
        {
            if (id != dishVM.Id)
            {
                return NotFound();
            }

            Dish dish = new Dish();
            dish.Id = dishVM.Id;
            dish.Name = dishVM.Name;

            if (activeDish != null)
                dish.Image = activeDish.Image;

            //if (ModelState.IsValid)
            {
                try
                {
                    if (dishVM.Avatar != null)
                    {
                        byte[] imageData = null;
                        // считываем переданный файл в массив байтов
                        using (var binaryReader = new BinaryReader(dishVM.Avatar.OpenReadStream()))
                        {
                            imageData = binaryReader.ReadBytes((int)dishVM.Avatar.Length);
                        }
                        // установка массива байтов
                        dish.Image = imageData;
                    }

                    _context.Update(dish);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DishExists(dish.Id))
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
            return View(dish);
        }

        // GET: Dishes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Dishes == null)
            {
                return NotFound();
            }

            var dish = await _context.Dishes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (dish == null)
            {
                return NotFound();
            }

            return View(dish);
        }

        // POST: Dishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Dishes == null)
            {
                return Problem("Entity set 'DishesContext.Dishes'  is null.");
            }
            var dish = await _context.Dishes.FindAsync(id);
            if (dish != null)
            {
                _context.Dishes.Remove(dish);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DishExists(int id)
        {
            return (_context.Dishes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
