using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace congrapp.Models;

public class BirthdayNote
{
    [Key]
    public int Id { get; set; }
    
    [ForeignKey("BirthdayInfo")]
    public int BirthdayId { get; set; }
    
    [Required]
    public string Note { get; set; }
    
    public BirthdayInfo BirthdayInfo { get; set; }
}