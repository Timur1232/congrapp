using Congrapp.Server.Data;
using Congrapp.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Congrapp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImagesController(BirthdayDbContext birthdayDbContext, ImageManager imageService)
    : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Upload([FromQuery] int birthdayId, [FromForm] IFormFile file)
    {
        var user = await birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound(new {Error = "User not found."});
        }

        var item = await birthdayDbContext.BirthdayInfos
            .FirstOrDefaultAsync(x => x.Id == birthdayId && x.UserId == user.Id);
        if (item == null)
        {
            return NotFound(new {Error = "Item not found."});
        }

        if (item.ImagePath != null)
        {
            _ = imageService.Delete(item.ImagePath);
            item.ImagePath = null;
            birthdayDbContext.BirthdayInfos.Update(item);
            await birthdayDbContext.SaveChangesAsync();
        }
        
        var res = imageService.Save(file);
        if (!res.IsValid)
        {
            return BadRequest(new {res.Error});
        }
        
        var filePath = res.Value;
        item.ImagePath = filePath;
        birthdayDbContext.BirthdayInfos.Update(item);
        await birthdayDbContext.SaveChangesAsync();

        return Ok(item);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] int birthdayId)
    {
        var user = await birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound(new {Error = "User not found."});
        }

        var item = await birthdayDbContext.BirthdayInfos
            .FirstOrDefaultAsync(x => x.Id == birthdayId && x.UserId == user.Id);
        if (item == null)
        {
            return NotFound(new {Error = "Item not found."});
        }
        if (item.ImagePath == null)
        {
            return NoContent();
        }
        
        var res = imageService.Delete(item.ImagePath);
        item.ImagePath = null;
        birthdayDbContext.BirthdayInfos.Update(item);
        await birthdayDbContext.SaveChangesAsync();
        
        if (!res.IsValid)
        {
            return StatusCode(500, res.Error);
        }
        return Ok(item);
    }

    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] int birthdayId)
    {
        var user = await birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound(new {Error = "User not found."});
        }

        var item = await birthdayDbContext.BirthdayInfos
            .FirstOrDefaultAsync(x => x.Id == birthdayId && x.UserId == user.Id);
        if (item == null)
        {
            return NotFound(new {Error = "Item not found."});
        }
        if (item.ImagePath == null)
        {
            return NoContent();
        }

        var res = imageService.Load(item.ImagePath);
        if (!res.IsValid)
        {
            item.ImagePath = null;
            birthdayDbContext.BirthdayInfos.Update(item);
            await birthdayDbContext.SaveChangesAsync();
            return StatusCode(500, res.Error);
        }

        var imageStream = res.Value;
        return File(imageStream, "image/jpeg");
    }
}