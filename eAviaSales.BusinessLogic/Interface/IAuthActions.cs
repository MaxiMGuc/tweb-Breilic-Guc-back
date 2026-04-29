using eAviaSales.Domains.Models.User;

namespace eAviaSales.BusinessLogic.Interface;

public interface IAuthActions
{
    string? LoginActionFlow(UserAuthRequest auth);
}
