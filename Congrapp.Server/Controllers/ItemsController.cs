using Congrapp.Server.Data;
using Congrapp.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Congrapp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ItemsController : ControllerBase
{
    private readonly BirthdayDbContext _birthdayDbContext;

    public ItemsController(BirthdayDbContext birthdayDbContext)
    {
        _birthdayDbContext = birthdayDbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var user = await _birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound("User not found.");
        }
        
        var items = await _birthdayDbContext.BirthdayInfos
            .Where(x => x.UserId == user.Id)
            .ToListAsync();
        return Ok(items); 
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Id must be higher than zero.");
        }
        
        var user = await _birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound("User not found.");
        }
        
        var item = await _birthdayDbContext.BirthdayInfos
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == user.Id);
        if (item == null)
        {
            return NotFound("Item not found.");
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BirthdayInfo.BirthdayInfoDto itemDto)
    {
        var user = await _birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var item = new BirthdayInfo
        {
            Id = 0,
            UserId = user.Id,
            BirthdayDate = itemDto.BirthdayDate,
            PersonName = itemDto.PersonName,
            Note = itemDto.Note
        };
        
        _birthdayDbContext.BirthdayInfos.Add(item);
        await _birthdayDbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] BirthdayInfo.BirthdayInfoDto itemDto)
    {
        var user = await _birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var item = new BirthdayInfo()
        {
            Id = id,
            UserId = user.Id,
            BirthdayDate = itemDto.BirthdayDate,
            PersonName = itemDto.PersonName,
            Note = itemDto.Note
        };
        
        _birthdayDbContext.BirthdayInfos.Update(item);
        await _birthdayDbContext.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound("User not found.");
        }
        
        var item = await _birthdayDbContext.BirthdayInfos.FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return NotFound("Item not found.");
        }
        
        _birthdayDbContext.BirthdayInfos.Remove(item);
        await _birthdayDbContext.SaveChangesAsync();
        return Ok(item);
    }
}