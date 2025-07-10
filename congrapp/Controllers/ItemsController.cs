using congrapp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace congrapp.Controllers;

public class ItemsController : Controller
{
    private readonly BirthdayDbContext _context;

    public ItemsController(BirthdayDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _context.BirthdayInfos.ToListAsync();
        return View(items);
    }

    public IActionResult Create()
    {
        return View();
    }
}