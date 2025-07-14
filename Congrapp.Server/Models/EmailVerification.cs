using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Congrapp.Server.Models;

public class EmailVerification
{
    [Key] public Guid Id { get; set; }
    [ForeignKey("User")] public int UserId { get; set; }
}