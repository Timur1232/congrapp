using System.ComponentModel.DataAnnotations;

namespace Congrapp.Server.Users;

public class User 
{
    [Key] public int Id { get; set; }
    [Required, EmailAddress] public string Email { get; set; }
    [Required] public string PasswordHash { get; set; }
    public bool EmailVarified { get; set; }
}