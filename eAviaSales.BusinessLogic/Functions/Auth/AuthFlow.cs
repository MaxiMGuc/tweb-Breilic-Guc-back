using eAviaSales.BusinessLogic.Core.Auth;
using eAviaSales.BusinessLogic.Interface;
using eAviaSales.Domains.Models.User;

namespace eAviaSales.BusinessLogic.Functions.Auth;

public class AuthFlow : AuthActions, IAuthActions
{
    public string? LoginActionFlow(UserAuthRequest auth)
    {
        if (!ValidateLogin(auth))
        {
            return null;
        }

        return GenerateToken(auth);
    }
}
