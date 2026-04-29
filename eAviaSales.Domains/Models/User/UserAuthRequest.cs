namespace eAviaSales.Domains.Models.User;

public class UserAuthRequest
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
