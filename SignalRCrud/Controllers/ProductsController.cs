using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;

using SignalRCrud.Data;
using SignalRCrud.Models;
using SignalRCrud.Hubs;

namespace SignalRCrud.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SignalServer> _signalrHub;

        public ProductsController(ApplicationDbContext context, IHubContext<SignalServer> signalrHub)
        {
            _context = context;
            _signalrHub = signalrHub;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
              return _context.Product != null ? 
                          View(await _context.Product.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Product'  is null.");
        }
        [HttpGet]
        public IActionResult GetProduct()
        {
            var res = _context.Product.ToList();
            return Ok(res);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }
            var products = await _context.Product.FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Item,Category,Price,Qty")] Products products)
        {
            if (ModelState.IsValid)
            {
                _context.Add(products);
                await _context.SaveChangesAsync();
                await _signalrHub.Clients.All.SendAsync("LoadProducts");
                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var products = await _context.Product.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, [Bind("Id,Item,Category,Price,Qty")] Products products)
        {
            if (Id != products.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(products);
                    await _context.SaveChangesAsync();
                    await _signalrHub.Clients.All.SendAsync("LoadProducts");

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.Id))
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
            return View(products);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Product == null)
            {
                return NotFound();
            }

            var products = await _context.Product.FirstOrDefaultAsync(m => m.Id == id);
            if (products == null)
            {
                return NotFound();
            }
            return View(products);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Product == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Product'  is null.");
            }
            var products = await _context.Product.FindAsync(id);
            if (products != null)
            {
                _context.Product.Remove(products);
                await _signalrHub.Clients.All.SendAsync("LoadProducts");

            }

            await _context.SaveChangesAsync();
            await _signalrHub.Clients.All.SendAsync("LoadProducts");
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(int id)
        {
          return (_context.Product?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
