using Congrapp.Server.Data;
using Congrapp.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Congrapp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ItemsController(BirthdayDbContext birthdayDbContext) : ControllerBase
{
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var user = await birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound("User not found.");
        }
        
        var items = await birthdayDbContext.BirthdayInfos
            .Where(x => x.UserId == user.Id)
            .ToListAsync();
        return Ok(items); 
    }

    [HttpGet]
    public async Task<IActionResult> GetById([FromQuery] int birthdayId)
    {
        if (birthdayId <= 0)
        {
            return BadRequest("Id must be higher than zero.");
        }
        
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
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BirthdayInfo.BirthdayInfoDto itemDto)
    {
        var user = await birthdayDbContext.GetUserByClaims(User);
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
        
        birthdayDbContext.BirthdayInfos.Add(item);
        await birthdayDbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { birthdayId = item.Id }, item);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromQuery] int birthdayId, [FromBody] BirthdayInfo.BirthdayInfoDto itemDto)
    {
        var user = await birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var item = await birthdayDbContext.BirthdayInfos.FirstOrDefaultAsync(x => x.Id == birthdayId);
        if (item == null)
        {
            return NotFound("Item not found.");
        }

        item.Update(itemDto);
        
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
            return NotFound("User not found.");
        }
        
        var item = await birthdayDbContext.BirthdayInfos.FirstOrDefaultAsync(x => x.Id == birthdayId);
        if (item == null)
        {
            return NotFound("Item not found.");
        }
        
        birthdayDbContext.BirthdayInfos.Remove(item);
        await birthdayDbContext.SaveChangesAsync();
        await birthdayDbContext.NotificationRecords
            .Where(x => x.BirthdayId == birthdayId)
            .ExecuteDeleteAsync();
        return Ok(item);
    }
}