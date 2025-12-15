using Microsoft.AspNetCore.Mvc;

namespace SASA.Controllers
{
    public class InventoryController : Controller
    {
        // GET: /Inventory
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Inventory/Create
        public IActionResult Create()
        {
            return View();
        }

        // GET: /Inventory/Detail/1
        public IActionResult Detail(int id)
        {
            return View();
        }

        // GET: /Inventory/Edit/1
        public IActionResult Edit(int id)
        {
            return View();
        }
    }
}