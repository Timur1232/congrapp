using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Congrapp.Server.Models;

public class NotificationRecord
{
    public record NotificationRecordDto(int DaysBefore);
    
    [Key] public int Id { get; set; }
    
    [ForeignKey("BirthdayInfo"), Required]
    public int BirthdayId { get; set; }
    
    [Required]
    [Range(1, 10, ErrorMessage = "DaysBefore must be between 1 and 10")]
    public int DaysBefore { get; set; }

    public void Update(NotificationRecordDto recordDto)
    {
        DaysBefore = recordDto.DaysBefore;
    }
}