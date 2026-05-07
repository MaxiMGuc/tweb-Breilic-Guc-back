using eAviaSales.Domains.Models.User;

namespace eAviaSales.BusinessLogic.Core.Auth;

public class AuthActions
{
    protected bool ValidateLogin(UserAuthRequest data)
    {
        return !string.IsNullOrWhiteSpace(data.Login) && !string.IsNullOrWhiteSpace(data.Password);
    }

    protected string? GenerateToken(UserAuthRequest data)
    {
        if (!ValidateLogin(data))
        {
            return null;
        }

        return $"token-{data.Login.ToLowerInvariant()}-{Guid.NewGuid():N}";
    }
}
