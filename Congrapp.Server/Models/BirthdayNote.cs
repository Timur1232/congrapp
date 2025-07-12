using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Congrapp.Server.Models;

public class BirthdayNote
{
    [Key] public int Id { get; set; }
    [Required] public string Note { get; set; }
    [ForeignKey("BirthdayInfo")] public int BirthdayId { get; set; }
}