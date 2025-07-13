using Congrapp.Server.Data;
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

    public ImagesController(BirthdayDbContext birthdayDbContext, IConfiguration config)
    {
        _birthdayDbContext = birthdayDbContext;
        _config = config;
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
        
        var extension = Path.GetExtension(file.FileName);
        if (extension != ".jpg" && extension != ".jpeg")
        {
            return BadRequest("Only jpg or jpeg files are supported.");
        }
        var fileName = Guid.NewGuid() + extension;
        var filePath = Path.Combine(_config["ImagesUploadDir"]!, fileName);

        var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);
        
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
        
        var filePath = Path.Combine(item.ImagePath);
        item.ImagePath = null;
        _birthdayDbContext.BirthdayInfos.Update(item);
        await _birthdayDbContext.SaveChangesAsync();
        
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("File does not exist.");
        }
        
        System.IO.File.Delete(filePath);

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

        var filePath = Path.Combine(item.ImagePath);
        if (!System.IO.File.Exists(filePath))
        {
            item.ImagePath = null;
            _birthdayDbContext.BirthdayInfos.Update(item);
            await _birthdayDbContext.SaveChangesAsync();
            return NotFound("File does not exist.");
        }

        var imageStream = System.IO.File.OpenRead(filePath);
        return File(imageStream, "image/jpeg");
    }
}