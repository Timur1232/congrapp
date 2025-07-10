using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace congrapp.Models;

public class BirthdayInfo
{
    [Key]
    public int Id { get; set; }
    
    [Required, DisplayName("Person\'s name")]
    public string PersonName { get; set; }
    
    [Required, DisplayName("Date of birth")]
    public DateTime BirthdayDate { get; set; }
    
    public string? ImagePath { get; set; }
    public List<BirthdayNote> Notes { get; set; } 
}