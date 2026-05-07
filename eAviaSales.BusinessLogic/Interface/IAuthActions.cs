using eAviaSales.Domains.Models.User;

namespace eAviaSales.BusinessLogic.Interface;

public interface IAuthActions
{
    UserAuthResponse? LoginActionFlow(UserAuthRequest auth);
}
