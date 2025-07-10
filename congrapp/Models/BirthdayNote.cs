using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace congrapp.Models;

public class BirthdayNote
{
    [Key]
    public int Id { get; init; }
    
    [ForeignKey("BirthdayInfo")]
    public int BirthdayId { get; init; }
    
    [Required]
    public string Note { get; init; }
    
    public BirthdayInfo? BirthdayInfo { get; init; }
}