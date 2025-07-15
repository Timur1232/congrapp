using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Congrapp.Server.Models;

public class BirthdayInfo
{
    public record BirthdayInfoRequestDto(string PersonName, DateOnly BirthdayDate, string? Note);
    public record BirthdayInfoRespondDto(int Id, string PersonName, DateOnly BirthdayDate, string? Note, bool HasImage);

    [Key] public int Id { get; set; }
    [ForeignKey("User"), Required] public int UserId { get; set; }
    [Required, MaxLength(50)] public string PersonName { get; set; }
    [Required] public DateOnly BirthdayDate { get; set; }
    [MaxLength(100)] public string? Note { get; set; }
    [MaxLength(100)] public string? ImagePath { get; set; }

    public void Update(BirthdayInfoRequestDto itemRequestDto)
    {
        PersonName = itemRequestDto.PersonName;
        BirthdayDate = itemRequestDto.BirthdayDate;
        Note = itemRequestDto.Note;
    }

    public BirthdayInfoRespondDto Respond()
    {
        return new BirthdayInfoRespondDto
        (
            Id,
            PersonName,
            BirthdayDate,
            Note,
            ImagePath != null
        );
    }
}