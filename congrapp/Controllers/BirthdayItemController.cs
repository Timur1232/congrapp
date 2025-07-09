using congrapp.Models;
using Microsoft.AspNetCore.Mvc;

namespace congrapp.Controllers;

public class BirthdayItemController : Controller
{
    public IActionResult BirthdayOverview()
    {
        var item = new BirthdayItem() {Name="Timur", BirthdayDate=DateTime.Now};
        return View(item);
    }
}