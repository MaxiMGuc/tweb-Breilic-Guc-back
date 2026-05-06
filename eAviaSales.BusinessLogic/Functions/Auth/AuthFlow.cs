using eAviaSales.BusinessLogic.Core.Auth;
using eAviaSales.BusinessLogic.Interface;
using eAviaSales.Domains.Models.User;

namespace eAviaSales.BusinessLogic.Functions.Auth;

public class AuthFlow : AuthActions, IAuthActions
{
    public UserAuthResponse? LoginActionFlow(UserAuthRequest auth)
    {
        if (!ValidateLogin(auth))
        {
            return null;
        }

        var token = GenerateToken(auth);
        if (string.IsNullOrWhiteSpace(token))
        {
            return null;
        }

        return new UserAuthResponse
        {
            Token = token
        };
    }
}
