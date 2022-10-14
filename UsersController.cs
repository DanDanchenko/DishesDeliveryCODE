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
    public class UsersController : Controller
    {
        private readonly DishesContext _context;

        public UsersController(DishesContext context)
        {
            _context = context;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            if (DishesContext.ActiveSiteViewMode != SiteViewMode.Admin)
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var dishesContext = _context.Users.Include(u => u.Role);
            return View(await dishesContext.ToListAsync());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        public async Task<IActionResult> PersonalCabinet()
        {
            //if (DishesContext.ActiveUser.Id == null || _context.Users == null)
            //{
            //    return NotFound();
            //}

            //var user = await _context.Users
            //    .Include(u => u.Role)
            //    .FirstOrDefaultAsync(m => m.Id == DishesContext.ActiveUser.Id);

            if (DishesContext.ActiveUser == null)
            {
                return NotFound();
            }

            return View(DishesContext.ActiveUser);
        }
        // GET: Login
        public IActionResult Login()
        {
           
            return View();
        }
        public IActionResult LogOut()
        {
            DishesContext.ActiveSiteViewMode = SiteViewMode.Login;
            return RedirectToAction("Index", "Home");
        }

        // GET: Login
        public IActionResult UserNotFound()
        {

            return View();
        }

        // POST: Login

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Email, Password")] User user)
        {
            DishesContext.ActiveUser = null;
            foreach (User u in _context.Users)
            {
                if (u.Email == user.Email && u.Password == user.Password)
                {
                    DishesContext.ActiveUser = u;

                    break;
                }

            }

           

            if (DishesContext.ActiveUser != null)
            {
                string rolename = "";
                foreach (Role r in _context.Roles)
                {
                    if (r.Id == DishesContext.ActiveUser.RoleId)
                        rolename = r.Name;
                }

                
                switch (rolename)
                {
                    case "Admin":
                        DishesContext.ActiveSiteViewMode = SiteViewMode.Admin;
                        _context.SetActiveOreder();
                        return RedirectToAction("Index", "Home");
                    case "Client":
                        DishesContext.ActiveSiteViewMode = SiteViewMode.User;
                        _context.SetActiveOreder();
                        return RedirectToAction("Index", "Home");
                    case "Curier":
                        DishesContext.ActiveSiteViewMode = SiteViewMode.Curier;
                        _context.SetActiveOreder();
                        return RedirectToAction("Index", "Home");
                    case "ChefCooker":
                        DishesContext.ActiveSiteViewMode = SiteViewMode.Chef;
                        _context.SetActiveOreder();
                        return RedirectToAction("Index", "Home");
                    default:
                        DishesContext.ActiveSiteViewMode = SiteViewMode.Login;
                        return RedirectToAction("Index", "Home");


                }
   
            }

           
            return RedirectToAction("AccessDenied", "Home");
        }



        // GET: Users/Create
        public IActionResult Register()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id,Name,Surname,Email,Age,Password")] User user)
        {
            foreach (Role r in _context.Roles)
            {
                if (r.Name == "Client")
                    user.RoleId = r.Id;
            }
            
           
                _context.Add(user);
                await _context.SaveChangesAsync();

                DishesContext.ActiveUser = user;
                DishesContext.ActiveSiteViewMode = SiteViewMode.User;
                return RedirectToAction("Index", "Home");
            
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", user.RoleId);
            return View(user);
        }

        //--------------------------------------------

        // GET: Users/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id");
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Surname,Email,Age,RoleId,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", user.RoleId);
            return View(user);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", user.RoleId);
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Surname,Email,Age,RoleId,Password")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", user.RoleId);
            return View(user);
        }

        // GET: Edit personal data

        public async Task<IActionResult> EditPersonalData()
        {
            if (DishesContext.ActiveUser.Id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(DishesContext.ActiveUser.Id);
            if (user == null)
            {
                return NotFound();
            }

            UserViewModel userVM = new UserViewModel();
            userVM.Id = user.Id;
            userVM.Name = user.Name;
            userVM.Email = user.Email;
            userVM.Surname = user.Surname;
            userVM.Age = user.Age;
            userVM.RoleId = user.RoleId;
            userVM.Password = user.Password;
            userVM.Image = user.Image;

            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Id", user.RoleId);
            return View(userVM);
        }

        // POST: Edit personal data

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPersonalData(int id, [Bind("Id,Name,Surname,Email,Age,RoleId,Password, Avatar")] UserViewModel userVM)
        {

            if (id != userVM.Id)
            {
                return NotFound();
            }

            User user = new User();
            user.Id = userVM.Id;
            user.Name = userVM.Name;
            user.Email = userVM.Email;
            user.Surname = userVM.Surname;
            user.Age = userVM.Age;
            user.RoleId = userVM.RoleId;
            user.Password = userVM.Password;
            user.Image = DishesContext.ActiveUser.Image;

            //if (ModelState.IsValid)
            {
                try
                {
                    if (userVM.Avatar != null)
                    {
                        byte[] imageData = null;
                        // считываем переданный файл в массив байтов
                        using (var binaryReader = new BinaryReader(userVM.Avatar.OpenReadStream()))
                        {
                            imageData = binaryReader.ReadBytes((int)userVM.Avatar.Length);
                        }
                        // установка массива байтов
                        user.Image = imageData;
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    DishesContext.ActiveUser = user;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(PersonalCabinet));
            }
            return View(user);
        }


        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'DishesContext.Users'  is null.");
            }
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
