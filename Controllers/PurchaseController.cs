using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RelationsNaN.Data;

namespace RelationsNaN.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly RelationsNaNContext _context;


        public PurchaseController(RelationsNaNContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> index()
        {

            //THEN INCLUDE DOES LINQ MANIPS TO THE RETRIEVED OBJ FROM THE FIRST INCLUDE JPS
            var relationNaNContext = _context.Purchase.Include(p => p.GamePurchases).ThenInclude(gp => gp.Game);
            return View(await relationNaNContext.ToListAsync());
        
        }
    }
}
