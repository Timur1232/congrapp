using Congrapp.Server.Data;
using Congrapp.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Congrapp.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsController : ControllerBase
{
    private readonly BirthdayDbContext _birthdayDbContext;
    private readonly UserDbContext _userDbContext;

    public ItemsController(BirthdayDbContext birthdayDbContext, UserDbContext userDbContext)
    {
        _birthdayDbContext = birthdayDbContext;
        _userDbContext = userDbContext;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var items = await _birthdayDbContext.BirthdayInfos.ToListAsync();
        return Ok(items);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        if (id <= 0)
        {
            return BadRequest("Id must be higher than zero.");
        }
        var item = await _birthdayDbContext.BirthdayInfos.FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] BirthdayInfo item)
    {
        item.Id = 0;
        _birthdayDbContext.BirthdayInfos.Add(item);
        await _birthdayDbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] BirthdayInfo item)
    {
        item.Id = id;
        _birthdayDbContext.BirthdayInfos.Update(item);
        await _birthdayDbContext.SaveChangesAsync();
        return Ok(item);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _birthdayDbContext.BirthdayInfos.FirstOrDefaultAsync(x => x.Id == id);
        if (item == null)
        {
            return NotFound();
        }
        _birthdayDbContext.BirthdayInfos.Remove(item);
        await _birthdayDbContext.SaveChangesAsync();
        return Ok(item);
    }
}