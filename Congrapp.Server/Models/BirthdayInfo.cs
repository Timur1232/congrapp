using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Congrapp.Server.Models;

public class BirthdayInfo
{
    [Key] public int Id { get; set; }
    [ForeignKey("User"), Required] public int UserId { get; set; }
    [Required] public DateOnly BirthdayDate { get; set; }
    [Required] public string PersonName { get; set; }
    public string? ImagePath { get; set; }
}