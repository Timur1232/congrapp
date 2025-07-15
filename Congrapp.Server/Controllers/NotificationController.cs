using Congrapp.Server.Data;
using Congrapp.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Congrapp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationsController(BirthdayDbContext birthdayDbContext) : ControllerBase
{
    
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int birthdayId)
    {
        if (birthdayId <= 0)
        {
            return BadRequest("Invalid birthdayId.");
        }
        
        var user = await birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound("User not found.");
        }
        
        var birthday = await birthdayDbContext.BirthdayInfos
            .FirstOrDefaultAsync(x => x.Id == birthdayId && x.UserId == user.Id);
        if (birthday == null)
        {
            return NotFound("Birthday not found.");
        }

        var records = await birthdayDbContext.NotificationRecords
            .Where(x => x.BirthdayId == birthdayId)
            .ToListAsync();
        return Ok(records);
    }

    [HttpGet]
    public async Task<IActionResult> GetById([FromQuery] int birthdayId, [FromQuery] int notificationId)
    {
        if (birthdayId <= 0 || notificationId <= 0)
        {
            return BadRequest("Invalid birthdayId or notificationId.");
        }
        
        var user = await birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound("User not found.");
        }
        
        var birthday = await birthdayDbContext.BirthdayInfos
            .FirstOrDefaultAsync(x => x.Id == birthdayId && x.UserId == user.Id);
        if (birthday == null)
        {
            return NotFound("Birthday not found.");
        }

        var record = await birthdayDbContext.NotificationRecords
            .FirstOrDefaultAsync(x => x.Id == notificationId && x.BirthdayId == birthdayId);
        return Ok(record);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromQuery] int birthdayId, [FromBody] NotificationRecord.NotificationRecordDto recordDto)
    {
        var user = await birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound("User not found.");
        }
        
        var birthday = await birthdayDbContext.BirthdayInfos
            .FirstOrDefaultAsync(x => x.Id == birthdayId && x.UserId == user.Id);
        if (birthday == null)
        {
            return NotFound("Birthday not found.");
        }

        var notificationRecord = new NotificationRecord
        {
            BirthdayId = birthdayId,
            DaysBefore = recordDto.DaysBefore
        };
        
        birthdayDbContext.NotificationRecords.Add(notificationRecord);
        await birthdayDbContext.SaveChangesAsync();
        return Ok(notificationRecord);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromQuery] int birthdayId, [FromQuery] int notificationId,
        [FromBody] NotificationRecord.NotificationRecordDto recordDto)
    {
        var user = await birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound("User not found.");
        }
        
        var birthday = await birthdayDbContext.BirthdayInfos
            .FirstOrDefaultAsync(x => x.Id == birthdayId && x.UserId == user.Id);
        if (birthday == null)
        {
            return NotFound("Birthday not found.");
        }   
        
        var notificationRecord = await birthdayDbContext.NotificationRecords
            .FirstOrDefaultAsync(x => x.Id == notificationId && x.BirthdayId == birthdayId);
        if (notificationRecord == null)
        {
            return NotFound("Notification not found.");
        }
        
        notificationRecord.Update(recordDto);
        birthdayDbContext.NotificationRecords.Update(notificationRecord);
        await birthdayDbContext.SaveChangesAsync();
        
        return Ok(notificationRecord);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] int birthdayId, [FromQuery] int notificationId)
    {
        var user = await birthdayDbContext.GetUserByClaims(User);
        if (user == null)
        {
            return NotFound("User not found.");
        }
        
        var birthday = await birthdayDbContext.BirthdayInfos
            .FirstOrDefaultAsync(x => x.Id == birthdayId && x.UserId == user.Id);
        if (birthday == null)
        {
            return NotFound("Birthday not found.");
        }   
        
        var notificationRecord = await birthdayDbContext.NotificationRecords
            .FirstOrDefaultAsync(x => x.Id == notificationId && x.BirthdayId == birthdayId);
        if (notificationRecord == null)
        {
            return NotFound("Notification not found.");
        }
        
        birthdayDbContext.NotificationRecords.Remove(notificationRecord);
        await birthdayDbContext.SaveChangesAsync();
        
        return Ok(notificationRecord);
    }
}