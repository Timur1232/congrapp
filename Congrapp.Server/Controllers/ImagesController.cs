using Congrapp.Server.Data;
using Congrapp.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Congrapp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ImagesController : ControllerBase
{
    private readonly BirthdayDbContext _birthdayDbContext;
    private readonly IConfiguration _config;
    private readonly IImageService _imageService;

    public ImagesController(BirthdayDbContext birthdayDbContext, IConfiguration config, IImageService imageService)
    {
        _birthdayDbContext = birthdayDbContext;
        _config = config;
        _imageService = imageService;
    }

    [HttpPost("{birthdayId}")]
    public async Task<IActionResult> Upload([FromRoute] int birthdayId, IFormFile file)
    {
        var user = await _birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var item = await _birthdayDbContext.BirthdayInfos
            .FirstOrDefaultAsync(x => x.Id == birthdayId && x.UserId == user.Id);
        if (item == null)
        {
            return NotFound("Item not found.");
        }

        var res = _imageService.Save(file);
        if (!res.IsValid)
        {
            return BadRequest(res.Error);
        }
        
        var filePath = res.Value;
        item.ImagePath = filePath;
        _birthdayDbContext.BirthdayInfos.Update(item);
        await _birthdayDbContext.SaveChangesAsync();

        return Ok(item);
    }

    [HttpDelete("{birthdayId}")]
    public async Task<IActionResult> Delete([FromRoute] int birthdayId)
    {
        var user = await _birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var item = await _birthdayDbContext.BirthdayInfos
            .FirstOrDefaultAsync(x => x.Id == birthdayId && x.UserId == user.Id);
        if (item == null)
        {
            return NotFound("Item not found.");
        }
        if (item.ImagePath == null)
        {
            return NoContent();
        }
        
        var res = _imageService.Delete(item.ImagePath);
        item.ImagePath = null;
        _birthdayDbContext.BirthdayInfos.Update(item);
        await _birthdayDbContext.SaveChangesAsync();
        
        if (!res.IsValid)
        {
            return StatusCode(500, res.Error);
        }
        return Ok(item);
    }

    [HttpGet("{birthdayId}")]
    public async Task<IActionResult> Get([FromRoute] int birthdayId)
    {
        var user = await _birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var item = await _birthdayDbContext.BirthdayInfos
            .FirstOrDefaultAsync(x => x.Id == birthdayId && x.UserId == user.Id);
        if (item == null)
        {
            return NotFound("Item not found.");
        }
        if (item.ImagePath == null)
        {
            return NoContent();
        }

        var res = _imageService.Load(item.ImagePath);
        if (!res.IsValid)
        {
            item.ImagePath = null;
            _birthdayDbContext.BirthdayInfos.Update(item);
            await _birthdayDbContext.SaveChangesAsync();
            return StatusCode(500, res.Error);
        }

        var imageStream = res.Value;
        return File(imageStream, "image/jpeg");
    }
}