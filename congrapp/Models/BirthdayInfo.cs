using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace congrapp.Models;

public class BirthdayInfo
{
    [Key]
    public int Id { get; init; }
    
    [Required, DisplayName("Person\'s name")]
    public string PersonName { get; init; }
    
    [Required, DisplayName("Date of birth")]
    public DateTime BirthdayDate { get; init; }
    
    public string? ImagePath { get; init; }
    public List<BirthdayNote>? Notes { get; init; } 
}