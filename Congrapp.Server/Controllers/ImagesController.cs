using Congrapp.Server.Data;
using Congrapp.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Congrapp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImagesController(BirthdayDbContext birthdayDbContext, IConfiguration config, ImageManager imageService)
    : ControllerBase
{
    private readonly IConfiguration _config = config;

    [HttpPost("{birthdayId}")]
    public async Task<IActionResult> Upload([FromRoute] int birthdayId, IFormFile file)
    {
        var user = await birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var item = await birthdayDbContext.BirthdayInfos
            .FirstOrDefaultAsync(x => x.Id == birthdayId && x.UserId == user.Id);
        if (item == null)
        {
            return NotFound("Item not found.");
        }

        var res = imageService.Save(file);
        if (!res.IsValid)
        {
            return BadRequest(res.Error);
        }
        
        var filePath = res.Value;
        item.ImagePath = filePath;
        birthdayDbContext.BirthdayInfos.Update(item);
        await birthdayDbContext.SaveChangesAsync();

        return Ok(item);
    }

    [HttpDelete("{birthdayId}")]
    public async Task<IActionResult> Delete([FromRoute] int birthdayId)
    {
        var user = await birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var item = await birthdayDbContext.BirthdayInfos
            .FirstOrDefaultAsync(x => x.Id == birthdayId && x.UserId == user.Id);
        if (item == null)
        {
            return NotFound("Item not found.");
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

    [HttpGet("{birthdayId}")]
    public async Task<IActionResult> Get([FromRoute] int birthdayId)
    {
        var user = await birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var item = await birthdayDbContext.BirthdayInfos
            .FirstOrDefaultAsync(x => x.Id == birthdayId && x.UserId == user.Id);
        if (item == null)
        {
            return NotFound("Item not found.");
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