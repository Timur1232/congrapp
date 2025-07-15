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
            return NotFound(new {Error = "User not found."});
        }
        
        var items = await birthdayDbContext.BirthdayInfos
            .Where(x => x.UserId == user.Id)
            .Select(x => x.Respond())
            .ToListAsync();
        return Ok(items); 
    }

    [HttpGet]
    public async Task<IActionResult> GetById([FromQuery] int birthdayId)
    {
        if (birthdayId <= 0)
        {
            return BadRequest(new {Error = "Id must be higher than zero."});
        }
        
        var user = await birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound(new {Error = "User not found."});
        }
        
        var item = await birthdayDbContext.BirthdayInfos
            .FirstOrDefaultAsync(x => x.Id == birthdayId && x.UserId == user.Id);
        if (item == null)
        {
            return NotFound(new {Error = "Birthday not found."});
        }
        return Ok(item.Respond());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BirthdayInfo.BirthdayInfoRequestDto itemRequestDto)
    {
        var user = await birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound(new {Error = "User not found."});
        }

        var item = new BirthdayInfo
        {
            Id = 0,
            UserId = user.Id,
            BirthdayDate = itemRequestDto.BirthdayDate,
            PersonName = itemRequestDto.PersonName,
            Note = itemRequestDto.Note
        };
        
        birthdayDbContext.BirthdayInfos.Add(item);
        await birthdayDbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { birthdayId = item.Id }, item.Respond());
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromQuery] int birthdayId, [FromBody] BirthdayInfo.BirthdayInfoRequestDto itemRequestDto)
    {
        var user = await birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound(new {Error = "User not found."});
        }

        var item = await birthdayDbContext.BirthdayInfos.FirstOrDefaultAsync(x => x.Id == birthdayId);
        if (item == null)
        {
            return NotFound(new {Error = "Birthday not found."});
        }

        item.Update(itemRequestDto);
        
        birthdayDbContext.BirthdayInfos.Update(item);
        await birthdayDbContext.SaveChangesAsync();
        return Ok(item.Respond());
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] int birthdayId)
    {
        var user = await birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound(new {Error = "User not found."});
        }
        
        var item = await birthdayDbContext.BirthdayInfos.FirstOrDefaultAsync(x => x.Id == birthdayId);
        if (item == null)
        {
            return NotFound(new {Error = "Birthday not found."});
        }
        
        birthdayDbContext.BirthdayInfos.Remove(item);
        await birthdayDbContext.SaveChangesAsync();
        await birthdayDbContext.NotificationRecords
            .Where(x => x.BirthdayId == birthdayId)
            .ExecuteDeleteAsync();
        return Ok(item.Respond());
    }
}