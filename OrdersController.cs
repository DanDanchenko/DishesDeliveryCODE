using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DishesDelivery.Models;

namespace DishesDelivery.Controllers
{
    public class OrdersController : Controller
    {
        private readonly DishesContext _context;

        public OrdersController(DishesContext context)
        {
            _context = context;
        }

        public async Task<ActionResult> CloseOrder()
        {
            DishesContext.ActiveOrder.State = 6;
            _context.Update(DishesContext.ActiveOrder);
            _context.SaveChanges();


            return RedirectToAction("ActiveOrderForCurier");

        }

        public async Task<ActionResult> History()
        {

            var history = _context.Orders.Include(o => o.Client).Where(o => o.State == 6);
            var historyType = history;

            switch (DishesContext.ActiveSiteViewMode)
            {
                case SiteViewMode.User:
                    historyType.Where(o => o.ClientId == DishesContext.ActiveUser.Id);
                    break;
                case SiteViewMode.Curier:
                    historyType.Where(o => o.CurierId == DishesContext.ActiveUser.Id);
                    break;
                case SiteViewMode.Chef:
                    historyType.Where(o => o.ChefId == DishesContext.ActiveUser.Id);
                    break;


            }
           
            return View(await historyType.ToListAsync());
        }

        public async Task<ActionResult> ReadyOrdersForCurier()
        {

            var dishesContext = _context.Orders.Include(o => o.Client).Where(o => o.State == 4).Where(o => o.CurierId == DishesContext.ActiveUser.Id);
            return View(await dishesContext.ToListAsync());
        }

        public async Task<IActionResult> GetReadyOrder(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            DishesContext.ActiveOrder = order;
            DishesContext.ActiveOrder.CurierId = DishesContext.ActiveUser.Id;
            DishesContext.ActiveOrder.State = 5;
            _context.Update(DishesContext.ActiveOrder);
            _context.SaveChanges();


            return RedirectToAction("ReadyOrdersForCurier");
        }

        public async Task<ActionResult> SendToCurier(int? id)
        {

            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            DishesContext.ActiveOrder = order;

            DishesContext.ActiveOrder.State = 4;
            _context.Update(DishesContext.ActiveOrder);
            _context.SaveChanges();

            return RedirectToAction("OrdersForChefs");
        }

        public async Task<ActionResult> OrdersForChefs()
        {

            var dishesContext = _context.Orders.Include(o => o.Client).Where(o => o.State == 3);
            return View(await dishesContext.ToListAsync());
        }


        public async Task<IActionResult> ActiveOrderForCurier()
        {

            if (DishesContext.ActiveOrder == null)
            {
                return NotFound();
            }
            String busket = "";

            var busketitems = _context.OrderItems.Where(i => i.OrderId == DishesContext.ActiveOrder.Id);
            List<int> disheIdInOrder = new List<int>();

            Dish dish;
            foreach (var i in busketitems)
            {
                disheIdInOrder.Add(i.DishId);
                dish = _context.Dishes.Find(i.DishId);
                busket += dish.Name + "   ";
            }
            // ir switch off key MultipleActiveResultSets=True then ////
            //foreach (int i in disheIdInOrder)
            //{
            //    dish = _context.Dishes.Find(i);
            //    busket += dish.Name + "   ";
            //}

            ViewBag.busket = busket;
            ViewBag.State = StateOfOrder();
            return View(DishesContext.ActiveOrder);
        }
        public async Task<IActionResult> GetOrder(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Client)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            DishesContext.ActiveOrder = order;
            DishesContext.ActiveOrder.CurierId = DishesContext.ActiveUser.Id;
            DishesContext.ActiveOrder.State = 3;
            _context.Update(DishesContext.ActiveOrder);
            _context.SaveChanges();


            return RedirectToAction("ActiveOrderForCurier");
        }


        public async Task<ActionResult> OrdersForCuriers()
        {

            var dishesContext = _context.Orders.Include(o => o.Client).Where(o => o.State == 2);
            return View(await dishesContext.ToListAsync());
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var dishesContext = _context.Orders.Include(o => o.Client);
            return View(await dishesContext.ToListAsync());
        }

        public String StateOfOrder()
        {
          
            switch (DishesContext.ActiveOrder.State)
            {
                case 0:
                    return "Order is not created";
                case 1:
                    return "Order is in busket";
                case 2:
                    return "Order is payed";
                case 3: 
                    return "Order send to Chef";
                case 4:
                    return "Order send to Curier";
                case 5:
                    return "Curier delivers to Client";
            }
            return "";
        }
        public async Task<IActionResult> Payment()
        {

            DishesContext.ActiveOrder.State = 2;
            //DishesContext.ActiveOrder.CurierId = 
            _context.Update(DishesContext.ActiveOrder);
            _context.SaveChanges();

            return RedirectToAction("ActiveOrder");

           
        }

        // GET: Active order
        public async Task<IActionResult> ActiveOrder()
        {

            if (DishesContext.ActiveOrder == null )
            {
                return NotFound();
            }
            String busket = "";

            var busketitems = _context.OrderItems.Where(i => i.OrderId == DishesContext.ActiveOrder.Id);
            List<int> disheIdInOrder = new List<int>();

            Dish dish;
            foreach (var i in busketitems)
            {
                disheIdInOrder.Add(i.DishId);
                dish = _context.Dishes.Find(i.DishId);
                busket += dish.Name + "   ";
            }
            // ir switch off key MultipleActiveResultSets=True then ////
            //foreach (int i in disheIdInOrder)
            //{
            //    dish = _context.Dishes.Find(i);
            //    busket += dish.Name + "   ";
            //}

            ViewBag.busket = busket;
            ViewBag.State = StateOfOrder();
            return View(DishesContext.ActiveOrder);
        }
        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreateTime,CloseTime,State,ClientId,Address,ChefId,CurierId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id", order.ClientId);
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id", order.ClientId);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CreateTime,CloseTime,State,ClientId,Address,ChefId,CurierId")] Order order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.Id))
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
            ViewData["ClientId"] = new SelectList(_context.Users, "Id", "Id", order.ClientId);
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Orders == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'DishesContext.Orders'  is null.");
            }
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
          return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
