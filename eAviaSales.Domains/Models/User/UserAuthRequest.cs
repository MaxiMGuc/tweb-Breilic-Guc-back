using System.ComponentModel.DataAnnotations;

namespace eAviaSales.Domains.Models.User;

public class UserAuthRequest
{
    [Required]
    [MinLength(3)]
    public string Login { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    public string Password { get; set; } = string.Empty;
}
