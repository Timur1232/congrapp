using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Congrapp.Server.Models;

public class BirthdayInfo
{
    public record BirthdayInfoDto(int Id, string PersonName, DateOnly BirthdayDate, string? Note);

    [Key] public int Id { get; set; }
    [ForeignKey("User"), Required] public int UserId { get; set; }
    [Required, MaxLength(50)] public string PersonName { get; set; }
    [Required] public DateOnly BirthdayDate { get; set; }
    [MaxLength(100)] public string? Note { get; set; }
    [MaxLength(100)] public string? ImagePath { get; set; }
}